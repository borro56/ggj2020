using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSSpawner : MonoBehaviour
{
    World world;
    Entity entityPrefab;
    
    [SerializeField] GameObject prefab;
    [SerializeField] float rate;

    void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
        
        var settings = GameObjectConversionSettings.FromWorld(world, new BlobAssetStore());
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        InvokeRepeating(nameof(Spawn), rate, rate);
    }

    void Spawn()
    {
        var em = world.EntityManager;
        var instance = em.Instantiate(entityPrefab);
        var rnd = Random.insideUnitSphere;
        rnd.y = 0;
        
        em.SetComponentData(instance, new Translation {Value = transform.position + rnd});
    }
}