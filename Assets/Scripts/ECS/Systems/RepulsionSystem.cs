using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class RepulsionSystem : JobComponentSystem
{
    EntityQuery targetEntities;
    
    protected override void OnCreate()
    {
        //TODO: Replace the query with a cache of positions
        targetEntities = GetEntityQuery(ComponentType.ReadOnly<Repulsion>(), ComponentType.ReadOnly<Translation>());
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var force = RepulsionGlobal.Instance.Force;
        var minDistance = RepulsionGlobal.Instance.Distance;
        var deltaTime = Time.DeltaTime;
        var repulsionPositions = targetEntities.ToComponentDataArray<Translation>(Allocator.TempJob);

        return Entities
            .WithDeallocateOnJobCompletion(repulsionPositions)
            .WithAll<Repulsion>()
            .ForEach((ref PhysicsVelocity vel, in Translation trans) =>
        {
            for (var i = 0; i < repulsionPositions.Length; i++)
            {
                var repulsionPosition = repulsionPositions[i].Value;
                if(math.distancesq(repulsionPosition, trans.Value) <= 0.01f) continue;

                var diff = trans.Value - repulsionPosition;
                var lengthSq = math.lengthsq(diff);
                if (lengthSq > minDistance * minDistance) continue;
                
                var dist = math.sqrt(lengthSq);
                var forceCoef = 1 - dist / minDistance;
                var dirToMouse = math.normalize(diff);
                vel.Linear += dirToMouse * force * forceCoef * deltaTime;
            }
        }).Schedule(inputDeps);
    }
}