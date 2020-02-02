using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public float score;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime* UnityEngine.Random.Range(1,130) * 10;
        GameUI.Instance.UpdateScore((int)score);
        GameUI.Instance.UpdateTimer(TimeSpan.FromSeconds(Time.time));
    }
}
