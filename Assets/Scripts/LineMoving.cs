using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineMoving : MonoBehaviour
{
    public Scrollbar sb;
    Vector3 originalPos;
    public float metreLenth = 1.37f;
    float dis;
    public static bool isMe = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        dis = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.isPlayingMusic)
        {
            transform.Translate(0, -1 * (metreLenth / (60f / GM.bpm)) * Time.deltaTime, 0);
            dis += (metreLenth / (60f / GM.bpm)) * Time.deltaTime;
            if (dis >= metreLenth*4*3)
            {
                if (sb.value + 3.0f / GM.barNumber < 1)
                {
                    isMe = true;
                    sb.value += 3.0f / GM.barNumber;
                    isMe = false;
                    //Debug.Log(sb.value + "(+ " + 3.0f / GM.barNumber + ")");
                }
            }
        }
    }

    public void BackToOriginal()
    {
        transform.position = originalPos;
        dis = 0;
    }
}
