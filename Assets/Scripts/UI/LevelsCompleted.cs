using RotatingRoutes.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsCompleted : MonoBehaviour
{
    private TMPro.TextMeshProUGUI _text;
    void Start()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
        _text.SetText($"Levels completed: {GameManager.ProgressionAmount}");
        GameManager.OnGameStarted += DestroyItself;
    }

    private void DestroyItself(StartSide side)
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        GameManager.OnGameStarted -= DestroyItself;
    }
}
