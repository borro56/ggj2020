using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    EntityQuery query;
    float time = -1;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject resetButton;

    public bool Won => time >= 0;

    void Awake()
    {
        query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Ship));
    }

    void Update()
    {
        if (!Won && query.CalculateEntityCount() <= 0)
        {
            time = Time.realtimeSinceStartup;
            text.text = "You survived " + time.ToString("000") + " seconds";
            text.gameObject.SetActive(true);
            //resetButton.SetActive(true);
        }
    }
}
