using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpicaSDK.Services.Models
{
    public class Bucket
    {
        [JsonConstructor]
        public Bucket(
            [JsonProperty("_id")] string id,
            [JsonProperty("title")] string title,
            [JsonProperty("description")] string description,
            [JsonProperty("icon")] string icon,
            [JsonProperty("primary")] string primary,
            [JsonProperty("readOnly")] bool readOnly,
            [JsonProperty("history")] bool history,
            [JsonProperty("properties")] JObject properties,
            [JsonProperty("order")] int order
        )
        {
            Id = id;
            Title = title;
            Description = description;
            Icon = icon;
            Primary = primary;
            ReadOnly = readOnly;
            History = history;
            Properties = properties;
            Order = order;
        }

        [JsonProperty("_id")] public string Id { get; }

        [JsonProperty("title")] public string Title { get; }

        [JsonProperty("description")] public string Description { get; }

        [JsonProperty("icon")] public string Icon { get; }

        [JsonProperty("primary")] public string Primary { get; }

        [JsonProperty("readOnly")] public bool ReadOnly { get; }

        [JsonProperty("history")] public bool History { get; }

        [JsonProperty("order")] public int Order { get; }

        [JsonProperty("properties")] public JObject Properties { get; }
    }
}