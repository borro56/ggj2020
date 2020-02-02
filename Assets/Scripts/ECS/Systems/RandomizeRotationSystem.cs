using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MoveForwardSystem))]
public class RandomizeRotationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var time = UnityEngine.Time.timeSinceLevelLoad;
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity en, ref Rotation rot, ref RandomizeRotation rndRot) =>
        {
            if (rndRot.time > 0)
            {
                rndRot.time -= deltaTime;
                if (rndRot.time <= 0) EntityManager.RemoveComponent<RandomizeRotation>(en);
            }

            var deltaRate = rndRot.finalRate - rndRot.startRate;
            var rate = rndRot.startRate + deltaRate * math.min(1, time / rndRot.rateTime);

            var delta = time - rndRot.lastTime;
            if (delta < rate) return;
            rndRot.lastTime = time;

            var rndY = UnityEngine.Random.Range(0, 360f);
            rot.Value = quaternion.Euler(0, rndY, 0);
        });
    }
}