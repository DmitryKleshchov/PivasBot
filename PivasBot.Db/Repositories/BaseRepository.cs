using MongoDB.Bson;
using MongoDB.Driver;
using System;
using MongoDB.Bson.IO;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace PivasBot.Db.Repositories
{
    public abstract class BaseRepository
    {
        private MongoClient _dbClient;
        protected IMongoDatabase _db;
        protected IMongoCollection<BsonDocument> _collection;

        protected BaseRepository(DbConnection dbConn, string dbName, string collectionName)
        {
            _dbClient = dbConn?.DbClient ?? throw new ArgumentNullException(nameof(dbConn));
            _db = _dbClient.GetDatabase(dbName);
            _collection = _db.GetCollection<BsonDocument>(collectionName);
        }

        public void InsertOne<T>(T obj)
        {
            InsertOne(JsonConvert.SerializeObject(obj));
        }

        public void InsertOne(string json)
        {
            BsonDocument doc = BsonDocument.Parse(json);
            _collection.InsertOne(doc);
        }

        public BsonDocument GetOneRandom()
        {
            var stage = new BsonDocument { { "$sample", new BsonDocument { { "size", 1 } } } };

            var aggregate = _collection.Aggregate()
                .AppendStage<BsonDocument>(stage);

            BsonDocument result =  aggregate.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// returns a random document with a specific match requirement
        /// </summary>
        /// <param name="matchJson"></param>
        /// <returns></returns>
        public BsonDocument GetOneRandom(string matchJson)
        {
            var matchStage = new BsonDocument { {"$match" , BsonDocument.Parse(matchJson) }};
            var sampleStage = new BsonDocument { { "$sample", new BsonDocument { { "size", 1 } } } };

            var aggregate = _collection.Aggregate()
                .AppendStage<BsonDocument>(matchStage)
                .AppendStage<BsonDocument>(sampleStage);

            BsonDocument result = aggregate.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Updates one document in a collection
        /// </summary>
        /// <param name="id">_id param of an object</param>
        /// <param name="setJson">json string with set params, without the {$set:} part.</param>
        public void UpdateOne(string id, string setJson)
        {
            _collection.UpdateOne(new BsonDocument {{"id", $"ObjectId({id})"}}
                , new BsonDocument {{"$set", BsonDocument.Parse(setJson)}});
        }
    }
}

