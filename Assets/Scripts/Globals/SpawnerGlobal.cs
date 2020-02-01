using UnityEngine;

namespace Globals
{
    public class SpawnerGlobal : MonoBehaviour
    {
        public static SpawnerGlobal Instance;

        public float DamagersOverTime = 1;

        void Awake()  { Instance = this; }
        
    }
}