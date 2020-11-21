using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class Opening : MonoBehaviour
{
    [SerializeField]
    private float translateSpeed = 25f;
    [SerializeField]
    private float secondsBeforeTransition = 30f;

    private float startTime;
    private bool transitInProgress = false;

    private void Awake()
    {
        transitInProgress = false;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.up * translateSpeed * Time.deltaTime);

        if (!transitInProgress && (Input.anyKeyDown || Time.time - startTime >= secondsBeforeTransition))
        {
            TransitionToLevels();
        }
    }

    /// <summary>
    /// Start transition to the next level, AKA the next scene.
    /// </summary>
    public void TransitionToLevels()
    {
        transitInProgress = true;
        this.GetComponent<Animator>().SetBool("transit", true);
        StartCoroutine(LoadNextSceneAfterSeconds(3f));
    }

    private IEnumerator LoadNextSceneAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
