using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers.Pathfinding
{
    public class PathNode
    {
        public Vector2 Position;
        public int FCost;
        public PathNode Parent;
        public Dictionary<PathNode, int> Adjacents;
        public bool Passable;

        public PathNode(int x, int y, bool passable)
        {
            Parent = null;
            FCost = 0;
            Position = new Vector2(x, y);
            Adjacents = new Dictionary<PathNode, int>();
            Passable = passable;
        }

        public List<Vector2> GetSteps()
        {
            List<Vector2> steps = new List<Vector2>();
            PathNode node = this;
            while (node != null)
            {
                steps.Add(node.Position);
                node = node.Parent;
            }
                
            steps.Reverse();
            steps.RemoveAt(0); // removes the first, where it's starting
            return steps;
        }
    }
}
