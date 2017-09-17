using System;
using Newtonsoft.Json;

namespace SP_Provider.Entities
{
    public class ListItem
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "Created")]
        public DateTime Created { get; set; }
    }
}
