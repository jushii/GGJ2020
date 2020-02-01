using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] Canvas main_canvas;
    [SerializeField] Canvas annouce_canvas;

    [Header("Progress UI")]
    [SerializeField] Animator progressbar_anim;
    [SerializeField] AnimationClip progressbar_in_clip;
    [SerializeField] AnimationClip progressbar_out_clip;

    [SerializeField] GameObject sun_img;
    [SerializeField] GameObject moon_img;
    [SerializeField] GameObject enemy_img;

    [SerializeField] Image progress_bar_fill;
    [SerializeField] TextMeshProUGUI timer_txt;
    [SerializeField] Color repair_fill_color;
    [SerializeField] Color defence_fill_color;


    [Header("Annouce UI")]
    [SerializeField] Animator annouce_anim;
    [SerializeField] AnimationClip annouce_in_clip;
    [SerializeField] AnimationClip annouce_out_clip;

    [SerializeField] TextMeshProUGUI annouce_txt;
    [SerializeField] string defese_word;
    [SerializeField] string repair_word;
 
    bool isShown_Progress = false;

    public IEnumerator StartStateRepair(string timeLimit, Action onComplete = null)
    {
        if(isShown_Progress) yield return ProgressBar_Close();
        sun_img.SetActive(false);
        moon_img.SetActive(false);
        enemy_img.SetActive(true);
        progress_bar_fill.fillAmount = 1;
        progress_bar_fill.color = repair_fill_color;
        timer_txt.text = timeLimit;
        yield return ProgressBar_Open();

        annouce_txt.text = repair_word;
        yield return Annouce_Open();
        yield return Annouce_Close();

        onComplete?.Invoke();
    }

    public IEnumerator StartStateDefense(string timeLimit, Action onComplete = null)
    {
        if (isShown_Progress) yield return ProgressBar_Close();
        sun_img.SetActive(true);
        moon_img.SetActive(true);
        enemy_img.SetActive(false);
        progress_bar_fill.fillAmount = 1;
        progress_bar_fill.color = defence_fill_color;
        timer_txt.text = timeLimit;
        yield return ProgressBar_Open();

        annouce_txt.text = defese_word;
        yield return Annouce_Open();
        yield return Annouce_Close();

        onComplete?.Invoke();
    }

    public IEnumerator ProgressBar_Open()
    {
        progressbar_anim.SetTrigger("open");
        yield return new WaitForSeconds(progressbar_in_clip.length);
        isShown_Progress = true;

    }

    public IEnumerator ProgressBar_Close()
    {
        progressbar_anim.SetTrigger("close");
        yield return new WaitForSeconds(progressbar_out_clip.length);
        isShown_Progress = false;
    }

    public void UpdateProgress(string timeleft, float amount)
    {
        progress_bar_fill.fillAmount = amount;
        timer_txt.text = timeleft;
    }

    public IEnumerator Annouce_Open()
    {
        annouce_anim.SetTrigger("open");
        yield return new WaitForSeconds(annouce_in_clip.length);
    }

    public IEnumerator Annouce_Close()
    {
        annouce_anim.SetTrigger("close");
        yield return new WaitForSeconds(annouce_out_clip.length);
    }

    //Debug function
    public void Open()
    {
        StartCoroutine(StartStateRepair("30 Sec",()=> { Debug.Log("Finish open repair UI"); }));
    }

    public void Close()
    {
        StartCoroutine(StartStateDefense("60 Sec", ()=> { Debug.Log("Finish open defense UI"); }));
    }
}
