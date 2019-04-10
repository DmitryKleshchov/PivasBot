using System;
using System.Threading;
using MongoDB.Driver;
using Newtonsoft.Json;
using PivasBot.Core.Services;
using PivasBot.Db;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using JokeRepository = PivasBot.Db.Repositories.JokeRepository;
using MessageRepository = PivasBot.Db.Repositories.MessageRepository;

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
