using Unity.Entities;

using UnityEngine;

public class HealthAuthoring : MonoBehaviour 
{
    public float Value;

    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            AddComponent(new Health
            {
                Current = authoring.Value,
                Max = authoring.Value
            });
        }
    }
}