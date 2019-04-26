using PivasBot.Core.Enums;
using PivasBot.Core.Managers;
using PivasBot.Core.Models;
using PivasBot.Db;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace PivasBot.Core.Services
{
    public class PivasBotService
    {
        private TelegramBotClient _botClient;
        private MessageService _messageService;
        private readonly JokeService _jokeService;
        private BotCommandManager _commandManager;
        private ConversationStats _conversationStats;
        private const int MessageLenLimit = 300;
        private const int TotalMessagesInConvoToParticipate = 5;
        private const int DelayForActiveConversationSecs = 20;
        private const int BotMessageDelaySecs = 300;

        public PivasBotService(string botToken, string dbConnectionString = null)
        {
            DbConnection dbConn = new DbConnection(dbConnectionString);
            _botClient = new TelegramBotClient(botToken);
            _messageService = new MessageService(dbConn);
            _jokeService = new JokeService(dbConn);
            _commandManager = new BotCommandManager(_botClient, _messageService, _jokeService);
            _conversationStats = new ConversationStats();
        }


        public void StartBot()
        {
            _botClient.OnMessage += SaveMessage;
            _botClient.OnMessage += ProcessCommands;
            _botClient.OnMessage += ParticipateInConversation;
            _botClient.StartReceiving();

            //todo: remove the hardcoded uris.
            _jokeService.ConsumeJokesFromRss(
                
                );
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

        private void UpdateStats(MessageEventArgs e)
        {
            double secondsFromLastMessage = (e.Message.Date - _conversationStats.LastMessagePostTime).TotalSeconds;
            if (secondsFromLastMessage < DelayForActiveConversationSecs)
            {
                _conversationStats.MessagesForPeriod++;
            }
            else
            {
                _conversationStats.MessagesForPeriod = 0;
            }

            // Logic dependent on old time above. move this with care.
            _conversationStats.LastMessagePostTime = e.Message.Date;

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

        private async void ParticipateInConversation(object sender, MessageEventArgs e)
        {
            if (e.Message?.Entities?.Any() == true && e.Message.Entities.First().Type != MessageEntityType.BotCommand)
            {
                return;
            }

            UpdateStats(e);
            DateTime now = DateTime.Now;

            if (_conversationStats.MessagesForPeriod >= TotalMessagesInConvoToParticipate &&
                (now - _conversationStats.LastMessageFromBotTime).TotalSeconds >= BotMessageDelaySecs)
            {
                await _commandManager.ExecuteCommandAsync(e, BotCommand.Randomni);
                _conversationStats.LastMessageFromBotTime = now;
                _conversationStats.MessagesForPeriod = 0;
            }
        }
    }
}
