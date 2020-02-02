using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonedCOlors : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Material mat;
    [SerializeField] float speed;
    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Update()
    {
        mat.SetColor(EmissionColor, gradient.Evaluate((Time.time * speed) - (int) (Time.time * speed)));
    }
}