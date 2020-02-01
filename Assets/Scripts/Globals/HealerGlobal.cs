using UnityEngine;

public class HealerGlobal : MonoBehaviour
{
    public static HealerGlobal Instance;

    public float Amount = 10;
    public float Distance = 10;

    void Awake()  { Instance = this; }
}