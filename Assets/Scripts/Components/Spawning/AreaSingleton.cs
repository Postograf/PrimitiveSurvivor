using Unity.Entities;
using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct AreaSingleton : IComponentData
{
    [Tooltip("Половина размера(диагонали)")]
    public float3 Extents;
    public float3 Position;
}