using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    class IMDBMovie
    {
        [JsonProperty ("id")]
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string runtimeStr { get; set; }
        public string genres { get; set; }
        public string imDbRating { get; set; }
        public string plot { get; set; }
        public string stars { get; set; }
        public string contentRating { get; set; }

    }
}
