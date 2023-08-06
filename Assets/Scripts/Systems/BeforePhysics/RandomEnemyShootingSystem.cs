using Unity.Core;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Systems;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
public partial struct RandomEnemyShootingSystem : ISystem
{
    [BurstCompile]
    public partial struct ShootingJob : IJobEntity
    {
		public uint Seed;
        public TimeData Time;
        public EntityCommandBuffer Buffer;
        [ReadOnly] public NativeArray<Entity> Enemies;
        [ReadOnly] public ComponentLookup<LocalToWorld> Transforms;

        [BurstCompile]
        public void Execute(
			[ChunkIndexInQuery] int index,
            in LocalToWorld transform,
            in RandomEnemyShooting shooting
        )
        {
            if (
                Time.ComeFor(shooting.Period) == false 
                || Enemies.IsCreated == false 
                || Enemies.Length == 0
            )
                return;
			
			var localSeed = math.max(1u, (uint)index + Seed);
            var random = new Random(localSeed);
            var enemy = Enemies[random.NextInt(0, Enemies.Length)];
            var enemyPosition = Transforms[enemy].Position;
            var position = transform.Position;

            var direction = math.normalizesafe(enemyPosition - position);

            var shot = Buffer.Instantiate(shooting.Prefab);
            Buffer.SetComponent(shot, new Translation { Value = position });
            Buffer.SetComponent(shot, new PhysicsVelocity 
            { 
                Linear = direction * shooting.StartSpeed 
            });
        }
    }

    private ShootingJob _shooting;
    private EntityQuery _enemies;
    private ComponentLookup<LocalToWorld> _transforms;

    private uint _updateCount;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _shooting = new();

        _enemies = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Enemy>()
            .Build(state.EntityManager);

        state.RequireForUpdate(_enemies);
        state.RequireForUpdate<GameStartTimeSIngleton>();

        _transforms = SystemAPI.GetComponentLookup<LocalToWorld>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _transforms.Update(ref state);
		
		var time = SystemAPI.Time;
        var startTime = SystemAPI.GetSingleton<GameStartTimeSIngleton>().Seconds;

        var enemies = _enemies.ToEntityArray(Allocator.TempJob);
        var buffer = new EntityCommandBuffer(Allocator.TempJob);

        _shooting.Time = time;
        _shooting.Buffer = buffer;
        _shooting.Enemies = enemies;
        _shooting.Transforms = _transforms;
		_shooting.Seed = startTime + _updateCount++;
        _shooting.Schedule(state.Dependency).Complete();

        buffer.Playback(state.EntityManager);
        enemies.Dispose();
        buffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}