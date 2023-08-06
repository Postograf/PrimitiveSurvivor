using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup), OrderFirst = true)]
public partial struct PlayerTargetingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<EnemiesTargetSingleton> targets) == false)
        {
            state.EntityManager.CreateSingletonBuffer<EnemiesTargetSingleton>();
            targets = SystemAPI.GetSingletonBuffer<EnemiesTargetSingleton>();
        }

        targets.Clear();

        foreach (var transform in SystemAPI.Query<LocalToWorld>().WithAll<Player>())
            targets.Add(new() { Position = transform.Position });
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}