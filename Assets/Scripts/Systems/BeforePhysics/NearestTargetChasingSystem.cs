using Unity.Core;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics.Systems;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
public partial struct NearestTargetChasingSystem : ISystem
{
    [BurstCompile]
    public partial struct ChasingJob : IJobEntity
    {
        public TimeData Time;
        [ReadOnly] public NativeArray<float3> Targets;

        [BurstCompile]
        public void Execute(
            in PlayerChasing chasing,
            in LocalToWorld transform,
            ref PhysicsVelocity velocity
        )
        {
            var position = transform.Position;
            var (target, distance) = GetNearest(position);

            var vector = target - position;
            var direction = distance == 0 ? vector : vector / distance;
            var speed = math.clamp(chasing.MaxSpeed, 0, distance / Time.DeltaTime);
			
            velocity.Linear = direction * speed;
        }

        private (float3, float) GetNearest(float3 referencePoint)
        {
            var minDistance = float.MaxValue;
            var nearest = referencePoint;

            foreach (var target in Targets) 
            {
                var distance = math.distancesq(referencePoint, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = target;
                }
            }

            return (nearest, math.sqrt(minDistance));
        }
    }

    private ChasingJob _chasing;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _chasing = new();

        state.RequireForUpdate<EnemiesTargetSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var targets = SystemAPI.GetSingletonBuffer<EnemiesTargetSingleton>(true);

        _chasing.Time = SystemAPI.Time;
        _chasing.Targets = targets.Reinterpret<float3>().AsNativeArray();
        state.Dependency = _chasing.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}