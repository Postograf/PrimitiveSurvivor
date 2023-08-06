using UnityEngine;

using Unity.Entities;
using System;

[Serializable]
public struct PlayerChasing : IComponentData
{
    [Tooltip("Скорость ограничивается ровно до позиции игрока")]
    public float MaxSpeed;
}