using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpicaSDK.Services.Models
{
    [JsonConverter(typeof(LocationConverter))]
    public readonly struct Point
    {
        public readonly float Latitude;
        public readonly float Longitude;

        public Point(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        [JsonConstructor]
        public Point(float[] coordinates)
        {
            Longitude = coordinates[0];
            Latitude = coordinates[1];
        }
    }

    public class LocationConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value != null)
            {
                Point p = (Point)value;
                JObject pObj = new JObject();
                pObj.Add("type", nameof(Point));
                pObj.Add("coordinates", JToken.FromObject(new float[] { p.Longitude, p.Latitude }));
                writer.WriteValue(pObj);
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);

            return new Point(jObj["coordinates"].ToObject<float[]>());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Point);
        }
    }
}