using UnityEngine;

public class HealerGlobal : MonoBehaviour
{
    public static HealerGlobal Instance;

    public float Amount = 3;
    public float Distance = 15;

    void Awake()  { Instance = this; }
}