using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraMove : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (!Mathf.Approximately(x, 0) || !Mathf.Approximately(y, 0))
        {
            transform.position += new Vector3(x, y, 0) * Time.deltaTime * 3;
        }
    }
}
