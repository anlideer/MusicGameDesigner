using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject playMusicObj;
    public GameObject stopMusicObj;
    public AudioSource music;
    public InputField firstMetre;
    public Scrollbar sb;
    float musicNow; //  现在在放的位置。
    public GameObject line;
    public GameObject clickType;
    public GameObject lastType;
    public GameObject autoMode;
    public GameObject manualMode;
    public GameObject lineMode;
    public GameObject nonLineMode;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic()
    {
        playMusicObj.SetActive(false);
        stopMusicObj.SetActive(true);
        line.GetComponent<LineMoving>().BackToOriginal();
        if (GM.musicLen * musicNow + GM.firstMetre < GM.musicLen)
        {
            music.time = GM.musicLen * musicNow + GM.firstMetre;
            music.Play();
            GM.isPlayingMusic = true;
        }
        else
        {
            StopMusic();
        }
    }

    public void StopMusic()
    {
        stopMusicObj.SetActive(false);
        playMusicObj.SetActive(true);
        music.Stop();
        GM.isPlayingMusic = false;
    }

    public void AdjustFirstMetre()
    {
        try
        {
            float f = float.Parse(firstMetre.text);
            GM.firstMetre = f;
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void AdjustShownBars()
    {
        GetComponent<InsMetres>().showBars(sb.value);
        if (!LineMoving.isMe)
            StopMusic();
        musicNow = sb.value;
        line.GetComponent<LineMoving>().BackToOriginal();
    }

    public void ChangeMode()
    {
        // 由长按模式切换到点击模式
        if (GM.mode)
        {
            lastType.SetActive(false);
            clickType.SetActive(true);
            GM.mode = !GM.mode;
        }
        // 由点击模式切换到长按模式
        else
        {
            clickType.SetActive(false);
            lastType.SetActive(true);
            GM.mode = !GM.mode;
        }
    }

    public void ChangeAMMode()
    {
        // 由自动模式变为手动模式
        if (GM.isAutoMode)
        {
            autoMode.SetActive(false);
            manualMode.SetActive(true);
            GM.isAutoMode = !GM.isAutoMode;
        }
        // 由手动模式变为自动模式
        else
        {
            manualMode.SetActive(false);
            autoMode.SetActive(true);
            GM.isAutoMode = !GM.isAutoMode;
        }
    }

    public void ChangeLineMode()
    {
        // 由连线模式变为不连线模式
        if (GM.isLineMode)
        {
            lineMode.SetActive(false);
            nonLineMode.SetActive(true);
            GM.isLineMode = !GM.isLineMode;
        }
        // 由不连线模式变为连线模式
        else
        {
            nonLineMode.SetActive(false);
            lineMode.SetActive(true);
            GM.isLineMode = !GM.isLineMode;
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
