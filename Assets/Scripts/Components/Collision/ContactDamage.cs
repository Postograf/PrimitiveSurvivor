using Unity.Entities;
using System;

[Serializable]
public struct ContactDamage : IComponentData
{
    public float Value;
}