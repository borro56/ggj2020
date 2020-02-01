using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttractorSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var cam = Camera.main;
        if (cam == null || !Mouse.current.leftButton.isPressed) return inputDeps; //TODO: Replace with new Unity Input
        
        var force = AttractionGlobal.Instance.Force;
        var minDistance = AttractionGlobal.Instance.Distance;
        
        var deltaTime = Time.DeltaTime;
        var distanceToFloor = cam.transform.position.y;
        var mousePos = new float3(Mouse.current.position.ReadValue(), distanceToFloor);
        float3 mousePos3D = cam.ScreenToWorldPoint(mousePos);
        
        Debug.DrawRay(mousePos3D, Vector3.forward);

        return Entities.WithAll<AttractorTag>().ForEach((ref PhysicsVelocity vel, in Translation trans) =>
        {
            var diff = mousePos3D - trans.Value;
            var lengthSq = math.lengthsq(diff);
            if (lengthSq > minDistance * minDistance) return;

            var dist = math.sqrt(lengthSq);
            var forceCoef = 1 - dist / minDistance;
            var dirToMouse = math.normalize(diff);
            vel.Linear += dirToMouse * force * forceCoef * deltaTime;

        }).Schedule(inputDeps);
    }
}