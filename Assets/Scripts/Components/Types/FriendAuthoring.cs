using Unity.Entities;

using UnityEngine;

public class FriendAuthoring : MonoBehaviour
{
    public class Baker : Baker<FriendAuthoring>
    {
        public override void Bake(FriendAuthoring authoring)
        {
            AddComponent<Friend>();
        }
    }
}
