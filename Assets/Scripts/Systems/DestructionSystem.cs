using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct DestructionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        var buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (health, entity) in SystemAPI.Query<Health>().WithEntityAccess())
        {
            if (health.Current <= 0)
                buffer.DestroyEntity(entity);
        }

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}