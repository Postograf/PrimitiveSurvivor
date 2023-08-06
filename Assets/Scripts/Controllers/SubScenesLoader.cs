using System.Collections.Generic;

using Unity.Scenes;
using Unity.Entities;
using Unity.Entities.Serialization;

using UnityEditor;

using UnityEngine;

public class SubScenesLoader : MonoBehaviour
{
    [SerializeField] private SubScene[] _scenes;

    private List<Entity> _loadedScenes = new();

    private void Start()
    {
        LoadScenes();
    }

    public void Reload()
    {
        UnloadScenes();
        LoadScenes();
    }

    public void LoadScenes()
    {
        var world = World.DefaultGameObjectInjectionWorld.Unmanaged;

        foreach (var scene in _scenes)
            _loadedScenes.Add(SceneSystem.LoadSceneAsync(world, scene.SceneGUID));
    }

    public void UnloadScenes()
    {
        var world = World.DefaultGameObjectInjectionWorld.Unmanaged;

        foreach (var scene in _loadedScenes)
            SceneSystem.UnloadScene(world, scene);

        _loadedScenes.Clear();
    }
}