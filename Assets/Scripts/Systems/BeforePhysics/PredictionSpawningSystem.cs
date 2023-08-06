using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup), OrderFirst = true)]
public partial struct PredictionSpawningSystem : ISystem
{
    private Random _random;
    private float3 _lastAreaPosition;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AreaSingleton>();
        state.RequireForUpdate<SpawnerSingleton>();
        state.RequireForUpdate<GameStartTimeSIngleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        var time = SystemAPI.Time;
        var startTime = SystemAPI.GetSingleton<GameStartTimeSIngleton>().Seconds;

        if (_random.state == 0)
            _random = new Random(math.max(1u, startTime));

        var area = SystemAPI.GetSingleton<AreaSingleton>();
        var spawners = SystemAPI
            .GetSingletonBuffer<SpawnerSingleton>()
            .ToNativeArray(Allocator.Temp);

        var areaMoveDirection = math.normalizesafe(area.Position - _lastAreaPosition);
        var isMoved = math.lengthsq(areaMoveDirection) > 0;

        foreach (var spawner in spawners)
        {
            if (time.ComeFor(spawner.Period) == false)
                continue;

            for (int i = 0; i < spawner.CountInWave; i++)
            {
                var predictedDirection = new float3(_random.NextFloat2Direction(), 0);
				
                if (isMoved)
                    predictedDirection = math.normalizesafe(
                        predictedDirection + areaMoveDirection * 2, 
                        areaMoveDirection
                    );

                var spawnVector = Math.BoundsClamp(
                    predictedDirection, 
                    area.Position, 
                    area.Position, 
                    area.Extents 
                );
				
				if (spawner.Offset != 0)
                    spawnVector += math.normalizesafe(spawnVector) * spawner.Offset;

                var entity = state.EntityManager.Instantiate(spawner.Prefab);
                state.EntityManager.SetComponentData(entity, new Translation 
				{
					Value = area.Position + spawnVector
                });
            }
        }

        spawners.Dispose();
        _lastAreaPosition = area.Position;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}