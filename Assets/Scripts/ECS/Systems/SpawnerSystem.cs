using ECS.Components;
using Globals;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
    public class SpawnerSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem buffer;

        protected override void OnCreate()
        {
            buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
            var time = (float)Time.ElapsedTime;
            var spawnsOverTime = SpawnerGlobal.Instance.DamagersOverTime;
            var deathTimer = DamagerPropertiesGlobal.Instance.unspawnTime;
            var speed = DamagerPropertiesGlobal.Instance.speed;
            
            var spawnerJob = Entities
                .ForEach((ref DamagerSpawner spawner, in Translation translation, in LocalToWorld l2w) =>
                {
                    var spawnerWorldPos = math.mul(l2w.Value, new float4(translation.Value, 1)).xyz;

                    if (time - spawner.lastCreatedTime > spawnsOverTime)
                    {
                        var randRotation = noise.snoise(spawnerWorldPos + time) * 360;
                        var quatRotation = quaternion.RotateY(randRotation);
                        var spawnPosition = spawnerWorldPos + math.rotate(quatRotation, new float3(1,0,0)) * spawner.radius;
                        var direction = math.normalize(spawnerWorldPos - spawnPosition);
                        var damagerInstance = commandBuffer.Instantiate(0, spawner.prefab);
                        
                        commandBuffer.SetComponent(0, damagerInstance, new Translation {Value = spawnPosition});
                        commandBuffer.SetComponent(0, damagerInstance, new Velocity {Value = direction * speed});
                        commandBuffer.AddComponent(0, damagerInstance, new DeathTimer()
                        {
                            timeWhenDeathTimerStarted= time,
                            timeUntilObjectIsDestroyed = deathTimer
                        });

                        spawner.lastCreatedTime = time;
                    }
                })
                .Schedule(inputDeps);
            buffer.AddJobHandleForProducer(spawnerJob);
            return spawnerJob;
        }
    }
}