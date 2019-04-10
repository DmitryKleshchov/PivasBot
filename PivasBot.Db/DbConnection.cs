using MongoDB.Driver;

namespace PivasBot.Db
{
    public class DbConnection
    {
        internal MongoClient DbClient;

        public DbConnection(string connectionString = null)
        {
            DbClient = new MongoClient(
                string.IsNullOrEmpty(connectionString) ? "mongodb://localhost:27017" : connectionString);
        }
    }
}
