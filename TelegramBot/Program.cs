using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.Models;

namespace TelegramBot
{


    class Program
    {
        private static string Token { get; set; } = "";
        public static TelegramBotClient client;


        [Obsolete]
        static void Main(string[] args)
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += Bot.OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }


        public static IReplyMarkup LevelButtons(string faculty)
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new TelegramBotContext())
            {
                foreach (var i in db.Raspisanies.Where(x =>x.Faculty == faculty).OrderBy(x => x.Id).Select(x => x.Level).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i } });
                }
            }
            return new ReplyKeyboardMarkup(Keyboardd);

        }
        public static IReplyMarkup specializationButtons(string level,string faculty)
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new TelegramBotContext())
            {
                foreach (var i in db.Raspisanies.Where(x => x.Level == level&&x.Faculty==faculty).OrderBy(x => x.Id).Select(x => x.Profil).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i } });
                }
            }
            return new ReplyKeyboardMarkup(Keyboardd,true);

        }
        public static IReplyMarkup DayButtons()
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = "Сегодня" }, new KeyboardButton { Text = "Завтра" }, new KeyboardButton { Text = "Вся неделя" } });
            Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = "Вернуться в начало" } });
            return new ReplyKeyboardMarkup(Keyboardd, true);
            //{
            //    Keyboard = new List<List<KeyboardButton>>
            //    {
            //    new List<KeyboardButton> { new KeyboardButton { Text = "Сегодня" }, new KeyboardButton { Text = "Завтра" }, new KeyboardButton { Text = "Вся неделя" } }
            //}
            //};

        }
        public static IReplyMarkup CourceButtons(string profil, string level)
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new TelegramBotContext())
            {
                foreach (var i in db.Raspisanies.Where(x => x.Profil == profil && x.Level == level).Select(x => x.Course).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i.ToString() } });
                }
            }
            return new ReplyKeyboardMarkup { Keyboard = Keyboardd };
        }

        public static IReplyMarkup GetButtons()
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new TelegramBotContext())
            {
                foreach (var i in db.Raspisanies.Select(x => x.Faculty).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i.ToString() } });
                }
            }
            return new ReplyKeyboardMarkup(Keyboardd, true);
        }
    }
}
