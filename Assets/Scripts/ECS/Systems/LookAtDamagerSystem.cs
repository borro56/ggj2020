using ECS.Components;
using Globals;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class LookAtDamagerSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _buffer;
        private EntityQuery _dangerEntities;

        protected override void OnCreate()
        {
            _buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _dangerEntities = GetEntityQuery(
                ComponentType.ReadOnly<Damager>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Team>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var targetPosition = _dangerEntities.ToComponentDataArray<Translation>(Allocator.TempJob);
            var targetTeam = _dangerEntities.ToComponentDataArray<Team>(Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            var rotationSpeed = TurretGlobals.Instance.rotationSpeed;
            var angleLimit = TurretGlobals.Instance.angleLimit;
            
            return Entities
                .WithDeallocateOnJobCompletion(targetPosition)
                .WithDeallocateOnJobCompletion(targetTeam)
                .ForEach((ref Rotation rotation, ref LookAtDamager lookAt, in LocalToWorld l2w, in Translation trans) =>
                {
                    int nearestIndex = -1;
                    float nearestDistance = float.MaxValue;
                    float3 nearestForward = 0;

                    var globalPos = math.mul(l2w.Value, new float4(trans.Value,1)).xyz;
                    
                    for (var i = 0; i < targetPosition.Length; i++)
                    {
                        if(targetTeam[i].id == 1) continue; //TODO: Unhardcode
                        
                        var forward = math.normalize(targetPosition[i].Value - globalPos);
                        var angle = math.dot(math.forward(lookAt.lastRotation), forward);
                        var sqdist = math.distancesq(targetPosition[i].Value, globalPos);

                        if (angle < 1 - angleLimit) continue;
                        
                        if (sqdist < nearestDistance)
                        {
                            nearestIndex = i;
                            nearestDistance = sqdist;
                            nearestForward = forward;
                        }
                    }

                    quaternion targetRotation = lookAt.lastRotation;
                    if (nearestIndex >= 0)
                        lookAt.lastRotation = targetRotation = quaternion.LookRotation(nearestForward, new float3(0, 1, 0));
                    
                    rotation.Value = math.slerp(
                        rotation.Value
                        , targetRotation
                        , deltaTime * rotationSpeed);
                    
                }).Schedule(inputDeps);
        }
    }
}