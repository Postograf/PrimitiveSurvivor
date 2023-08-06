using System;
using UnityEngine;
using Unity.Entities;

public class GameStartTimeSIngletonAuthoring : MonoBehaviour
{
    public class Baker : Baker<GameStartTimeSIngletonAuthoring>
    {
        public override void Bake(GameStartTimeSIngletonAuthoring authoring)
        {
            var now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            AddComponent(new GameStartTimeSIngleton
            {
                Seconds = (uint)now.TotalSeconds,
            });
        }
    }
}