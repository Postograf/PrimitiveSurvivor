using Unity.Entities;
using Unity.Transforms;
using Unity.Physics.Systems;

using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial class CameraMovingSystem : SystemBase
{
    private Camera _camera;
    private Transform _cameraTransform;

    private EntityQuery _players;
    private int _width;
    private int _height;

    protected override void OnCreate()
    {
        _players = new EntityQueryBuilder(Unity.Collections.Allocator.Temp)
            .WithAll<Player, LocalToWorld>()
            .Build(EntityManager);
    }

    protected override void OnUpdate()
    {
        Dependency.Complete();
		
		if (_camera is null) 
		{
			_camera = Camera.main;
			_cameraTransform = _camera.transform;
		}

        if (SystemAPI.HasSingleton<AreaSingleton>() == false)
            EntityManager.CreateSingleton<AreaSingleton>();

        var area = SystemAPI.GetSingletonRW<AreaSingleton>();

        if (_players.IsEmpty == false)
        {
            var player = _players.GetSingleton<LocalToWorld>();
            _cameraTransform.position = new Vector3(
                player.Position.x, 
                player.Position.y, 
                _cameraTransform.position.z
            );
			
            area.ValueRW.Position = player.Position;
        }

        if (_width != _camera.pixelWidth || _height != _camera.pixelHeight)
        {
			_width = _camera.pixelWidth;
			_height = _camera.pixelHeight;
            var topRightCorner = _camera.ScreenToWorldPoint(new Vector3(_width, _height, 0));
            area.ValueRW.Extents = topRightCorner - _cameraTransform.position;
        }
    }
}