using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using PivasBot.Core.Enums;
using PivasBot.Core.MessageStore.Repositories;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace PivasBot.Core.Managers
{
    public class BotCommandManager
    {
        private MessageRepository _messageRepository;
        private JokeRepository _jokeRepository;
        private TelegramBotClient _bot;
        public BotCommandManager(TelegramBotClient bot)
        {
            _messageRepository = MessageRepository.Instance;
            _jokeRepository = JokeRepository.Instance;
            _bot = bot;
        }

        public async Task ExecuteCommandAsync(MessageEventArgs e, BotCommand command)
        {
            switch (command)
            {
                case BotCommand.HochuSrati:
                    Vsratsa(e);
                    break;
                case BotCommand.Randomni:
                    await GenerateRandomMessage(e);
                    break;
                case BotCommand.Shutkani:
                    await PostJoke(e);
                    break;
                case BotCommand.AddJoke:
                    AddJoke(e);
                    break;
                default:
                    throw new ArgumentException($"This command type is not supported - {command}", nameof(command));
            }
        }


        public async Task Vsratsa(MessageEventArgs e)
        {
            await _bot.SendTextMessageAsync(e.Message.Chat.Id,
                $"Ты захотел всраться, а я хочу ебаться. Повернись ко мне жопой и твоё желание будет выполнено.",
                replyToMessageId: e.Message.MessageId);
        }

        public async Task GenerateRandomMessage(MessageEventArgs e)
        {
            Message m1 = _messageRepository.GetRandomMessage();
            Message m2 = _messageRepository.GetRandomMessage();
            await _bot.SendTextMessageAsync(e.Message.Chat.Id, m1.Text + " " + m2.Text);
        }

        public async Task PostJoke(MessageEventArgs e)
        {
            await _bot.SendTextMessageAsync(e.Message.Chat.Id, _jokeRepository.GetJoke());
        }

        public void AddJoke(MessageEventArgs e)
        {
            string jokeStr = e.Message.Text.Replace("/" + BotCommand.AddJoke, "", StringComparison.OrdinalIgnoreCase).Trim();
            _jokeRepository.AddJoke(jokeStr);
        }
    }
}
