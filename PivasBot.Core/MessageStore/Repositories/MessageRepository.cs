using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PivasBot.Core.MessageStore.Interfaces;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace PivasBot.Core.MessageStore.Repositories
{
    public sealed class MessageRepository : BaseFileMessageRepository, IMessageRepository
    {
        static MessageRepository(){}

        private MessageRepository() : base(Settings.ChatMessagesPath)
        {
            
        } 
        
        public static MessageRepository Instance { get; } = new MessageRepository();
    }
}

