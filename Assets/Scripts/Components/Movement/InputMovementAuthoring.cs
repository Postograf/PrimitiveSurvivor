using Unity.Entities;

using UnityEngine;

public class InputMovementAuthoring : MonoBehaviour
{
    public float Speed;

    public class Baker : Baker<InputMovementAuthoring>
    {
        public override void Bake(InputMovementAuthoring authoring)
        {
            AddComponent(new InputMovement
            {
                Speed = authoring.Speed,
            });
        }
    }
}