using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class HealerTargetingSystem : JobComponentSystem
    {
        private EntityQuery _targetEntities;

        protected override void OnCreate()
        {
            _targetEntities = GetEntityQuery(
                ComponentType.ReadOnly<Life>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<Healeable>());
            
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var healDistance = HealerGlobal.Instance.Distance;
            var targetPosition = _targetEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
            var targetLife = _targetEntities.ToComponentDataArray<Life>(Allocator.TempJob);
            var targetEntities = _targetEntities.ToEntityArray(Allocator.TempJob);
            
            return Entities
                .WithDeallocateOnJobCompletion(targetPosition)
                .WithDeallocateOnJobCompletion(targetLife)
                .WithDeallocateOnJobCompletion(targetEntities)
                .ForEach((ref Healer healer, in Translation trans) =>
                {
                    int nearestIndex = -1;
                    float nearestDistance = 9999;
                    for (var i = 0; i < targetPosition.Length; i++)
                    {
                        if (targetLife[i].amount >= targetLife[i].maxAmount) continue;
                        
                        var sqdist = math.distancesq(targetPosition[i].Value, trans.Value);

                        if (sqdist < healDistance * healDistance)
                        {
                            if (sqdist < nearestDistance)
                            {
                                nearestIndex = i;
                                nearestDistance = sqdist;
                            }
                        }
                    }

                    if (nearestIndex >= 0)
                    {
                        healer.target = targetEntities[nearestIndex];
                    }
                    else
                    {
                        healer.target = Entity.Null;
                    }
                    
                }).Schedule(inputDeps);
        }
    }
}