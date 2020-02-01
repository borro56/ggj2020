using UnityEngine;

namespace Globals
{
    public class TurretGlobals : MonoBehaviour
    {
        public static TurretGlobals Instance;

        public float angleLimit;
        public float rotationSpeed;

        void Awake()  { Instance = this; }
    }
}