using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MapLoader.Models
{
    public class TileUV
    {
        public Vector2 UV1 { get; set; }
        public Vector2 UV2 { get; set; }
        public Vector2 UV3 { get; set; }
        public Vector2 UV4 { get; set; }

        public TileUV(int x, int y, float offsetX, float offsetY)
        {
            UV1 = new Vector2(x * offsetX, y * offsetY);
            UV2 = new Vector2((x + 1) * offsetX, y * offsetY);
            UV3 = new Vector2(x * offsetX, (y + 1) * offsetY);
            UV4 = new Vector2((x + 1) * offsetX, (y + 1) * offsetY);
        }
    }
}
