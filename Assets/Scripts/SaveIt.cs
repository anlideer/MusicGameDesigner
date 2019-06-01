using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// 前排提示一下，点击式音符的tag是clickIcon，长按式音符的tag是lastIcon。查找物体会用到。
/// </summary>
/// 

[System.Serializable]
public class MyData
{
    public List<MusicIcon> iconList;    // 最后按pos排下序就行辽，没必要用dictionary了
}


[System.Serializable]
public class MusicIcon
{
    public bool type;   //  false - 点击式, true - 长按式
    public int track;   //  轨道编号
    public float pos;   //  在小节中的位置
    public int code;    //  唯一标识符
    public int lastPoint = 0;
    public int nextPoint = 0;   //  用唯一标识符来表示节点间的关系，因为貌似json不支持引用那种...
}


public class SaveIt : MonoBehaviour
{
    
    string fileName = @"D:\Dream\myData.json";
    private MyData loadedData;
    private bool loaded = false;
    private List<GameObject> objList;
    private List<GameObject> metreList;

    public GameObject clickIconPrefab;
    public GameObject lastIconPrefab;
    public GameObject successMes;
    private float timecnt;
    private bool shown = false;

    // Start is called before the first frame update
    void Start()
    {
        loaded = false;
        shown = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
        {
            loaded = true;
            // load
            // 先获取一下metreList
            metreList = new List<GameObject>(GameObject.FindGameObjectsWithTag("metre"));
            LoadJson();
            LoadDatatoObj();
            // 处理一下最后的关联
            DealWithRelated2();
        }

        if (shown && timecnt + 1f < Time.time)
        {
            shown = false;
            successMes.SetActive(false);
        }
    }

    void LoadJson() //  加载
    {
        if (File.Exists(fileName))
        {
            // 有就加载进来呗
            string dataAsJson = File.ReadAllText(fileName); //读取所有数据送到json格式的字符串里面。
            //直接赋值。FromJson
            loadedData = JsonUtility.FromJson<MyData>(dataAsJson);
            Debug.Log("loaded");
        }
        else
        {
            // 没有就创建一个呗
            loadedData = new MyData();
            loadedData.iconList = new List<MusicIcon>();
            string tmps = JsonUtility.ToJson(loadedData);
            File.WriteAllText(fileName, tmps);
            Debug.Log("created");
        }
    }

    // 将data转换为可以看见的东西们
    void LoadDatatoObj()
    {
        objList = new List<GameObject>();
        foreach(MusicIcon icon in loadedData.iconList)
        {
            GameObject metre = FindMetre(icon.pos, icon.track);
            if (metre == null)
            {
                Debug.Log("error in finding metre");
            }
            else
            {
                Transform left = metre.transform.Find("Left");
                Transform right = metre.transform.Find("Right");
                Vector3 v = left.position;
                v.x += (icon.pos - metre.transform.parent.GetComponent<SixMetre>().metreNum) * Vector3.Distance(left.position, right.position);
                GameObject iconObj;
                // 点击式
                if (icon.type == false)
                {
                    iconObj = Instantiate(clickIconPrefab, v, transform.rotation);
                }
                // 下落式
                else
                {
                    iconObj = Instantiate(lastIconPrefab, v, transform.rotation);
                }
                iconObj.GetComponent<EditIcon>().type = icon.type;
                iconObj.GetComponent<EditIcon>().pos = icon.pos;
                iconObj.GetComponent<EditIcon>().track = icon.track;
                iconObj.transform.parent = metre.transform;
                objList.Add(iconObj);
            }
        }
    }

    // 处理物体间关联的（读取json过程
    void DealWithRelated2()
    {
        for (int i = 0; i < loadedData.iconList.Count; i++)
        {
            int last = loadedData.iconList[i].lastPoint;
            int next = loadedData.iconList[i].nextPoint;
            if (last != 0)
            {
                int lastP = FindIconInIconList(last);
                if (lastP == -1)
                {
                    Debug.Log("error in finding icon");
                }
                else
                {
                    objList[i].GetComponent<EditIcon>().lastPoint = objList[lastP];
                }
            }
            if (next != 0)
            {
                int nextP = FindIconInIconList(next);
                if (nextP == -1)
                {
                    Debug.Log("error in finding icon");
                }
                else
                {
                    objList[i].GetComponent<EditIcon>().nextPoint = objList[nextP];
                }
            }
        }
    }

    GameObject FindMetre(float pos, int track)
    {
        int metreNum = (int)pos;
        foreach(GameObject obj in metreList)
        {
            if (obj.transform.parent.GetComponent<SixMetre>().metreNum == metreNum && obj.GetComponent<Metre>().trackNo == track)
            {
                return obj;
            }
        }
        return null;
    }
    // 将现有的这些东西转换为data
    void TransferObjtoData()
    {
        loadedData.iconList = new List<MusicIcon>();    // 先清空，重新填东西
        GameObject[] clickIcons = GameObject.FindGameObjectsWithTag("clickIcon");
        GameObject[] lastIcons = GameObject.FindGameObjectsWithTag("lastIcon");
        ObjListToData(clickIcons);
        ObjListToData(lastIcons);
        // 处理前置节点、后置节点的事情
        DealWithRelated();
        // 排序
        loadedData.iconList = loadedData.iconList.OrderBy(o => o.pos).ToList();
        // 应该星了
    }

    void ObjListToData(GameObject[] objs)
    {
        
        foreach(GameObject obj in objs)
        {
            float tmpPos = obj.GetComponent<EditIcon>().pos;
            int tmpTrack = obj.GetComponent<EditIcon>().track;
            bool tmpType = obj.GetComponent<EditIcon>().type;
            MusicIcon tmpMusicIcon = new MusicIcon();
            tmpMusicIcon.type = tmpType;
            tmpMusicIcon.track = tmpTrack;
            tmpMusicIcon.pos = tmpPos;
            tmpMusicIcon.code = objList.Count + 1;
            loadedData.iconList.Add(tmpMusicIcon);
            objList.Add(obj);
        }
    }

    void DealWithRelated()
    {
        for (int i = 0; i < objList.Count; i++)
        {
            GameObject last = objList[i].GetComponent<EditIcon>().lastPoint;
            GameObject next = objList[i].GetComponent<EditIcon>().nextPoint;
            if (last != null)
            {
                int lastP = FindObjInObjList(last);
                if (lastP == -1)
                {
                    Debug.Log("error in finding obj");
                }
                else
                {
                    loadedData.iconList[i].lastPoint = loadedData.iconList[lastP].code;
                }
            }
            if (next != null)
            {
                int nextP = FindObjInObjList(next);
                if (nextP == -1)
                {
                    Debug.Log("error in finding obj");
                }
                else
                {
                    loadedData.iconList[i].nextPoint = loadedData.iconList[nextP].code;
                }
            }
        }
    }

    int FindObjInObjList(GameObject obj)
    {
        for(int i = 0; i < objList.Count; i++)
        {
            if (objList[i].Equals(obj))
            {
                return i;
            }
        }
        return -1;
    }


    int FindIconInIconList(int code)
    {
        for (int i = 0; i < loadedData.iconList.Count; i++)
        {
            if (loadedData.iconList[i].code == code)
            {
                return i;
            }
        }
        return -1;
    }
    public void SaveData()  //  保存到json文件
    {
        TransferObjtoData();
        string dataAsJson = JsonUtility.ToJson(loadedData);
        File.WriteAllText(fileName, dataAsJson);
        Debug.Log("saved");
        successMes.SetActive(true);
        timecnt = Time.time;
        shown = true;
    }
}
