using UnityEngine;
using System.Collections;
using MapLoader.Models;
using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Pathfinding;

public class CharacterController : MonoBehaviour {
    
    public GameObject selectionTile;

    public bool isWalking = false;

	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool didHitMap = Physics.Raycast(ray, out hit);

        Vector3 mousePosition = Vector3.zero;
        Vector2 targetPosition = Vector2.zero;
        
        // Adjusts the tile position and the selection tile
        if (didHitMap)
        {
            targetPosition = new Vector2((int)hit.point.x, (int)hit.point.z);

            if (MapLoader.MapManager.CurrentMap.WillCollide(targetPosition))
            {
                didHitMap = false;
            } else
            {
                float height = MapLoader.MapManager.CurrentMap.GetTileHeight(targetPosition) * MapLoader.MapManager.HeightStage;

                targetPosition = new Vector2((int)hit.point.x, (int)hit.point.z);
                mousePosition = new Vector3(targetPosition.x + 0.5f, height + 0.5f, targetPosition.y + 0.5f);

                selectionTile.transform.position = new Vector3(mousePosition.x, 0.01f, mousePosition.z);
                selectionTile.GetComponent<MeshFilter>().mesh = MapLoader.MapManager.CurrentMap.GetTile(targetPosition).GetSelectionMesh(selectionTile.GetComponent<MeshFilter>().mesh);
            }
        }
        
        if (Input.GetMouseButtonUp(0) && didHitMap)
        {
            PathNode node = PathfindingController.Instance.GetMovement(new Vector2((int)oldPosition.x, (int)oldPosition.z), targetPosition);
            
        }
    }
}
