using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsIcon : MonoBehaviour
{
    public GameObject clickIconPrefab;
    public GameObject lastIconPrefab;
    private Transform left, right, middle;
    private float iconLen;
    private int metreNum;

    bool notme = false;
    RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        left = transform.Find("Left");
        right = transform.Find("Right");
        middle = transform.Find("Middle");
        iconLen = Vector3.Distance(left.position, right.position);
        metreNum = transform.parent.GetComponent<SixMetre>().metreNum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, 1 << LayerMask.NameToLayer("Icon"));
        if (hit)
        {
            // Debug.Log("had icon here");
            if (hit.collider.gameObject.tag == "clickIcon" || hit.collider.gameObject.tag == "lastIcon")
            {
                notme = true;
                hit.collider.gameObject.GetComponent<EditIcon>().OnMouseDown();
            }
        }
        else
        {
            notme = false;
            PlaceIcon();
        }
    }

    void OnMouseUp()
    {
        if (notme)
        {
            notme = false;
            hit.collider.gameObject.GetComponent<EditIcon>().OnMouseUp();
        }
    }

    void PlaceIcon()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.y = left.position.y;
        vec.z = 0;
        float pos = Vector3.Distance(left.position, vec) / iconLen;
        // 手动模式
        if (GM.isAutoMode == false)
        {
            if (GM.mode == false)   // 点击式
            {
                GameObject tmpobj = Instantiate(clickIconPrefab, vec, transform.rotation);
                tmpobj.transform.parent = transform;
                tmpobj.GetComponent<EditIcon>().type = false;
                tmpobj.GetComponent<EditIcon>().track = GetComponent<Metre>().trackNo;
                tmpobj.GetComponent<EditIcon>().pos = pos + metreNum;
            }
            else    // 长按式
            {
                GameObject tmpobj = Instantiate(lastIconPrefab, vec, transform.rotation);
                tmpobj.transform.parent = transform;
                tmpobj.GetComponent<EditIcon>().type = true;
                tmpobj.GetComponent<EditIcon>().track = GetComponent<Metre>().trackNo;
                tmpobj.GetComponent<EditIcon>().pos = pos + metreNum;
            }

        }
        // 自动模式
        else
        {
            Vector3 resPos = left.position;
            float dis = Vector3.Distance(left.position, vec);
            float choice = 0f;
            if (Vector3.Distance(middle.position, vec) < dis)
            {
                dis = Vector3.Distance(middle.position, vec);
                resPos = middle.position;
                choice = 0.5f;
            }
            if (Vector3.Distance(right.position, vec) < dis)
            {
                dis = Vector3.Distance(right.position, vec);
                resPos = right.position;
                choice = 1f;
            }



            if (GM.mode == false)   //  点击式
            {
                GameObject tmpobj = Instantiate(clickIconPrefab, resPos, transform.rotation);
                tmpobj.transform.parent = transform;
                tmpobj.GetComponent<EditIcon>().type = false;
                tmpobj.GetComponent<EditIcon>().track = GetComponent<Metre>().trackNo;
                tmpobj.GetComponent<EditIcon>().pos = choice + metreNum;
            }
            else    // 长按式
            {
                GameObject tmpobj = Instantiate(lastIconPrefab, resPos, transform.rotation);
                tmpobj.transform.parent = transform;
                tmpobj.GetComponent<EditIcon>().type = true;
                tmpobj.GetComponent<EditIcon>().track = GetComponent<Metre>().trackNo;
                tmpobj.GetComponent<EditIcon>().pos = choice + metreNum;

            }
        }
    }
}
