using System;
using System.Collections.Generic;
using System.Timers;
using ECS.Components;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MonoBehaviours
{
    public class DamagerSpawnerConverter : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public float radius = 10;
        public GameObject prefabReference;
        
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabReference);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var rotatingCubePrefabEntity = conversionSystem.GetPrimaryEntity(prefabReference);

            var spawnData = new DamagerSpawner
            {
                prefab =  rotatingCubePrefabEntity,
                radius = radius
            };

            dstManager.AddComponentData(entity, spawnData);

        }
    }
    
    
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(DamagerSpawnerConverter))]

    public class DamageSpawnerEditor : Editor
    {
        private void OnSceneGUI()
        {
            DamagerSpawnerConverter spawnerConverter = (DamagerSpawnerConverter) target;
            Handles.DrawWireDisc( spawnerConverter.transform.position, Vector3.up, spawnerConverter.radius);
        }
    }
#endif
    
}
