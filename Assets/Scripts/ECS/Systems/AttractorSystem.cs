using ECS.Systems;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Group1AttractorSystem : BaseAttractorSystem
{
    public override int TargetGroup => 0;
    public override ButtonControl Button => Mouse.current.leftButton;
}

public class Group2AttractorSystem : BaseAttractorSystem
{
    public override int TargetGroup => 1;
    public override ButtonControl Button => Mouse.current.rightButton;
}

[UpdateBefore(typeof(DamageSystem))]
[UpdateBefore(typeof(DestroyIfNoParentSystem))]
[UpdateBefore(typeof(RemoveDeadSystem))]
[UpdateBefore(typeof(UnspawnSystem))]
public abstract class BaseAttractorSystem : JobComponentSystem
{
    public abstract int TargetGroup { get; }
    public abstract ButtonControl Button { get; }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var cam = Camera.main;
        if (cam == null || !Button.isPressed) return inputDeps; //TODO: Replace with new Unity Input
        
        var targetGroup = TargetGroup;
        
        var deltaTime = Time.DeltaTime;
        var distanceToFloor = cam.transform.position.y;
        var mousePos = new float3(Mouse.current.position.ReadValue(), distanceToFloor);
        float3 mousePos3D = cam.ScreenToWorldPoint(mousePos);

        //TODO: Remove unity physics
        return Entities.ForEach((ref Velocity vel, in Attractor attractor, in Translation trans) =>
        {
            if(attractor.groupId != targetGroup) return;

            var diff = mousePos3D - trans.Value;
            diff.y = 0;
            
            var lengthSq = math.lengthsq(diff);
            if (lengthSq > attractor.Distance * attractor.Distance) return;

            var dist = math.sqrt(lengthSq);
            var distCoef = dist / attractor.Distance;
            var forceCoef = 1 - distCoef * distCoef;
            var dirToMouse = math.normalize(diff);
            vel.Value += dirToMouse * attractor.Force * forceCoef * deltaTime;

        }).Schedule(inputDeps);
    }
}