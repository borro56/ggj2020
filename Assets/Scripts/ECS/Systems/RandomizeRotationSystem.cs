using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RandomizeRotationSystem : JobComponentSystem
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
        var time = UnityEngine.Time.timeSinceLevelLoad;
        var deltaTime = Time.DeltaTime;
        
        var jobHandle= Entities.ForEach((Entity en, ref Rotation rot, ref RandomizeRotation rndRot) =>
        {
            if (rndRot.time > 0)
            {
                rndRot.time -= deltaTime;
                if (rndRot.time <= 0) commandBuffer.RemoveComponent<RandomizeRotation>(0, en);
            }
            
            var deltaRate = rndRot.finalRate - rndRot.startRate;
            var rate = rndRot.startRate + deltaRate * math.min(1, time / rndRot.rateTime);
            
            var delta = time - rndRot.lastTime;
            if(delta < rate) return;
            rndRot.lastTime = time;
            
            var rndY = noise.snoise(rot.Value.value * time * 1256) * 180;
            rot.Value = quaternion.Euler(0, rndY, 0);
        }).Schedule(inputDeps);
        buffer.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}