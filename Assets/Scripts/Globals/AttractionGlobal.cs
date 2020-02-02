using UnityEngine;

public class AttractionGlobal : MonoBehaviour
{
    public static AttractionGlobal Instance;

    public float Distance = 80;
    public float Force = 230;

    void Awake()  { Instance = this; }
}