using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(AfterPhysicsSystemGroup), OrderLast = true)]
public partial struct ContactSelfDestructionSystem : ISystem
{
    [BurstCompile]
    public partial struct DestructionJob : ICollisionEventsJob
    {
        public EntityCommandBuffer Buffer;
        [ReadOnly] public ComponentLookup<ContactSelfDestroy> SelfDestroy;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (SelfDestroy.HasComponent(collisionEvent.EntityA))
                Buffer.DestroyEntity(collisionEvent.EntityA);
            if (SelfDestroy.HasComponent(collisionEvent.EntityB))
                Buffer.DestroyEntity(collisionEvent.EntityB);
        }
    }

    private DestructionJob _destruction;
    private ComponentLookup<ContactSelfDestroy> _contactSelfDestroy;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _destruction = new();

        _contactSelfDestroy = SystemAPI.GetComponentLookup<ContactSelfDestroy>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _contactSelfDestroy.Update(ref state);

        var buffer = new EntityCommandBuffer(Allocator.TempJob);

        _destruction.Buffer = buffer;
        _destruction.SelfDestroy = _contactSelfDestroy;
        _destruction.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency).Complete();

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}