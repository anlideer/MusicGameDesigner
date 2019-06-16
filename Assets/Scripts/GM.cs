using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后续优化的话，可以让超出屏幕的物体就setActive(false)，UI跟物体都是。现在就先这样...还不至于有啥性能问题...
/// </summary>
/// 
/*
public class IconObj
{
    GameObject obj;
    bool type;  //  false - 点击式, true - 长按式
    int track;  //  轨道编号
    float endTime;
    List<IconObj> mainPoints = null;   //  长按式中间那些点
}
*/

public class GM : MonoBehaviour
{
    public static float bpm = 63.745f;    // 换音乐了再重新设
    public static int barNumber;
    public AudioSource music;
    public static float musicLen;
    public int barNum = 39;
    public static float firstMetre = 0;
    public static bool isPlayingMusic = false;
    public static bool mode = false;    //  false - 点击式，true - 长按式
    public static bool isAutoMode = true;   // true - 自动贴合模式， false - 手动调控模式
    public static bool isLineMode = false;  //  false - 不连线， true - 连线


    // Start is called before the first frame update
    void Start()
    {
        musicLen = music.GetComponent<AudioSource>().clip.length;
        barNumber = barNum;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
