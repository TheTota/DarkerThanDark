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
    [SerializeField] private GameObject awarenessObj;
    [SerializeField] private RectTransform awarenessHandle;

    private void Awake()
    {
        levelText.text = "level " + (SceneManager.GetActiveScene().buildIndex - 1);
        if ((SceneManager.GetActiveScene().buildIndex - 1) == 1)
        {
            levelText.text += "\n<size=60%>find a way out";
        }
    }

    public void RenderAwareness(float awarenessValue)
    {
        // Show or hide the awareness slider
        if (awarenessValue == 0 && awarenessObj.activeInHierarchy)
        {
            awarenessObj.SetActive(false);
        }
        else if (awarenessValue > 0 && !awarenessObj.activeInHierarchy)
        {
            awarenessObj.SetActive(true);
        }

        // Render the awareness value
        awarenessHandle.localScale = new Vector3(awarenessValue, 1f, 1f);
    }
}
