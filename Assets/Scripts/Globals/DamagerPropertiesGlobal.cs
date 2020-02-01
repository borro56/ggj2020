using UnityEngine;

namespace Globals
{
    public class DamagerPropertiesGlobal : MonoBehaviour
    {
        public static DamagerPropertiesGlobal Instance;

        public float speed = 10;
        public float unspawnTime = 10;
        public float damage = 25;

        void Awake()  { Instance = this; }
    }
}