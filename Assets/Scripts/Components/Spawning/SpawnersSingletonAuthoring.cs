using System;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

public class SpawnersSingletonAuthoring : MonoBehaviour
{
    [Serializable]
    public struct SingleAuthoring
    {
        public GameObject Prefab;
        public float Offset;

        [Min(0)] public float Period;
        [Min(1)] public int CountInWave;
    }

    public List<SingleAuthoring> Value;

    public class Baker : Baker<SpawnersSingletonAuthoring>
    {
        public override void Bake(SpawnersSingletonAuthoring authoring)
        {
            var buffer = AddBuffer<SpawnerSingleton>();

            if (authoring.Value is null)
                return;

            foreach (var item in authoring.Value)
                buffer.Add(new()
                {
                    Period = item.Period,
                    Offset = item.Offset,
                    CountInWave = item.CountInWave,
                    Prefab = GetEntity(item.Prefab),
                });
        }
    }
}