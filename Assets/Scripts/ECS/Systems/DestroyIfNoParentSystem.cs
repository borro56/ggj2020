using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECS.Systems
{
    //TODO: Destroy this negrada
    public class DestroyIfNoParentSystem : JobComponentSystem
    {
        EndSimulationEntityCommandBufferSystem buffer;

        protected override void OnCreate()
        {
            buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = buffer.CreateCommandBuffer().ToConcurrent();
            var translationContainer = GetComponentDataFromEntity<Translation>(true);
            var jobHandle = Entities.WithReadOnly(translationContainer).WithAll<DestroyIfNoParent>().ForEach((Entity en, in Parent parent) =>
            {
                if (!translationContainer.Exists(parent.Value))
                    commandBuffer.DestroyEntity(0, en);

            }).Schedule(inputDeps);
            buffer.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}