using Unity.Entities;
using System;

using UnityEngine;

[Serializable]
public struct RandomEnemyShooting : IComponentData
{
    public Entity Prefab;
    public float StartSpeed;
    [Min(0)] public float Period;
}