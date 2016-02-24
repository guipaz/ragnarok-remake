using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MapLoader.Models
{
    public class Tile
    {
        public static Vector3[] DefaultSelectionVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0)
        };

        [JsonProperty(PropertyName = "tid")]
        public int TextureIdentifier { get; set; }

        [JsonProperty(PropertyName = "uvid")]
        public int TileUVIdentifier { get; set; }

        [JsonProperty(PropertyName = "p")]
        public bool Passable { get; set; }

        // Heights
        [JsonProperty(PropertyName = "st")] // Stage (full tile)
        public float Stage { get; set; }

        // Each vertex
        [JsonProperty(PropertyName = "tl")]
        public float TopLeft { get; set; }
        [JsonProperty(PropertyName = "tr")]
        public float TopRight { get; set; }
        [JsonProperty(PropertyName = "bl")]
        public float BottomLeft { get; set; }
        [JsonProperty(PropertyName = "br")]
        public float BottomRight { get; set; }

        // Each edge
        [JsonProperty(PropertyName = "t")]
        public float Top { get; set; }
        [JsonProperty(PropertyName = "b")]
        public float Bottom { get; set; }
        [JsonProperty(PropertyName = "l")]
        public float Left { get; set; }
        [JsonProperty(PropertyName = "r")]
        public float Right { get; set; }

        public Tile()
        {
            Passable = true;
        }

        public float GetAverageHeight()
        {
            Vector3 topLeft = new Vector3(0, Top + TopLeft + Left, 0);
            Vector3 topRight = new Vector3(1, Top + TopRight + Right, 0);
            Vector3 bottomLeft = new Vector3(0, Bottom + BottomLeft + Left, 1);
            Vector3 bottomRight = new Vector3(1, Bottom + BottomRight + Right, 1);

            Vector3 centroid = topLeft + topRight + bottomLeft + bottomRight;
            return centroid.y / 4 + Stage;
        }

        public Mesh GetSelectionMesh(Mesh mesh)
        {
            if (GetAverageHeight() == 0)
            {
                mesh.vertices = DefaultSelectionVertices;
                return mesh;
            }
            
            float topLeft = TopLeft + Top + Stage;
            float topRight = TopRight + Top + Stage;
            float bottomLeft = BottomLeft + Bottom + Stage;
            float bottomRight = BottomRight + Bottom + Stage;

            Vector3[] nV = new Vector3[4];
            nV[0] = new Vector3(-0.5f, -0.5f, -1 * topRight * MapManager.HeightStage);
            nV[1] = new Vector3(0.5f, 0.5f, -1 * bottomRight * MapManager.HeightStage);
            nV[2] = new Vector3(0.5f, -0.5f, -1 * topLeft * MapManager.HeightStage);
            nV[3] = new Vector3(-0.5f, 0.5f, -1 * bottomLeft * MapManager.HeightStage);
            mesh.vertices = nV;

            return mesh;
        }
    }
}
