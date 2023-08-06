using Unity.Entities;

using UnityEngine;

public class ContactSelfDestroyAuthoring : MonoBehaviour 
{
    public class Baker : Baker<ContactSelfDestroyAuthoring>
    {
        public override void Bake(ContactSelfDestroyAuthoring authoring)
        {
            AddComponent<ContactSelfDestroy>();
        }
    }
}