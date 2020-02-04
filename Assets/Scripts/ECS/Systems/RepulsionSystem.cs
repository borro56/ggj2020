using ECS.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(DamageSystem))]
[UpdateBefore(typeof(DestroyIfNoParentSystem))]
[UpdateBefore(typeof(RemoveDeadSystem))]
[UpdateBefore(typeof(UnspawnSystem))]
public class RepulsionSystem : JobComponentSystem
{
    EntityQuery targetEntities;
    
    protected override void OnCreate()
    {
        //TODO: Replace the query with a cache of positions
        targetEntities = GetEntityQuery(ComponentType.ReadOnly<Repulsion>(), ComponentType.ReadOnly<Translation>());
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var force = RepulsionGlobal.Instance.Force;
        var minDistance = RepulsionGlobal.Instance.Distance;
        var deltaTime = Time.DeltaTime;
        var repulsionPositions = targetEntities.ToComponentDataArray<Translation>(Allocator.TempJob);

        //TODO: Replace with overlapsphere? Use triggers?
        return Entities
            .WithDeallocateOnJobCompletion(repulsionPositions)
            .WithAll<Repulsion>()
            .ForEach((ref Velocity vel, in Translation trans) =>
        {
            for (var i = 0; i < repulsionPositions.Length; i++)
            {
                var repulsionPosition = repulsionPositions[i].Value;
                if(math.distancesq(repulsionPosition, trans.Value) <= 0.01f) continue;

                var diff = trans.Value - repulsionPosition;
                diff.y = 0;
                
                var lengthSq = math.lengthsq(diff);
                if (lengthSq > minDistance * minDistance) continue;
                
                var dist = math.sqrt(lengthSq);
                var forceCoef = 1 - dist / minDistance;
                var dirToMouse = math.normalize(diff);
                vel.Value += dirToMouse * force * forceCoef * deltaTime;
            }
        }).Schedule(inputDeps);
    }
}

/*
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class RepulsionSystem : JobComponentSystem
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    //[BurstCompile]
    struct TriggerGravityFactorJob : ITriggerEventsJob
    {
        public float deltaTime;
        public float repulsionForce;
        public ComponentDataFromEntity<PhysicsVelocity> velocityContainer;
        [ReadOnly] public ComponentDataFromEntity<Repulsion> repulsionContainer;
        [ReadOnly] public ComponentDataFromEntity<Translation> translationContainer;
        [ReadOnly] public ComponentDataFromEntity<Parent> parentContainer;
        [ReadOnly] public PhysicsWorld physicsWorld;
        
        public void Execute(TriggerEvent triggerEvent)
        {

            
            var bodyA = physicsWorld.Bodies[triggerEvent.BodyIndices.BodyAIndex];

            bodyA.Collider->ge ->GetLeaf(triggerEvent.ColliderKeys.ColliderKeyA, out var a);
            
            
            Entity colliderA = bodyA.Entity;
            Entity colliderB = physicsWorld.Bodies[triggerEvent.BodyIndices.BodyBIndex].Entity;
            if (!repulsionContainer.Exists(colliderA) || !repulsionContainer.Exists(colliderB)) return;
            
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;
            var posA = translationContainer[entityA].Value;
            var posB = translationContainer[entityB].Value;

            var velA = velocityContainer[entityA];
            velA.Linear += math.normalize(posA - posB) * repulsionForce * deltaTime;
            velocityContainer[entityA] = velA;

            var velB = velocityContainer[entityB];
            velB.Linear += math.normalize(posB - posA) * repulsionForce * deltaTime;
            velocityContainer[entityB] = velB;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var repulsionContainer = GetComponentDataFromEntity<Repulsion>(true);
        var translationContainer = GetComponentDataFromEntity<Translation>(true);
        var parentContainer = GetComponentDataFromEntity<Parent>(true);
        var velocityContainer = GetComponentDataFromEntity<PhysicsVelocity>();

        JobHandle jobHandle = new TriggerGravityFactorJob
            {
                repulsionContainer = repulsionContainer,
                velocityContainer = velocityContainer,
                translationContainer = translationContainer,
                parentContainer = parentContainer,
                repulsionForce = RepulsionGlobal.Instance.Force,
                deltaTime = Time.DeltaTime,
                physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld
            }
            .Schedule(m_StepPhysicsWorldSystem.Simulation,
                ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        return jobHandle;
    }
}*/

