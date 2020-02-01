using UnityEngine;

public class AttractionGlobal : MonoBehaviour
{
    public static AttractionGlobal Instance;

    public float Distance = 10;
    public float Force = 100;

    void Awake()  { Instance = this; }
}