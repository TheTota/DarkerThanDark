using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void Play()
    {
        this.GetComponent<Animator>().SetBool("Play", true);
        StartCoroutine(LoadNextScene(2f));
    }

    private IEnumerator LoadNextScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayNotes()
    {
        RuntimeManager.PlayOneShot("event:/MenuAmbiance", new Vector3(0f,1f,-10f));
    }
}
