using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExplosionSounds : MonoBehaviour
{
    [SerializeField] AudioClip shootSound;
    [SerializeField] float volume = 1;
    [SerializeField] int maxAmount = 3;
    
    EntityQuery query;
    int prevAmount = int.MinValue;

    void Start()
    {
        query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Asteroid));
    }

    void Update()
    {
        var amount = query.CalculateEntityCount();
        var diff = prevAmount - amount;
        prevAmount = amount;

        if(diff > 0)
        {
            var audio = new GameObject().AddComponent<AudioSource>();
            audio.clip = shootSound;
            audio.volume = volume;
            audio.pitch = Random.Range(0.5f, 1.5f);
            audio.Play();
            Destroy(audio.gameObject, audio.pitch * shootSound.length);
        }
    }
}
