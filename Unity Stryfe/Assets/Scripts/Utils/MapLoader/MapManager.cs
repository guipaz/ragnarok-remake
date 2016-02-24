using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using MapLoader.Models;
using System.Collections.Generic;
using Assets.Scripts.Controllers;

namespace MapLoader
{
    public class MapManager : MonoBehaviour
    {
        public static Map CurrentMap { get; set; }

        public static int TileSize = 64;
        public static float HeightStage = 0.1f;
        
        public void Start()
        {
            BuildMap();
        }

        public void BuildMap()
        {
            // Loads the map from the JSON
            CurrentMap = JsonConvert.DeserializeObject<Map>(File.ReadAllText("Assets/Resources/Maps/neo_prontera_2.json"));
            CurrentMap.CalculateUVs();
            CurrentMap.PopulatePathData();

            // Builds the mesh
            BuildMesh(CurrentMap);
        }

        //TEST
        private void BuildMesh(Map map)
        {
            GameObject obj = new GameObject(map.Name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

            int width = map.Width;
            int height = map.Height;

            Mesh mesh = new Mesh();

            Dictionary<int, int> tilesBySubmesh = new Dictionary<int, int>();
            Dictionary<int, int[]> trianglesBySubmesh = new Dictionary<int, int[]>();
            Dictionary<int, int> controlBySubmesh = new Dictionary<int, int>();
            Dictionary<int, int> indexBySubmesh = new Dictionary<int, int>();

            // Define 0-index based ids for each texture id
            int index = 0;
            foreach (int textureId in map.Textures.Keys)
                indexBySubmesh[textureId] = index++;
            
            // Gets the number of tiles in each submesh
            foreach (Tile tile in map.Tiles)
            {
                if (tilesBySubmesh.ContainsKey(tile.TextureIdentifier))
                    tilesBySubmesh[tile.TextureIdentifier]++;
                else
                    tilesBySubmesh[tile.TextureIdentifier] = 1;
            }
            
            // Creates each triangle array based on the number of tiles
            foreach (KeyValuePair<int, int> submesh in tilesBySubmesh)
            {
                trianglesBySubmesh[submesh.Key] = new int[submesh.Value * 3 * 2];

                Debug.Log(string.Format("Texture {0}: {1} triangles", submesh.Key, trianglesBySubmesh[submesh.Key].Length));

                // Defines each index for triangle creation control
                controlBySubmesh[submesh.Key] = 0;
            }

            // Defines the variables
            int vertNumber = width * height * 4;
            Vector3[] vertices = new Vector3[vertNumber];
            Vector2[] uvs = new Vector2[vertNumber];
            Vector3[] normals = new Vector3[vertNumber];

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                Tile tile = map.Tiles[i];

                int x = i % map.Width;
                int z = i / map.Width;

                int topLeft = z * 4 * width + x * 2;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + width * 2;
                int bottomRight = bottomLeft + 1;

                vertices[topLeft] = new Vector3(x, HeightStage * (tile.Top + tile.Left + tile.TopLeft + tile.Stage), z);
                vertices[topRight] = new Vector3(x + 1, HeightStage * (tile.Top + tile.Right + tile.TopRight + tile.Stage), z);
                vertices[bottomLeft] = new Vector3(x, HeightStage * (tile.Bottom + tile.Left + tile.BottomLeft + tile.Stage), z + 1);
                vertices[bottomRight] = new Vector3(x + 1, HeightStage * (tile.Bottom + tile.Right + tile.BottomRight + tile.Stage), z + 1);

                TileUV tileUVs = map.Textures[tile.TextureIdentifier].UVs[tile.TileUVIdentifier];
                uvs[topLeft] = tileUVs.UV1;
                uvs[topRight] = tileUVs.UV2;
                uvs[bottomLeft] = tileUVs.UV3;
                uvs[bottomRight] = tileUVs.UV4;

                normals[topLeft] = Vector3.up;
                normals[topRight] = Vector3.up;
                normals[bottomLeft] = Vector3.up;
                normals[bottomRight] = Vector3.up;

                // Gets the right triangle array and control index according to the submesh
                int[] triangles = trianglesBySubmesh[tile.TextureIdentifier];
                int triangleIndex = controlBySubmesh[tile.TextureIdentifier];

                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomRight;
                triangles[triangleIndex++] = topRight;

                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;

                controlBySubmesh[tile.TextureIdentifier] = triangleIndex;
            }

            // Defines the variables in the mesh
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;

            // Defines the triangles and materials for each submesh
            mesh.subMeshCount = map.Textures.Count;
            Material[] materials = new Material[mesh.subMeshCount];
            foreach (KeyValuePair<int, int[]> triangles in trianglesBySubmesh)
            {
                mesh.SetTriangles(triangles.Value, indexBySubmesh[triangles.Key]);
                materials[indexBySubmesh[triangles.Key]] = GetTexturedMaterial(map.Textures[triangles.Key].Name);
            }

            obj.GetComponent<MeshRenderer>().materials = materials;
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
        }

        private Material GetTexturedMaterial(string textureName)
        {
            Material material = new Material(Resources.Load<Material>("Materials/DefaultMaterial"));
            material.mainTexture = Resources.Load<Texture>(string.Format("Textures/{0}", textureName));
            return material;
        }

        // Singleton stuff
        private static MapManager instance;
        protected MapManager() { }
        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapManager();
                }
                return instance;
            }
        }
    }
}
