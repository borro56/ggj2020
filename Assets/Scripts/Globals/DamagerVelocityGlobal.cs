using UnityEngine;

namespace Globals
{
    public class DamagerVelocityGlobal : MonoBehaviour
    {
        public static DamagerVelocityGlobal Instance;

        public float speed = 10;

        void Awake()  { Instance = this; }
    }
}