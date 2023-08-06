using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct DamageIndicationSystem : ISystem
{
    [BurstCompile]
    [WithAll(typeof(DamageIndicator))]
    public partial struct IndicationJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<Health> Health;

        [BurstCompile]
        public void Execute(
            in Parent parent,
            ref NonUniformScale scale
        )
        {
            if (Health.TryGetComponent(parent.Value, out var health) == false)
                return;

            var current = math.clamp(health.Max - health.Current, 0, health.Max);
            scale.Value = new float3(current / health.Max);
        }
    }

    private IndicationJob _indication;
    private ComponentLookup<Health> _health;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _indication = new();

        _health = SystemAPI.GetComponentLookup<Health>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _health.Update(ref state);

        _indication.Health = _health;
        state.Dependency = _indication.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}