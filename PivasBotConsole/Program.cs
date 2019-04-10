using System;
using System.Threading;
using PivasBot.Core.Services;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace PivasBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new PivasBotService(Settings.Default.BotToken).StartBot();
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
