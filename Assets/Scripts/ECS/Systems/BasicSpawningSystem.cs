using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Video;

public class BasicSpawningSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
        var time = (float)Time.ElapsedTime;
        
        var baseJob = Entities
            .ForEach((ref BasicSpawner spawner, in Translation translation, in Rotation rot, in LocalToWorld l2w) =>
            {
                var deltaRate = spawner.finalRate - spawner.startRate;
                var rate = spawner.startRate + deltaRate * math.min(1, time / spawner.rateTime);
                
                var delta = time - spawner.lastSpawn;
                if(delta < rate) return;
                spawner.lastSpawn = time;

                var globalRotation = quaternion.LookRotation(l2w.Forward, l2w.Up);
                var globalPos = math.mul(l2w.Value, new float4(translation.Value, 1)).xyz;
                
                var up = math.mul(globalRotation, new float3(0, 1, 0));
                var right = math.mul(globalRotation, new float3(1, 0, 0));
                var forward = math.mul(globalRotation, new float3(0, 0, 1));

                var scaledTime = (time * 1036) % (float.MaxValue / 2);
                
                var rndX = noise.snoise(globalPos + scaledTime) * right * spawner.areaSize.x;
                var rndZ = noise.snoise(globalPos - scaledTime) * forward * spawner.areaSize.y;
                var rndY = noise.snoise(globalPos * scaledTime) * up * spawner.areaSize.z;
                var spawnPosition = globalPos + rndX + rndY + rndZ;
                
                var instance = commandBuffer.Instantiate(0, spawner.prefab);
                commandBuffer.SetComponent(0, instance, new Translation {Value = spawnPosition});
                commandBuffer.SetComponent(0, instance, new Rotation{Value = globalRotation}); 
            }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(baseJob);
        return baseJob;
    }
}