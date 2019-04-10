using MongoDB.Bson;
using MongoDB.Driver;
using System;

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
            result.Remove("_id");
            return result;
        }
    }
}

