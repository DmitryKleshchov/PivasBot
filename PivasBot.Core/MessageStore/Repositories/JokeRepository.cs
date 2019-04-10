using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using File = System.IO.File;

namespace PivasBot.Core.MessageStore.Repositories
{
    public class JokeRepository
    {
        protected ConcurrentDictionary<int, string> _store;
        private readonly string _path;
        private const string Separator = "?:%";

        private JokeRepository()
        {
            _path = Settings.SavedJokesPath;
            string jokes = string.Empty;
            if (File.Exists((_path)))
            {
                jokes = File.ReadAllText(_path);
            }

            _store = InitStore(jokes);
        }

        private ConcurrentDictionary<int, string> InitStore(string messages)
        {
            var result = new ConcurrentDictionary<int, string>();
            if (string.IsNullOrEmpty(messages))
            {
                return result;
            }

            var jokesArr = messages.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < jokesArr.Length; i++)
            {
                result.TryAdd(i, jokesArr[i]);
            }

            return result;
        }

        static JokeRepository() { }

        public static JokeRepository Instance { get; } = new JokeRepository();

        public void AddJoke(string jokeStr)
        {
            _store.TryAdd(_store.Count, jokeStr);
            File.AppendAllText(_path, Separator + jokeStr);
        }

        public string GetJoke()
        {
            Random rnd = new Random();
            return _store.GetValueOrDefault(rnd.Next(0, _store.Count));
        }
    }
}
