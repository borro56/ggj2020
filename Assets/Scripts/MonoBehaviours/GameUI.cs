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
        timer.text = string.Format("{0}:{1}",t.Minutes.ToString("00"),t.Seconds.ToString("00"));
    }

    public void UpdateScore(int s)
    {
        score.text = s.ToString();
    }
}
