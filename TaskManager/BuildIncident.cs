using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IncidentSending
{
    class BuildIncident
    {
        [JsonProperty("assignment_group")]
        public string Group { get; set; }

        [JsonProperty("caller_id")]
        public string Caller { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("cmdb_ci")]
        public string CI { get; set; }
        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }
        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }
        [JsonProperty("impact")]
        public int Impact { get; set; }
        [JsonProperty("urgency")]
        public int Urgency { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
