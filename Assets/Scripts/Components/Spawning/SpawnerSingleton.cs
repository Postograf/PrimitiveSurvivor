using System;

using Unity.Entities;

[Serializable]
public struct SpawnerSingleton : IBufferElementData
{
    public Entity Prefab;
    public float Offset;

    public float Period;
    public int CountInWave;
}