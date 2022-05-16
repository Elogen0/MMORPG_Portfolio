using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StageSelectCameraMovement : MonoBehaviour
{
    private Vector3 dragOrigin;
    [SerializeField] private float zoomstep, minCamSize, maxCamsize;
    
    private Camera cam;
    private CinemachineVirtualCamera vCam;

    [SerializeField] SpriteRenderer mapRenderer;
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;
    void Start()
    {
        cam = Camera.main;
        vCam = GetComponent<CinemachineVirtualCamera>();
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;
        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }
    
    void Update()
    {
        PanCamera();
        ZoomCamera();
    }

    private void ZoomCamera()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0f)
        {
            ZoomOut();
        }
        else if (wheel < 0f)
        {
            ZoomIn();
        }
        vCam.transform.position = ClampCamera(vCam.transform.position);
    }

    void PanCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = dragOrigin - cam.ScreenToViewportPoint(Input.mousePosition);
            vCam.transform.position = ClampCamera(vCam.transform.position + delta);
        }
    }


    public void ZoomIn()
    {
        float newSize = vCam.m_Lens.OrthographicSize + zoomstep;
        vCam.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamsize);
    }
    
    public void ZoomOut()
    {
        float newSize = vCam.m_Lens.OrthographicSize - zoomstep;
        vCam.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamsize);
    }

    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camH = vCam.m_Lens.OrthographicSize;
        float camW = vCam.m_Lens.OrthographicSize * cam.aspect;

        float minX = mapMinX + camW;
        float maxX = mapMaxX - camW;
        float minY = mapMinY + camH;
        float maxY = mapMaxY - camH;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
