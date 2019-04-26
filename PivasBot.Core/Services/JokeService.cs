using Newtonsoft.Json;
using PivasBot.Core.Models;
using PivasBot.Db;
using PivasBot.Db.Repositories;
using SimpleFeedReader;
using System.Collections.Generic;
using System.Threading.Tasks;
using PivasBot.Db.Extensions;

namespace PivasBot.Core.Services
{
    public class JokeService
    {
        private JokeRepository _jokeRepository;

        public JokeService(DbConnection dbConn)
        {
            _jokeRepository = new JokeRepository(dbConn);
        }

        public Joke GetJoke()
        {
            Joke joke = _jokeRepository.GetOneRandom(@"{isRead: 0}")
                .As<Joke>();
            _jokeRepository.UpdateOne(joke.Id, "{\"isRead\": 1}");
            return joke;
        }

        public void AddJoke(Joke joke)
        {
            _jokeRepository.InsertOne(JsonConvert.SerializeObject(joke));
        }

        public void ConsumeJokesFromRss(params string[] rssUris)
        {
            IEnumerable<FeedItem> jokes = RssConsumer.RssConsumer.GetFeeds(rssUris);
            foreach (FeedItem joke in jokes)
            {
                if (string.IsNullOrEmpty(joke.Content)) { continue; }
                AddJoke(new Joke()
                {
                    Id = joke.Id,
                    Text = joke.Content,
                    IsRead = false
                });
            }
        }
    }
}
