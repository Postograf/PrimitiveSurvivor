using Unity.Entities;

using UnityEngine;

public class DamageIndicatorAuthoring : MonoBehaviour
{
    public class Baker : Baker<DamageIndicatorAuthoring>
    {
        public override void Bake(DamageIndicatorAuthoring authoring)
        {
            AddComponent<DamageIndicator>();
        }
    }
}