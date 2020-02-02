using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RandomizeRotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var time = UnityEngine.Time.realtimeSinceStartup;
        return Entities.ForEach((ref Rotation rot, ref RandomizeRotation rndRot) =>
        {
            var deltaRate = rndRot.finalRate - rndRot.startRate;
            var rate = rndRot.startRate + deltaRate * math.min(1, time / rndRot.rateTime);
            
            var delta = time - rndRot.lastTime;
            if(delta < rate) return;
            rndRot.lastTime = time;
            
            var rndY = noise.snoise(rot.Value.value * time * 1256) * 180;
            rot.Value = quaternion.Euler(0, rndY, 0);
        }).Schedule(inputDeps);
    }
}