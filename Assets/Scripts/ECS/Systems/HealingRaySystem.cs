using ECS.Systems;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(HealerTargetingSystem))]
[UpdateBefore(typeof(DamageSystem))]
[UpdateBefore(typeof(DestroyIfNoParentSystem))]
[UpdateBefore(typeof(RemoveDeadSystem))]
[UpdateBefore(typeof(UnspawnSystem))]
public class HealingRaySystem : JobComponentSystem
{ 
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var healerContainer = GetComponentDataFromEntity<Healer>(true);
        var worldRenderBounds = GetComponentDataFromEntity<WorldRenderBounds>(true);
        var time = UnityEngine.Time.timeSinceLevelLoad *  HealerRayGlobal.Instance.Frequency;

        return Entities
            .WithReadOnly(healerContainer)
            .WithReadOnly(worldRenderBounds)
            .WithAll<HealerRay>()
            .ForEach((ref NonUniformScale scale, ref Rotation rot, in LocalToWorld l2w, in Translation tran, in Parent parent) =>
        {
            var target = healerContainer[parent.Value].target;

            if (target == Entity.Null)
            {
                scale.Value = new float3(0, 0, 0);
                return;
            }
            
            var pos = math.mul(l2w.Value, new float4(tran.Value, 1)).xyz;
            var targetBound = worldRenderBounds[target].Value;
            
            var rnd = noise.snoise(pos + time);
            var rndPos = targetBound.Center + targetBound.Extents * rnd;

            var dist = math.distance(rndPos, pos);
            var dir = math.normalize(rndPos - pos);

            rot.Value = quaternion.LookRotation(dir, new float3(0, 1, 0));
            scale.Value = new float3(1, 1, dist);
        }).Schedule(inputDeps);
    }
}