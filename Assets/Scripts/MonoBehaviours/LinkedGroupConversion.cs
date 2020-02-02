using Unity.Entities;
using UnityEngine;

namespace MonoBehaviours
{
    public class LinkedGroupConversion : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            conversionSystem.DeclareLinkedEntityGroup(gameObject);
        }
    }
}