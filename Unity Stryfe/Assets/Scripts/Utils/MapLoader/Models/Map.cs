using Assets.Scripts.Controllers.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MapLoader.Models
{
    public class Map
    {
        public string Name { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Dictionary<int, MapTexture> Textures { get; set; }
        public List<Tile> Tiles { get; set; }
        
        public Dictionary<Vector2, PathNode> Nodes { get; set; }

        public void CalculateUVs()
        {
            foreach (MapTexture texture in Textures.Values)
                texture.CalculateUVs();
        }

        public bool WillCollide(Vector2 pos)
        {
            return !GetTile(pos).Passable;
        }

        public float GetTileHeight(Vector2 pos)
        {
            return GetTile(pos).GetAverageHeight();
        }

        public Tile GetTile(Vector2 pos)
        {
            return Tiles[(int)pos.y * Width + (int)pos.x];
        }

        public void PopulatePathData()
        {
            Nodes = new Dictionary<Vector2, PathNode>();

            // Adds all the nodes
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    Nodes[pos] = new PathNode((int)pos.x, (int)pos.y, GetTile(pos).Passable);
                }
                    

            // Sets possible positions for adjacent tiles
            Dictionary<Vector2, int> adjPositions = new Dictionary<Vector2, int>();
            adjPositions.Add(new Vector2(0, 1), 10); // up
            adjPositions.Add(new Vector2(0, -1), 10); // down
            adjPositions.Add(new Vector2(-1, 0), 10); // left
            adjPositions.Add(new Vector2(1, 0), 10); // right
            adjPositions.Add(new Vector2(-1, 1), 14); // top left
            adjPositions.Add(new Vector2(1, 1), 14); // top right
            adjPositions.Add(new Vector2(-1, -1), 14); // bottom left
            adjPositions.Add(new Vector2(1, -1), 14); // bottom right

            // Adds the adjacents for each node
            foreach (PathNode node in Nodes.Values)
                foreach (KeyValuePair<Vector2, int> adjPair in adjPositions)
                    if (Nodes.ContainsKey(node.Position + adjPair.Key))
                        node.Adjacents.Add(Nodes[node.Position + adjPair.Key], adjPair.Value);
        }

        public void ResetPathData()
        {
            foreach (PathNode node in Nodes.Values)
            {
                node.Parent = null;
                node.FCost = 0;
            }
        }

        public Vector3 GetWorldPosition(Vector2 pos)
        {
            float height = GetTileHeight(pos) * MapManager.HeightStage;
            return new Vector3(pos.x + 0.5f, height + 0.5f, pos.y + 0.5f);
        }
    }
}
