using UnityEngine;

public class RepulsionGlobal : MonoBehaviour
{
    public static RepulsionGlobal Instance;

    public float Distance = 10;
    public float Force = 100;

    void Awake()  { Instance = this; }
}