using Kame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    CharacterController controller;
    public float speed = 3;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float horz = Input.GetAxis("Horizontal");

        if (!Mathf.Approximately(Mathf.Abs(vert) + Mathf.Abs(horz), 0))
        {
            controller.SimpleMove(new Vector3(horz, 0, vert) * speed);
        }

    }
}
