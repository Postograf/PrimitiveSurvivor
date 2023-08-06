using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
public partial struct InputMovingSystem : ISystem
{
    public struct Direction : IComponentData
    {
        public float3 Value;
    }

    [BurstCompile]
    public partial struct MovingJob : IJobEntity
    {
        public float3 Direction;

        [BurstCompile]
        public void Execute(
            in InputMovement movement,
            ref PhysicsVelocity velocity
        )
        {
            velocity.Linear = Direction * movement.Speed;
        }
    }

    private MovingJob _moving;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _moving = new();
		
        state.EntityManager.AddComponent<Direction>(state.SystemHandle);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _moving.Direction = state.EntityManager.GetComponentData<Direction>(state.SystemHandle).Value;
        state.Dependency = _moving.Schedule(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}