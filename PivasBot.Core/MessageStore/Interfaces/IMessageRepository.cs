using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace PivasBot.Core.MessageStore.Interfaces
{
    interface IMessageRepository
    {
        void AddMessage(Message m);
        Message GetRandomMessage();
        Message GetMessage(int messageId);
    }
}
