using ECS.Components;
using Globals;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Systems
{
    public class DamagerTranslationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var speed = DamagerVelocityGlobal.Instance.speed;
            var deltaTime = Time.DeltaTime;
            return Entities
                .WithAll<Damager>()
                .ForEach((ref Translation trans, in DamagerDirection dir) =>
                {
                    trans.Value = trans.Value + dir.Value * speed * deltaTime;
                }).Schedule(inputDeps);
        }
    }
}