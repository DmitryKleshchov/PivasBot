using MongoDB.Bson;
using MongoDB.Bson.IO;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace PivasBot.Db.Extensions
{
    public static class BsonDocumentExtensions
    {
        public static T As<T>(this BsonDocument obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            string json = obj.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
