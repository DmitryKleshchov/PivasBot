using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PivasBot.Core.MessageStore.Interfaces;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace PivasBot.Core.MessageStore.Repositories
{
    public abstract class BaseFileMessageRepository 
    {
        protected ConcurrentDictionary<int, Message> _store;
        

        public BaseFileMessageRepository(string path)
        {
            if (File.Exists(path))
            _store = InitStore(File.ReadAllText(path) + "]");
            else
            {
                _store = new ConcurrentDictionary<int, Message>();
            }
        }

        private ConcurrentDictionary<int, Message> InitStore(string messages)
        {
            var messagesArr = JsonConvert.DeserializeObject<Message[]>(messages);
            var result = new ConcurrentDictionary<int, Message>();
            foreach (Message m in messagesArr)
            {
                result.TryAdd(m.MessageId, m);
            }

            return result;
        }

        public void AddMessage(Message m)
        {
            string mStr = JsonConvert.SerializeObject(m);
            _store.TryAdd(m.MessageId, m);
            SaveMessage(mStr);
        }

        public Message GetRandomMessage()
        {
            Random random = new Random();
            int index = random.Next(0, _store.Count);
            return _store.GetValueOrDefault(_store.Keys.ElementAt(index), null);
        }
        public Message GetMessage(int messageId)
        {
            Message result;

            if (_store.TryGetValue(messageId, out result))
            {
                return result;
            }

            throw new KeyNotFoundException($"couldn't find message with the id {messageId} in store.");
        }

        private void SaveMessage(string s)
        {
            File.AppendAllText(Settings.ChatMessagesPath, "," + s);
        }

    }
}
