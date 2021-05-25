using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View3ZoomCamera : MonoBehaviour
{
    float speed = 10.0f;
    Camera worldCamera;
    float maxScreenWidth = 35f;
    float minScreenWidth = -5f;
    float maxScreenHeight = 65f;
    float minScreenHeight = -1f;

    // Use this for initialization
    void Start()
    {
        worldCamera = GameObject.FindGameObjectWithTag("View3WorldCamera").GetComponent < Camera > ();
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                ChangeCameraPosition();
            }
        }
    }

    private void ChangeCameraPosition()
    {
        Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Vector3 location = new Vector3(hit.point.x, hit.point.y+4, 7f);
            transform.position = location;
        }
    }
}
