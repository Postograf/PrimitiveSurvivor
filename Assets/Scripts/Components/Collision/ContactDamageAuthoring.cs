using Unity.Entities;

using UnityEngine;

public class ContactDamageAuthoring : MonoBehaviour
{
    public float Value;

    public class Baker : Baker<ContactDamageAuthoring>
    {
        public override void Bake(ContactDamageAuthoring authoring)
        {
            AddComponent(new ContactDamage { Value = authoring.Value });
        }
    }
}