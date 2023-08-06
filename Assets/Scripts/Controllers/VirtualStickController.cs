using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

public class VirtualStickController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;

    private void Awake()
    {
        _joystick.Moved += OnMove;
    }

    private void OnMove(float3 direction)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var system = world.GetOrCreateSystem<InputMovingSystem>();
		
        world.EntityManager.SetComponentData(system, new InputMovingSystem.Direction
        {
            Value = direction
        });
    }
}