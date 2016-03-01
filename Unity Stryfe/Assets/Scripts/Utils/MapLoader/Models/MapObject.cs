using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapLoader.Models
{
    public class MapObject
    {
        [JsonProperty(PropertyName = "pid")]
        public int PrefabId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        [JsonProperty(PropertyName = "r")]
        public float Rotation { get; set; }
    }
}
