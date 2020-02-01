using UnityEngine;

public class DeathBoundsGlobal : MonoBehaviour
{
    public static DeathBoundsGlobal Instance;

    public Vector3 min;
    public Vector3 max;

    void Awake()  { Instance = this; }
}