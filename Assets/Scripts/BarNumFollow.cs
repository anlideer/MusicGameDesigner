using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarNumFollow : MonoBehaviour
{
    public GameObject bars;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // follow the bars
        transform.position = Camera.main.WorldToScreenPoint(bars.transform.position);
    }
}
