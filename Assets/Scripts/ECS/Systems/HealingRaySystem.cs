using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class HealingRaySystem : JobComponentSystem
{ 
    EntityQuery targetEntities;
    
    protected override void OnCreate()
    {
        targetEntities = GetEntityQuery(typeof(Life), ComponentType.ReadOnly<WorldRenderBounds>());
        base.OnCreate();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var healDistance = HealerGlobal.Instance.Distance;
        var deltaTime = Time.DeltaTime;
        var targets = targetEntities.ToComponentDataArray<WorldRenderBounds>(Allocator.TempJob);

        return Entities
            .WithDeallocateOnJobCompletion(targets)
            .WithAll<HealerRay>()
            .ForEach((ref NonUniformScale scale, ref Rotation rot, in LocalToWorld l2w, in Translation tran) =>
        {
            for (var i = 0; i < targets.Length; i++)
            {
                var pos = math.mul(new float4(tran.Value, 1), l2w.Value).xyz;
                var targetBound = targets[i].Value;

                var sqdist = math.distancesq(pos, targetBound.Center);
                Debug.Log(pos);
                if (sqdist > healDistance * healDistance)
                {
                    scale.Value = new float3(0, 0, 0);
                    return;
                }

                var dist = math.sqrt(sqdist);
                var dir = math.normalize(targetBound.Center - pos);

                rot.Value = quaternion.LookRotation(dir, new float3(0, 1, 0));
                scale.Value = new float3(0.25f, 0.25f, dist);
            }
        }).Schedule(inputDeps);
        
    }
}