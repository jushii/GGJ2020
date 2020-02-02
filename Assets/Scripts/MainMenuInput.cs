using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuInput : MonoBehaviour
{
    public Image fadeInImage;
    public AudioClip clip;
    public AudioSource AudioSource;
    private bool isStarting;
    
    private void Update()
    {
        if (Input.GetButtonDown("Joystick_1_A") || Input.GetButtonDown("Joystick_2_A"))
        {
            if (isStarting) return;

            isStarting = true;

            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        fadeInImage.DOColor(Color.black, 0.75f);
        AudioSource.clip = clip;
        AudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("SampleScene");
    }
}
