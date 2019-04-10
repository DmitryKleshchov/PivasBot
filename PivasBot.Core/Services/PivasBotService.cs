using PivasBot.Core.Enums;
using PivasBot.Core.Managers;
using PivasBot.Db;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace PivasBot.Core.Services
{
    public class PivasBotService
    {
        private TelegramBotClient _botClient;
        private MessageService _messageService;
        private readonly JokeService _jokeService;
        private BotCommandManager _commandManager;
        private const int MessageLenLimit = 300;
        public PivasBotService(string botToken, string dbConnectionString = null)
        {
            DbConnection dbConn = new DbConnection(dbConnectionString);
            _botClient = new TelegramBotClient(botToken);
            _messageService = new MessageService(dbConn);
            _jokeService = new JokeService(dbConn);
            _commandManager = new BotCommandManager(_botClient, _messageService, _jokeService);
        }


        public void StartBot()
        {
            _botClient.OnMessage += SaveMessage;
            _botClient.OnMessage += ProcessCommands;

            _botClient.StartReceiving();
        }

        private async void SaveMessage(object sender, MessageEventArgs e)
        {
            try
            {
                Console.WriteLine("SaveMessage entered");
                if (!string.IsNullOrEmpty(e.Message.Text) &&
                    e.Message.Text.Length < MessageLenLimit &&
                    !GetCommands().Any(x => e.Message.Text.ToLower().Contains(x.ToLower())))
                {
                    _messageService.AddMessage(e.Message);
                    Console.WriteLine("Saved Message");
                }
            }
            catch (Exception ex)
            {
                await SendErrorMessage(e.Message.Chat.Id, e.Message.MessageId, ex.Message);
            }
        }


        private async void ProcessCommands(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.Message.Text != null)
                {
                    string[] commands = GetCommands();
                    foreach (string command in commands)
                    {
                        if (e.Message.Text.ToLower().Contains(command.ToLower()))
                        {
                            await _commandManager.ExecuteCommandAsync(e, Enum.Parse<BotCommand>(command));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await SendErrorMessage(e.Message.Chat.Id, e.Message.MessageId, ex.Message);
            }
        }

        private string[] GetCommands()
        {
            return Enum.GetNames(typeof(BotCommand));
        }

        private async Task SendErrorMessage(long chatId, int messageId, string message)
        {
            await _botClient.SendTextMessageAsync(chatId, $"Пидар, ты меня сломал. {message}",
                 replyToMessageId: messageId);
        }
    }
}
