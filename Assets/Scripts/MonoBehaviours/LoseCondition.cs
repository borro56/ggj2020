using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    float delay = 1;
    EntityQuery query;
    float time = -1;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject resetButton;
    [SerializeField] AudioSource music;
    [SerializeField] AudioClip loseMusic;

    public bool Won => time >= 0;

    void Start()
    {
        query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Ship));
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if(delay > 0) return;
        
        if (!Won && query.CalculateEntityCount() <= 0)
        {
            music.clip = loseMusic;
            time = Time.realtimeSinceStartup;
            text.text = "You survived " + time.ToString("000") + " seconds";
            text.gameObject.SetActive(true);
            //resetButton.SetActive(true);
        }
    }
}
