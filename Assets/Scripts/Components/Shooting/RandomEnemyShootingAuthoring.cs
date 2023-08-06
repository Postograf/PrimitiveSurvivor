using Unity.Entities;

using UnityEngine;

public class RandomEnemyShootingAuthoring : MonoBehaviour 
{
    public GameObject Prefab;
    public float StartSpeed;
    [Min(0)] public float Period;

    public class Baker : Baker<RandomEnemyShootingAuthoring>
    {
        public override void Bake(RandomEnemyShootingAuthoring authoring)
        {
            AddComponent(new RandomEnemyShooting
            {
                Period = authoring.Period,
                StartSpeed = authoring.StartSpeed,
                Prefab = GetEntity(authoring.Prefab),
            });
        }
    }
}