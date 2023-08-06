using UnityEngine;

using Unity.Entities;

public class PlayerChasingAuthoring : MonoBehaviour
{
    [Tooltip("Скорость ограничивается ровно до позиции игрока")]
    public float MaxSpeed;

    public class Baker : Baker<PlayerChasingAuthoring>
    {
        public override void Bake(PlayerChasingAuthoring authoring)
        {
            AddComponent(new PlayerChasing
            {
                MaxSpeed = authoring.MaxSpeed
            });
        }
    }
}