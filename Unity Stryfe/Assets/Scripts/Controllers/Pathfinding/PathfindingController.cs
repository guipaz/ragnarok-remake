using Assets.Scripts.Controllers.Pathfinding;
using MapLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PathfindingController
    {
        List<PathNode> Opened;
        List<PathNode> Closed;

        public PathNode GetMovement(Vector2 start, Vector2 end)
        {
            MapManager.CurrentMap.ResetPathData();

            PathNode current = null;

            Opened = new List<PathNode>();
            Closed = new List<PathNode>();

            Opened.Add(MapManager.CurrentMap.Nodes[start]);

            while (Opened.Count > 0)
            {
                current = GetLowestFCost();
                Opened.Remove(current);
                Closed.Add(current);

                if (current.Position == end)
                    return current;

                foreach (KeyValuePair<PathNode, int> adjPair in current.Adjacents)
                {
                    PathNode adj = adjPair.Key;
                    int cost = adjPair.Value;

                    if (!adj.Passable || Closed.Contains(adj))
                        continue;

                    if (current.FCost + cost < adj.FCost || !Opened.Contains(adj))
                    {
                        adj.FCost = current.FCost + cost;
                        adj.Parent = current;
                        if (!Opened.Contains(adj))
                            Opened.Add(adj);
                    }
                }
            }
            
            return current;
        }

        private PathNode GetLowestFCost()
        {
            PathNode lowest = null;
            foreach (PathNode node in Opened)
                if (lowest == null || node.FCost <= lowest.FCost)
                    lowest = node;
            return lowest;
        }
        
        // Singleton stuff
        private static PathfindingController instance;
        protected PathfindingController() { }

        public static PathfindingController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PathfindingController();
                }
                return instance;
            }
        }
    }
}
