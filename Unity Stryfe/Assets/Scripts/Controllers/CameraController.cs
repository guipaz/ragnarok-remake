using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform orbitingObject;

    Vector3 lastPos = Vector3.zero;

    int maxZoom = 5;
    int minZoom = 10;

    public void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
            lastPos = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(orbitingObject.position, Vector3.up, Input.mousePosition.x - lastPos.x);
            lastPos = Input.mousePosition;
        }

        float distance = Vector3.Distance(transform.position, orbitingObject.position);
        float scroll = Input.mouseScrollDelta.y;
        if ((distance > maxZoom || scroll < 0) && (distance < minZoom || scroll > 0))
            transform.position = Vector3.MoveTowards(transform.position, orbitingObject.position, Input.mouseScrollDelta.y * 2);
    }
}
