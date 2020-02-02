using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BasicSpawningSystem : JobComponentSystem
{
    BeginSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
        var time = UnityEngine.Time.time;
        
        var baseJob = Entities
            .ForEach((Entity en, ref BasicSpawner spawner, in Translation translation, in Rotation rot, in LocalToWorld l2w) =>
            {
                var toSpawn = spawner.amount > 0 ? Mathf.Min(spawner.amount, spawner.amountPerFrame) : spawner.amountPerFrame;

                if (spawner.amount > 0)
                    spawner.amount -= toSpawn;
                
                if (spawner.amount == 0)
                    commandBuffer.RemoveComponent<BasicSpawner>(0, en);

                var deltaRate = spawner.finalRate - spawner.startRate;
                var rate = spawner.startRate + deltaRate * math.min(1, time / spawner.rateTime);
                
                var delta = time - spawner.lastSpawn;
                if(delta < rate) return;
                spawner.lastSpawn = time;

                //TODO: Fix this negrada
                var globalRotation = spawner.global ? quaternion.LookRotation(l2w.Forward, l2w.Up) : rot.Value;
                var globalPos = spawner.global ? math.mul(l2w.Value, new float4(translation.Value, 1)).xyz : translation.Value;
                
                var up = math.mul(globalRotation, new float3(0, 1, 0));
                var right = math.mul(globalRotation, new float3(1, 0, 0));
                var forward = math.mul(globalRotation, new float3(0, 0, 1));

                var scaledTime = (time * 1036) % (float.MaxValue / 2);

                for (int i = 0; i < toSpawn; i++)
                {
                    var rndX = noise.snoise(globalPos * i + scaledTime) * right * spawner.areaSize.x;
                    var rndZ = noise.snoise(globalPos * i - scaledTime) * forward * spawner.areaSize.y;
                    var rndY = noise.snoise(globalPos * i * scaledTime) * up * spawner.areaSize.z;
                    var spawnPosition = globalPos + rndX + rndY + rndZ;

                    var instance = commandBuffer.Instantiate(0, spawner.prefab);
                    commandBuffer.SetComponent(0, instance, new Translation {Value = spawnPosition});

                    if (spawner.setRotation)
                        commandBuffer.SetComponent(0, instance, new Rotation {Value = globalRotation});
                }
            }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(baseJob);
        return baseJob;
    }
}