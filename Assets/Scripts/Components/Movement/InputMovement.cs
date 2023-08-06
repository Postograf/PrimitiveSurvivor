using Unity.Entities;
using System;

[Serializable]
public struct InputMovement : IComponentData
{
    public float Speed;
}