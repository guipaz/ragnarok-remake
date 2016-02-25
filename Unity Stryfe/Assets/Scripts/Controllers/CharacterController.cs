using UnityEngine;
using System.Collections;
using MapLoader.Models;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Pathfinding;
using System.Collections.Generic;
using MapLoader;

public enum FacingDirection
{
    Bottom = 0,
    BottomLeft,
    Left,
    UpperLeft,
    Up,
    UpperRight,
    Right,
    BottomRight
}

public class CharacterController : MonoBehaviour {
    
    public GameObject selectionTile;

    // Dragging control
    private float clickCooldown = 0.1f;
    private float clickCooldownLeft = 0;

    // Pathfinding
    private bool isWalking = false;
    private List<Vector2> steps;
    private Vector3 previousPosition;
    private Vector3 nextDestination;

    // Animation control
    private float timeStartedLerp;
    private float timeTakenDuringLerp = 1 / 4.5f;
    private Animator animator;

    /*
        Directions (Clockwise)
        0 - Down
        1 - Bottom Left
        2 - Left
        3 - Upper Left
        4 - Up
        5 - Upper Right
        6 - Right
        7 - Bottom Right
    */
    private int direction; // this is gonna be a bitch
    

    // Animator parameters (better const this shit out)
    private const string WalkingParam = "Walking";
    private const string DirectionParam = "Direction";

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update () {
        transform.LookAt(Camera.main.transform);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool didHitMap = Physics.Raycast(ray, out hit);

        Vector3 mousePosition = Vector3.zero;
        Vector2 targetPosition = Vector2.zero;
        
        // Adjusts the tile position and the selection tile
        if (didHitMap)
        {
            targetPosition = new Vector2((int)hit.point.x, (int)hit.point.z);

            if (MapManager.CurrentMap.WillCollide(targetPosition))
            {
                didHitMap = false;
            } else
            {
                mousePosition = MapManager.CurrentMap.GetWorldPosition(targetPosition);

                selectionTile.transform.position = new Vector3(mousePosition.x, 0.01f, mousePosition.z);
                selectionTile.GetComponent<MeshFilter>().mesh = MapManager.CurrentMap.GetTile(targetPosition).GetSelectionMesh(selectionTile.GetComponent<MeshFilter>().mesh);
            }
        }

        // If is walking, checks the cooldown for the next step
        if (isWalking)
        {
            float timeSinceStarted = Time.time - timeStartedLerp;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            if (percentageComplete >= 1)
            {
                if (steps.Count > 0)
                {
                    previousPosition = transform.position;
                    nextDestination = MapManager.CurrentMap.GetWorldPosition(steps[0]);

                    // Sets the direction for the animator
                    direction = (int)GetDirection(new Vector2((int)previousPosition.x, (int)previousPosition.z),
                                                  steps[0]);

                    steps.RemoveAt(0);

                    timeStartedLerp = Time.time;
                }
                else
                {
                    isWalking = false;
                    nextDestination = transform.position;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(previousPosition, nextDestination, percentageComplete);
            }
        }

        // If clicks, resets the path
        if (clickCooldownLeft <= 0)
        {
            if (Input.GetMouseButton(0) && didHitMap)
            {
                Vector2 currentPosition = new Vector2((int)transform.position.x, (int)transform.position.z);
                if (isWalking)
                    currentPosition = new Vector2((int)nextDestination.x, (int)nextDestination.z);

                PathNode node = PathfindingController.Instance.GetMovement(currentPosition, targetPosition);
                steps = node.GetSteps();
                isWalking = true;

                clickCooldownLeft = clickCooldown;
            }
        } else
        {
            clickCooldownLeft -= Time.deltaTime;
        }

        //TODO: encapsulate this
        animator.SetBool(WalkingParam, isWalking);
        animator.SetInteger(DirectionParam, direction);
    }

    // Gotta do some magic here, this is gonna be tricky regarding the camera
    // I will ignore camera for now since it'll be hell
    //
    // If I do some shit, just say it in the chat and I'll be happy to fix it haha
    //
    private FacingDirection GetDirection(Vector2 curPos, Vector2 nextPos)
    {
        FacingDirection dir = 0;

        Vector2 diff = nextPos - curPos;

        int x = (int)diff.x;
        int y = (int)diff.y;

        Debug.Log(curPos);
        Debug.Log(nextPos);
        Debug.Log(string.Format("X: {0} Y: {1}", x, y));

        if (x == 1)
            if (y == 1)
                dir = FacingDirection.UpperRight; // Upper right
            else if (y == -1)
                dir = FacingDirection.BottomRight; // Bottom right
            else
                dir = FacingDirection.Right; // Right
        else if (x == -1)
            if (y == 1)
                dir = FacingDirection.UpperLeft; // Upper left
            else if (y == -1)
                dir = FacingDirection.BottomLeft; // Bottom left
            else
                dir = FacingDirection.Left; // Left
        else
            if (y == 1)
                dir = FacingDirection.Up; // Up
            else if (y == -1)
                dir = FacingDirection.Bottom; // Bottom
            else
                dir = (FacingDirection)direction;

        Debug.Log(dir);

        return dir;
    }
}
