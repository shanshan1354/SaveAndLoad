using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    public Text message;
    public Text shootText;
    public Text scoreText;
    public Toggle bgmToggle;
    public AudioSource bgmSource;

    public int shootNum = 0;
    public int scoreNum = 0;

    private void Awake()
    {
        _instance = this;
        if (PlayerPrefs.HasKey("PlayBGM"))
        {
            if (PlayerPrefs.GetInt("PlayBGM")==0)
            {
                bgmToggle.isOn = false;
                bgmSource.enabled = false;
            }
            else
            {
                bgmToggle.isOn = true;
                bgmSource.enabled = true;
            }
        }
    }
    //是否播放背景音乐
    public void BGMToggle()
    {
        if (bgmToggle.isOn)
        {
            bgmSource.enabled = true;
            PlayerPrefs.SetInt("PlayBGM", 1);
        }
        else
        {
            bgmSource.enabled = false;
            PlayerPrefs.SetInt("PlayBGM", 0);
        }
    }

    public void AddShootNum()
    {
        shootNum++;
        LoadUI();
    }

    public void AddScore()
    {
        scoreNum++;
        LoadUI();
    }
    //刷新UI
    public void UpdateUI()
    {
        shootNum = 0;
        scoreNum = 0;
        LoadUI();
    }
    //加载UI
    public void LoadUI()
    {
        shootText.text = shootNum.ToString();
        scoreText.text = scoreNum.ToString();
    }

    public void ShowMessage(string text)
    {
        message.text = text;
        message.enabled = true;
    }

    public void CloseMessage()
    {
        message.enabled = false;
    }
}
