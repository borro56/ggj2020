using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShootSounds : MonoBehaviour
{
    [SerializeField] AudioClip shootSound;

    float delay;
    List<AudioSource> shootSounds = new List<AudioSource>();
    EntityQuery query;

    void Start()
    {
        query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Ship));
    }

    void Update()
    {
        var amount = query.CalculateEntityCount();
        
        for (int i = amount; i < shootSounds.Count; i++)
        {
            Destroy(shootSounds[i].gameObject);
        }

        for (int i = amount; i < shootSounds.Count; i++)
        {
            shootSounds.RemoveAt(shootSounds.Count - 1);
        }

        delay -= Time.deltaTime;
        if (delay < 0)
        {
            delay = shootSound.length / 2;

            if (shootSounds.Count < amount)
            {
                var audio = new GameObject().AddComponent<AudioSource>();
                audio.clip = shootSound;
                audio.loop = true;
                audio.volume = 0.25f;
                audio.pitch = Random.Range(0.5f, 1.5f);
                audio.Play();
                shootSounds.Add(audio);
            }
        }
    }
}
