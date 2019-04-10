using Newtonsoft.Json;
using PivasBot.Db;
using PivasBot.Db.Repositories;

namespace PivasBot.Core.Services
{
    public class JokeService
    {
        private JokeRepository _jokeRepository;

        public JokeService(DbConnection dbConn)
        {
            _jokeRepository = new JokeRepository(dbConn);
        }

        public string GetJoke()
        {
            return _jokeRepository.GetOneRandom()?["joke"].AsString;
        }

        public void AddJoke(string jokeStr)
        {
            _jokeRepository.InsertOne(JsonConvert.SerializeObject(new { joke = jokeStr }));
        }
    }
}
