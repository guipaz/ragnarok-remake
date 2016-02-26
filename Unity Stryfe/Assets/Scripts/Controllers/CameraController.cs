using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform orbitingObject;

    CharacterController playerController;
    Vector3 lastCameraPos = Vector3.zero;
    Vector3 lastObjectPos = Vector3.zero;

    int maxZoom = 5;
    int minZoom = 10;
    
    public void Awake()
    {
        playerController = orbitingObject.GetComponent<CharacterController>();
    }
    
    public void Start()
    {
        lastCameraPos = transform.position;
        lastObjectPos = orbitingObject.position;
    }

    public void Update()
    {
        transform.position += orbitingObject.position - lastObjectPos;
        lastObjectPos = orbitingObject.position;

        if (Input.GetMouseButtonDown(1))
            lastCameraPos = Input.mousePosition;

        // Orbiting
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(orbitingObject.position, Vector3.up, Input.mousePosition.x - lastCameraPos.x);
            lastCameraPos = Input.mousePosition;

            playerController.UpdateAnimator();
        }

        // Scroll
        float distance = Vector3.Distance(transform.position, orbitingObject.position);
        float scroll = Input.mouseScrollDelta.y;
        if ((distance > maxZoom || scroll > 0) && (distance < minZoom || scroll < 0))
            transform.position = Vector3.MoveTowards(transform.position, orbitingObject.position, Input.mouseScrollDelta.y * -2);
    }

    public FacingDirection GetRotatedDirection(FacingDirection dir)
    {
        // Applies camera rotation
        float rotation = Camera.main.transform.rotation.eulerAngles.y;
        int mod = (int)(rotation / 40);
        if (rotation > 180)
            mod = (int)((360 - rotation) / 40) * -1;
        
        int modded = ((int)dir - mod) % 8;
        if (modded < 0)
            modded = 8 - mod;

        return (FacingDirection)modded;
    }
}
