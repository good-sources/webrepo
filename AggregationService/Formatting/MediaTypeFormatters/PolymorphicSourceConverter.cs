namespace AggregationService.Formatting
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using AggregationService.Domain.Models;

    public class PolymorphicSourceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Source);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Source source;

            JObject obj = JObject.Load(reader);

            var uri = obj["Uri"];
            var type = obj["Type"];
            var collection = obj["CollectionId"];

            if (uri == null)
                throw new ArgumentException("Missing URI", "Uri");
            if (type == null)
                throw new ArgumentException("Missing type", "Type");
            if (collection == null)
                throw new ArgumentException("Missing collection", "CollectionId");

            int typeValue = type.Value<int>();

            switch ((SourceType)typeValue)
            {
                case SourceType.RSS:
                    source = new RssFeed()
                    {
                        Uri = uri.Value<string>(),
                        CollectionId = collection.Value<int>()
                    };
                    break;
                default:
                    throw new NotSupportedException("Unknown source type: " + typeValue);
            }

            serializer.Populate(obj.CreateReader(), source);

            return source;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}