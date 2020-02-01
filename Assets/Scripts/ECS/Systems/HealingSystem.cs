using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class HealingSystem : JobComponentSystem
{
    EntityQuery targetEntities;
    
    protected override void OnCreate()
    {
        targetEntities = GetEntityQuery(typeof(Life), ComponentType.ReadOnly<Translation>());
        base.OnCreate();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return inputDeps;
    }
}