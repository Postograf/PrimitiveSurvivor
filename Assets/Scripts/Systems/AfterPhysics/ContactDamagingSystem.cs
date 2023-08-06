using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct ContactDamagingSystem : ISystem
{
    [BurstCompile]
    public partial struct CollisionJob : ICollisionEventsJob
    {
        public EntityCommandBuffer Buffer;
        [ReadOnly] public ComponentLookup<Health> Health;
        [ReadOnly] public ComponentLookup<ContactDamage> ContactDamages;

        public void Execute(CollisionEvent collisionEvent)
        {
            var isADamage = ContactDamages.TryGetComponent(collisionEvent.EntityA, out var aDamage);
            var isBDamage = ContactDamages.TryGetComponent(collisionEvent.EntityB, out var bDamage);

            if (isADamage == false && isBDamage == false)
                return;

            var isAHealth = Health.TryGetComponent(collisionEvent.EntityA, out var aHealth);
            var isBHealth = Health.TryGetComponent(collisionEvent.EntityB, out var bHealth);

            if (isADamage && isBHealth)
            {
                bHealth.Current -= aDamage.Value;
                Buffer.SetComponent(collisionEvent.EntityB, bHealth);
            }

            if (isBDamage && isAHealth)
            {
                aHealth.Current -= bDamage.Value;
                Buffer.SetComponent(collisionEvent.EntityA, aHealth);
            }
        }
    }

    private CollisionJob _collision;
    private ComponentLookup<Health> _health;
    private ComponentLookup<ContactDamage> _contactDamage;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _collision = new();

        _health = SystemAPI.GetComponentLookup<Health>(true);
        _contactDamage = SystemAPI.GetComponentLookup<ContactDamage>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _health.Update(ref state);
        _contactDamage.Update(ref state);

        var buffer = new EntityCommandBuffer(Allocator.TempJob);

        _collision.Buffer = buffer;
        _collision.Health = _health;
        _collision.ContactDamages = _contactDamage;
        _collision.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency).Complete();

        buffer.Playback(state.EntityManager);
        buffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}