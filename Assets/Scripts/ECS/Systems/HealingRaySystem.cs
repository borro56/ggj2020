using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
        var time = (float)Time.ElapsedTime *  HealerRayGlobal.Instance.Frequency;
        var targets = targetEntities.ToComponentDataArray<WorldRenderBounds>(Allocator.TempJob);

        return Entities
            .WithDeallocateOnJobCompletion(targets)
            .WithAll<HealerRay>()
            .ForEach((ref NonUniformScale scale, ref Rotation rot, in LocalToWorld l2w, in Translation tran) =>
        {
            var pos = math.mul(l2w.Value, new float4(tran.Value, 1)).xyz;
            for (var i = 0; i < targets.Length; i++)
            {
                var targetBound = targets[i].Value;

                var sqdist = targetBound.DistanceSq(pos);
                if (sqdist > healDistance * healDistance)
                {
                    scale.Value = new float3(0, 0, 0);
                    return;
                }

                var rnd = noise.snoise(pos + time);
                var rndPos = targetBound.Center + targetBound.Extents * rnd;
                
                var dist = math.sqrt(sqdist);
                var dir = math.normalize(rndPos - pos);

                rot.Value = quaternion.LookRotation(dir, new float3(0, 1, 0));
                scale.Value = new float3(1, 1, dist);
            }
        }).Schedule(inputDeps);
    }
}