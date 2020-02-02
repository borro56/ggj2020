using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [SerializeField] public TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI score;


    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTimer(TimeSpan t)
    {
        timer.text = string.Format("{0:00}:{1:00}", (int)t.TotalMinutes,(int)t.TotalSeconds);
    }

    public void UpdateScore(int s)
    {
        score.text = s.ToString();
    }
}
