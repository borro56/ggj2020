using System;
using Unity.Entities;
using UnityEngine;

namespace Globals
{
    public class DamagerPropertiesGlobal : MonoBehaviour
    {
        public static DamagerPropertiesGlobal Instance;

        public float speed = 10;
        public float unspawnTime = 10;
        public float damage = 25;
        public GameObject prefab;

        public Entity EntityPrefab { get; private set; }

        void Awake()  { Instance = this; }

        void Start()
        { 
           /*var world = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(world, new BlobAssetStore());*/
            //EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);
        }
    }
}