using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RandomizeRotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var time = (float)Time.ElapsedTime;
        return Entities.ForEach((ref Rotation rot, ref RandomizeRotation rndRot) =>
        {
            var delta = time - rndRot.lastTime;
            if(delta < rndRot.rate) return;
            rndRot.lastTime = time;
            
            var rndY = noise.snoise(rot.Value.value * time * 123156) * 180;
            rot.Value = quaternion.Euler(0, rndY, 0);
        }).Schedule(inputDeps);
    }
}