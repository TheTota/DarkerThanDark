using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject gameOverObj;
    [SerializeField] private RectTransform awarenessHandle;

    private void Awake()
    {
        levelText.text = "level " + SceneManager.GetActiveScene().buildIndex;
    }

    public void RenderAwareness(float awarenessValue)
    {
        awarenessHandle.localScale = new Vector3(awarenessValue, 1f, 1f);
    }
}
