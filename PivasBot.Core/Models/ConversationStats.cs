using System;
using System.Collections.Generic;
using System.Text;

namespace PivasBot.Core.Models
{
    public class ConversationStats
    {
        public DateTime LastMessageFromBotTime { get; set; }

        public DateTime LastMessagePostTime { get; set; }

        public int MessagesForPeriod { get; set; }

        public long SecondsFromLastBotMessage { get; set; }
    }
}
