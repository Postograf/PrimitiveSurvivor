using Unity.Entities;
using System;

[Serializable]
public struct Health : IComponentData
{
    public float Current;
    public float Max;
}