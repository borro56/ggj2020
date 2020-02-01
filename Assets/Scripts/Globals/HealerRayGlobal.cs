using UnityEngine;

public class HealerRayGlobal : MonoBehaviour
{
    public static HealerRayGlobal Instance;

    public float RandomRange = 10;

    void Awake()  { Instance = this; }
}