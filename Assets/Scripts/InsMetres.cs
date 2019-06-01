using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsMetres : MonoBehaviour
{

    public Transform startPoint;
    public float interval = 1.37f;
    public GameObject metrePrefab;  //  "SixMetre" Prefab
    int metreNumber;
    public List<GameObject> metres = new List<GameObject>();    //  这里还是以一节拍为单位
    public GameObject bars; //  作为所有生成的东西的父物体，方便移动
    public GameObject barNumPrefab; //  barNum 那个Text(UI)预制体
    public GameObject barNumList;

    // Start is called before the first frame update
    void Start()
    {
        metreNumber = GetComponent<GM>().barNum * 4;
        InstantiateMetres();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateMetres()
    {
        Vector3 insPos = startPoint.position;
        Vector3 barPos = startPoint.position;
        barPos.y += 3.2f;
        barPos.x -= 0.5f;
        for (int i = 0; i < metreNumber; i++)
        {
            GameObject tmpObj = Instantiate(metrePrefab, insPos, transform.rotation, bars.transform);
            tmpObj.GetComponent<SixMetre>().metreNum = i;
            if (i % 4 == 0) //  那就是一个小节的第一拍，得有个提示的数字
            {
                GameObject barNumTmpObj = Instantiate(barNumPrefab, Camera.main.WorldToScreenPoint(barPos), transform.rotation, barNumList.transform);
                barNumTmpObj.GetComponent<Text>().text = (i / 4).ToString();
                barPos.x += 4 * interval;
            }
            metres.Add(tmpObj);
            insPos.x += interval;
        }
    }

    public void showBars(float val) //  直接传入Scrollbar的那个value就行
    {
        Vector3 barsPos = new Vector3(0, 0, 0);
        barsPos.x = -1 * metreNumber * interval * val;
        bars.transform.position = barsPos;
    }
}
