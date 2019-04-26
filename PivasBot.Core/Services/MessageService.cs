using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using PivasBot.Db;
using PivasBot.Db.Extensions;
using PivasBot.Db.Repositories;
using Telegram.Bot.Types;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace PivasBot.Core.Services
{
    public class MessageService
    {
        private MessageRepository _messageRepository;

        public MessageService(DbConnection dbConn)
        {
            _messageRepository = new MessageRepository(dbConn);
        }

        public Message GetRandomMessage()
        {
            return _messageRepository.GetOneRandom()
                .As<Message>();
        }

        public void AddMessage(Message message)
        {
            _messageRepository.InsertOne(JsonConvert.SerializeObject(message));
        }
    }
}
