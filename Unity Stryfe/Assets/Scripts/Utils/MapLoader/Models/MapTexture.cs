using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MapLoader.Models
{
    public class MapTexture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<int, TileUV> UVs { get; set; }

        public void CalculateUVs()
        {
            UVs = new Dictionary<int, TileUV>();

            float tilesWide = Width / MapManager.TileSize;
            float tilesHigh = Height / MapManager.TileSize;

            float offsetX = 1 / tilesWide;
            float offsetY = 1 / tilesHigh;

            int index = 0;
            for (int y = 0; y < tilesHigh; y++)
            {
                for (int x = 0; x < tilesWide; x++)
                {
                    UVs[index++] = new TileUV(x, y, offsetX, offsetY);
                }
            }
        }
    }
}
