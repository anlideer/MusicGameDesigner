using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditIcon : MonoBehaviour
{
    public bool type;   //  false - 点击式, true - 长按式
    public int track;   //  轨道编号
    public float pos;
    public GameObject lastPoint = null;
    public GameObject nextPoint = null;

    private bool isMoving = false;
    private Vector3 originalV;
    private Transform originalP;
    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 跟随鼠标
        if (isMoving)
        {
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vec.z = 0;
            transform.position = vec;
        }

        if (gameObject.GetComponent<LineRenderer>())
        {
            LineRenderer line = gameObject.GetComponent<LineRenderer>();
            if (nextPoint == null)
            {
                Destroy(line);
            }
            else
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, nextPoint.transform.position);
            }
        }
        else if (nextPoint != null)
        {
            LineRenderer line = gameObject.AddComponent<LineRenderer>();
            line.sortingLayerName = "Icon";
            //只有设置了材质 setColor才有作用
            Shader s = Resources.Load("lineShader") as Shader;
            line.material = new Material(s);
            line.positionCount = 2;//设置两点
            line.startColor = line.endColor = Color.green; //设置直线颜色
            line.startWidth = line.endWidth = 0.1f;//设置直线宽度

            //设置指示线的起点和终点
            line.SetPosition(0, transform.position);
            line.SetPosition(1, nextPoint.transform.position);
        }
    }

    public void OnMouseDown()
    {
        //Debug.Log("down!");
        // move
        if (GM.isLineMode == false || type == false)
        {
            originalV = transform.position;
            originalP = transform.parent;
            transform.parent = null;
            isMoving = true;
        }
        else
        {
            // 连线呢
            
        }

    }

    public void OnMouseUp()
    {
        if (isMoving)    //  说明不是连线模式
        {
            //Debug.Log("up!");
            isMoving = false;
            bool flag = false;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, 1 << LayerMask.NameToLayer("Track"));
            if (hit)
            {
                if (hit.collider.gameObject.tag == "metre")
                {
                    flag = true;
                    GameObject metreObj = hit.collider.gameObject;
                    Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    v.z = 0;
                    Transform left = metreObj.transform.Find("Left");
                    Transform right = metreObj.transform.Find("Right");
                    Transform middle = metreObj.transform.Find("Middle");
                    v.y = left.position.y;
                    float relatedPos = Vector3.Distance(left.position, v) / Vector3.Distance(left.position, right.position);
                    track = metreObj.GetComponent<Metre>().trackNo;
                    // 手动模式
                    if (GM.isAutoMode == false)
                    {
                        pos = relatedPos + metreObj.transform.parent.GetComponent<SixMetre>().metreNum;
                        transform.position = v;
                    }
                    // 自动模式
                    else
                    {
                        Vector3 resPos = left.position;
                        float dis = Vector3.Distance(left.position, v);
                        float choice = 0f;
                        if (Vector3.Distance(middle.position, v) < dis)
                        {
                            dis = Vector3.Distance(middle.position, v);
                            resPos = middle.position;
                            choice = 0.5f;
                        }
                        if (Vector3.Distance(right.position, v) < dis)
                        {
                            dis = Vector3.Distance(right.position, v);
                            resPos = right.position;
                            choice = 1f;
                        }

                        pos = choice + metreObj.transform.parent.GetComponent<SixMetre>().metreNum;
                        transform.position = resPos;

                    }

                    transform.parent = metreObj.transform;
                }
                else if (hit.collider.gameObject.tag == "trash")
                {
                    flag = true;
                    Destroy(gameObject);
                }
            }
            // 回归原来的状态
            if (flag == false)
            {
                transform.position = originalV;
                transform.parent = originalP;
            }
        }
        else // 说明是连线模式
        {
            // 还是看看鼠标有没有搞到可以连线的东西
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, 1 << LayerMask.NameToLayer("Icon"));

            if (hit && hit.collider.gameObject != gameObject && hit.collider.gameObject.tag == "lastIcon")
            {
                GameObject hitObj = hit.collider.gameObject;
                if (pos <= hitObj.GetComponent<EditIcon>().pos)
                {
                    nextPoint = hitObj;
                    hitObj.GetComponent<EditIcon>().lastPoint = gameObject;
                    LineRenderer line;
                    //画线
                    line = gameObject.AddComponent<LineRenderer>();
                    line.sortingLayerName = "Icon";
                    //只有设置了材质 setColor才有作用
                    Shader s = Resources.Load("lineShader") as Shader;
                    line.material = new Material(s);
                    line.positionCount = 2;//设置两点
                    line.startColor = line.endColor = Color.green; //设置直线颜色
                    line.startWidth = line.endWidth = 0.1f;//设置直线宽度

                    //设置指示线的起点和终点
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, hitObj.transform.position);
                }
                else
                {
                    lastPoint = hitObj;
                    hitObj.GetComponent<EditIcon>().nextPoint = gameObject;
                    LineRenderer line;
                    //画线
                    line = hitObj.AddComponent<LineRenderer>();
                    line.sortingLayerName = "Icon";
                    //只有设置了材质 setColor才有作用
                    Shader s = Resources.Load("lineShader") as Shader;
                    line.material = new Material(s);
                    line.positionCount = 2;//设置两点
                    line.startColor = line.endColor = Color.green; //设置直线颜色
                    line.startWidth = line.endWidth = 0.1f;//设置直线宽度

                    //设置指示线的起点和终点
                    line.SetPosition(0, hitObj.transform.position);
                    line.SetPosition(1, transform.position);
                }
            }
        }
    }
}
