using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
            .ForEach((ref BasicSpawner spawner, in Translation translation) =>
            {
                var delta = time - spawner.lastSpawn;
                if(delta < spawner.rate) return;
                spawner.lastSpawn = time;

                var rndX = noise.snoise(translation.Value + time);
                var rndZ = noise.snoise(translation.Value - time);
                var spawnPosition = translation.Value + new float3(rndX, 0, rndZ);
                
                var instance = commandBuffer.Instantiate(0, spawner.prefab);
                commandBuffer.SetComponent(0, instance, new Translation {Value = spawnPosition});
            }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(baseJob);
        return baseJob;
    }
}