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
//using static TelegramBot.Data.ApplicationDbContext;

namespace TelegramBot
{
    class Program
    {
        private static string Token { get; set; } = "";
        private static TelegramBotClient client;
        public static string profil { get; set; } = "";
        public static string course { get; set; } = "";
        public static string faculty { get; set; } = "";
        public static string level { get; set; } = "";
        public static DateTime September { get; set; } = new DateTime(2021, 9, 01);

        [Obsolete]
        static void Main(string[] args)
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            List<string> pop = new List<string>();
            string[] Days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            var now_Data = DateTime.Now;
            var msg = e.Message;
            var q = Convert.ToInt32(September.DayOfWeek);
            for (var i = 0; i <= 7; i++)
            {
                if (q - i == 1)
                {
                    q = i;
                }
            }
            var c = September.DayOfYear - q;
            var week = Convert.ToInt32((now_Data.DayOfYear - c) / 7)+1;
            string one_down, two_zvezda_one_group, one_zvezda_two_group, two_zvezda_two_group,one_group,two_group, one_zvezda_one_group_2, one_zvezda_two_group_2, one_zvezda, gg = "";
            var zvezda = "";
            if (week % 2 == 0)
            {
                zvezda = "**";
            }
            else
            {
                zvezda = "*";
            }
            using (var db = new KadrsContext())
            {
                try {
                if (msg.Text != null)
                {
                    if (msg.Text == "/start")
                    {
                        await client.SendPhotoAsync(
                                  chatId: msg.Chat.Id,
                                  photo: "https://sun9-25.userapi.com/impg/wjMtGXUI1_-D_X_EaLtYIj_wyl9AIGXEEg3V8A/GKShLW8oG1c.jpg?size=450x450&quality=96&sign=718a97b2c586d26264e820d356962fcd&type=album",
                                  caption: "<b>Добро пожаловать!</b>",
                                  parseMode: ParseMode.Html,
                                  replyMarkup: GetButtons()
);

                        }
                    if (msg.Text == "Физико-Математический Факультет"|| msg.Text == "физико-математический факультет"|| msg.Text == "Физмат"||msg.Text == "физмат" || msg.Text == "Физико-математический факультет")
                    {
                        faculty = msg.Text;
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                "Выберите уровень подготовки",
                                replyMarkup: LevelButtons());
                    }
                    
                    if ((msg.Text == "Магистратура" || msg.Text == "Бакалавр")&&faculty!="")
                    {
                        
                        level = msg.Text;
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                "Выберите специальность",
                                replyMarkup: specializationButtons());
                    }
                    foreach (var i in db.Raspisanies.Select(x => x.Profil).Distinct().ToList())
                        {
                            if (i == msg.Text&&level!="")
                            {
                                profil = msg.Text;
                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите курс",
                                        replyMarkup: CourceButtons(profil));

                            }
                        }
                    foreach (var i in db.Raspisanies.Where(x => x.Profil == profil & x.Level ==level).Select(x => x.Course).Distinct().ToList())
                    {
                        if (msg.Text == i.ToString()) {
                                course = msg.Text;
                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите день недели",
                                        replyMarkup: DayButtons());
                            }
                    }
                    if(msg.Text=="Вся неделя"&&course!="")
                    {
                        foreach (var i in Days)
                        {
                            gg += $"\u2705 {i}\n";
                            foreach (var j in db.Raspisanies.Where(x =>x.Level==level&& x.Day == i &&x.Course.ToString() == course && x.Profil.Contains(profil)).OrderBy(x => x.Para).ToList())
                            {
                                    var a = "";
                                    one_group = Regex.Match(j.OneGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                    one_down = Regex.Match(j.OneGruppa, @"\n+\*.+").ToString();//косяк
                                    two_zvezda_one_group = Regex.Match(j.OneGruppa, @"^(\*\*)+.+").ToString();
                                    one_zvezda_one_group_2 = Regex.Match(j.OneGruppa, @".+?(?<=\*).").ToString();
                                    one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                    string one_up = Regex.Match(j.OneGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                    string one2 = Regex.Match(j.OneGruppa, @".+?(?<=\.).+\.").ToString();
                                    string one1_add_if = "";
                                    string one2_remove = "";
                                    int one2_index = 0;
                                    var replace = one_down.Replace("\n", "");
                                    if (one2 != "")
                                    {
                                        one2_remove = one2.Remove(0, 2);//**
                                        one2_index = one_down.IndexOf("\n");//\n...
                                        one1_add_if = Regex.Match(j.OneGruppa, @"\n.\s+[А-я]{2}\s+\d{3}").ToString().Replace("\n", "");
                                    }
                                    if (j.OneGruppa != "")
                                    {


                                        if (one_group != "")
                                        {
                                            a = $"\u270f {j.Para} Пара {j.Time}\n{one_group}";
                                        }
                                        if (Regex.Match(j.OneGruppa, @"^\d\-[A-z].+").ToString() != "")
                                        {
                                            a = $"\u270f {j.Para} Пара {j.Time}\n{j.OneGruppa}";
                                        }
                                        if ((one_zvezda_one_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_one_group_2.Replace(" ", "") != "")
                                        {
                                            if (zvezda == "**" && zvezda == one_zvezda_one_group_2.Replace(" ", ""))
                                            {
                                                a = $"\u270f {j.Para} Пара {j.Time}\n{two_zvezda_one_group}";
                                            }

                                            if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                            {
                                                var onee = Regex.Match(j.OneGruppa, @".+?(?=[А-я]{2}\s\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.

                                                if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                {
                                                    a = $"\u270f {j.Para} Пара {j.Time}\n{one_down}";
                                                }

                                                if (one_down.Replace("\n", "").Replace(" ", "") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                {
                                                    var one1_add = one_down.Insert(one2_index, one2_remove);
                                                    a = $"\u270f {j.Para} Пара {j.Time}\n{one1_add}";
                                                }
                                            }
                                            if (one_up == "*")
                                            {
                                                a = $"\u270f {j.Para} Пара {j.Time}\n{j.OneGruppa}";
                                            }
                                        }

                                    }
                                    if (j.TwoGruppa != "")
                                    {
                                        two_group = Regex.Match(j.TwoGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                        one_zvezda_two_group = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                        two_zvezda_two_group = Regex.Match(j.TwoGruppa, @"^(\*\*)+.+").ToString();
                                        one_zvezda_two_group_2 = Regex.Match(j.TwoGruppa, @".+?(?<=\*).").ToString();
                                        one_up = Regex.Match(j.TwoGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                        one_down = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();//косяк
                                        one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                        one2 = Regex.Match(j.TwoGruppa, @".+?(?<=\.).+\.").ToString();
                                        if (one2 != "")
                                        {
                                            one2_remove = one2.Remove(0, 2);//**
                                            one2_index = one_zvezda_two_group.IndexOf("\n");//\n...
                                            one1_add_if = Regex.Match(j.TwoGruppa, @"\n.\s+[А-я]{2}\s+\d{3}").ToString();
                                        }
                                        if (two_group != "")
                                        {
                                            a += $"|{two_group}";
                                        }
                                        if (Regex.Match(j.OneGruppa, @"^\d\-[A-z].+").ToString() != "")
                                        {
                                            a += $"|{j.OneGruppa}";
                                        }
                                        if ((one_zvezda_two_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_two_group_2.Replace(" ", "") != "")
                                        {
                                            if (zvezda == "**" && zvezda == one_zvezda_two_group_2.Replace(" ", ""))
                                            {
                                                a += $"|{two_zvezda_two_group}";
                                            }

                                            if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                            {
                                                var onee = Regex.Match(j.TwoGruppa, @".+?(?=[А-я]{2}\s\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.

                                                if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                {
                                                    a += $"|{one_down}";
                                                }

                                                if (one_down.Replace("\n", "").Replace(" ", "") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                {
                                                    var one1_add = one_down.Insert(one2_index, one2_remove);
                                                    a += $"|{one1_add}";
                                                }
                                            }
                                            if (one_up == "*")
                                            {
                                                a += $"|{j.TwoGruppa}";
                                            }
                                        }
                                        if (one_up == "*")
                                        {
                                            a += $"|{j.OneGruppa}";
                                        }
                                    }
                                    if (j.OneGruppa== "День самостоятельной работы")
                                {
                                    a = j.OneGruppa;
                                }
                                    
                                
                                gg = gg + a + "\n\n";
                                
                            }
                        }
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                text:gg,
                                replyMarkup: DayButtons());
                    }
                    if(msg.Text=="Вернуться в начало")
                    {
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                text: "Выберите факультет",
                                replyMarkup: GetButtons());
                    }
                    if ((msg.Text == "Понедельник" || msg.Text == "Вторник" || msg.Text == "Среда" || msg.Text == "Четверг" || msg.Text == "Пятница" || msg.Text == "Суббота")&&course!="")
                        if (msg.Text == db.Raspisanies.FirstOrDefault(x => x.Day == msg.Text).Day)
                        {
                                foreach (var j in db.Raspisanies.Where(x => x.Level == level && x.Day == msg.Text&& x.Course.ToString()==course && x.Profil.Contains(profil)).OrderBy(x=>x.Para).ToList())
                                {

                                    var a = "";
                                    one_group = Regex.Match(j.OneGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                    one_down = Regex.Match(j.OneGruppa, @"\n+\*.+").ToString();//косяк
                                    two_zvezda_one_group = Regex.Match(j.OneGruppa, @"^(\*\*)+.+").ToString();
                                    one_zvezda_one_group_2 = Regex.Match(j.OneGruppa, @".+?(?<=\*).").ToString();
                                    one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                    string one_up=Regex.Match(j.OneGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                    string one2 = Regex.Match(j.OneGruppa, @".+?(?<=\.).+\.").ToString();
                                    string one1_add_if= "";
                                    string one2_remove = "";
                                    int one2_index = 0;
                                    var replace = one_down.Replace("\n", "");
                                    if (one2 != "") {
                                    one2_remove = one2.Remove(0, 2);//**
                                    one2_index = one_down.IndexOf("\n");//\n...
                                    one1_add_if = Regex.Match(j.OneGruppa, @"\n.\s+[А-я]{2}\s+\d{3}").ToString().Replace("\n", "");
                                    }
                                    if (j.OneGruppa != "")
                                    {
                                        if (one_group != "" )
                                        {
                                            a = $"\u270f {j.Para} Пара {j.Time}\n{one_group}";
                                        }
                                        if(Regex.Match(j.OneGruppa, @"^\d\-[A-z].+").ToString() != "")
                                        {
                                            a= $"\u270f {j.Para} Пара {j.Time}\n{j.OneGruppa}";
                                        }
                                        if ((one_zvezda_one_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_one_group_2.Replace(" ", "") != "")
                                        {
                                            if (zvezda == "**" && zvezda == one_zvezda_one_group_2.Replace(" ", ""))
                                            {
                                                a = $"\u270f {j.Para} Пара {j.Time}\n{two_zvezda_one_group}";
                                            }
                                            
                                            if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                            {
                                                var onee = Regex.Match(j.OneGruppa, @".+?(?=[А-я]{2}\s\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.
                                                
                                                if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                {
                                                    a = $"\u270f {j.Para} Пара {j.Time}\n{one_down}";
                                                }
                                                
                                                if (one_down.Replace("\n", "").Replace(" ","") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                {
                                                    var one1_add = one_down.Insert(one2_index, one2_remove);
                                                    a = $"\u270f {j.Para} Пара {j.Time}\n{one1_add}";
                                                }
                                            }
                                            if (one_up == "*")
                                            {
                                                a = $"\u270f {j.Para} Пара {j.Time}\n{j.OneGruppa}";
                                            }
                                        }

                                    }
                                    if (j.TwoGruppa != "") 
                                    {
                                    two_group = Regex.Match(j.TwoGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                    one_zvezda_two_group = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                    two_zvezda_two_group = Regex.Match(j.TwoGruppa, @"^(\*\*)+.+").ToString();
                                    one_zvezda_two_group_2 = Regex.Match(j.TwoGruppa, @".+?(?<=\*).").ToString();
                                    one_up = Regex.Match(j.TwoGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                    one_down = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();//косяк
                                    one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                    one2 = Regex.Match(j.TwoGruppa, @".+?(?<=\.).+\.").ToString();
                                        if (one2 != "")
                                        {
                                            one2_remove = one2.Remove(0, 2);//**
                                            one2_index = one_zvezda_two_group.IndexOf("\n");//\n...
                                            one1_add_if = Regex.Match(j.TwoGruppa, @"\n.\s+[А-я]{2}\s+\d{3}").ToString();
                                        }
                                        if (two_group != "")
                                        {
                                            a += $"|{two_group}";
                                        }
                                        if (Regex.Match(j.OneGruppa, @"^\d\-[A-z].+").ToString() != "")
                                        {
                                            a += $"|{j.OneGruppa}";
                                        }
                                        if ((one_zvezda_two_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_two_group_2.Replace(" ", "") != "")
                                        {
                                            if (zvezda == "**" && zvezda == one_zvezda_two_group_2.Replace(" ", ""))
                                            {
                                                a += $"|{two_zvezda_two_group}";
                                            }

                                            if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                            {
                                                var onee = Regex.Match(j.TwoGruppa, @".+?(?=[А-я]{2}\s\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.

                                                if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                {
                                                    a +=$"|{one_down}";
                                                }

                                                if (one_down.Replace("\n", "").Replace(" ", "") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                {
                                                    var one1_add = one_down.Insert(one2_index, one2_remove);
                                                    a +=$"|{one1_add}";
                                                }
                                            }
                                            if (one_up == "*")
                                            {
                                                a+=$"|{j.TwoGruppa}";
                                            }
                                        }
                                        if (one_up == "*")
                                        {
                                            a+= $"|{j.OneGruppa}";
                                        }
                                    }
                                    if (j.OneGruppa == "День самостоятельной работы")
                                    {
                                        a = j.OneGruppa;
                                    }
                                    gg = gg + a + "\n\n";
                                    
                                }
                            await client.SendTextMessageAsync(
                                    msg.Chat.Id,
                                    gg,
                                    replyMarkup: DayButtons());
                        }
                }
                }
                catch
                {
                    await client.SendTextMessageAsync(
                                   msg.Chat.Id,
                                   "Произошла ошибка",
                                   replyMarkup: GetButtons());
                }
            }
        }
        private static IReplyMarkup LevelButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                new List<KeyboardButton> { new KeyboardButton { Text = "Магистратура" }, new KeyboardButton { Text = "Бакалавр" }}
            }
            };

        }
        private static IReplyMarkup specializationButtons()
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new KadrsContext())
            {
                foreach (var i in db.Raspisanies.Where(x=>x.Level==level).OrderBy(x => x.Id).Select(x => x.Profil).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i } });
                }
            }
            return new ReplyKeyboardMarkup {Keyboard=Keyboardd };

        }
        private static IReplyMarkup DayButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                new List<KeyboardButton> { new KeyboardButton { Text = "Понедельник" }, new KeyboardButton { Text = "Вторник" }, new KeyboardButton { Text = "Среда" } },
                new List<KeyboardButton> { new KeyboardButton { Text = "Четверг" }, new KeyboardButton { Text = "Пятница" }, new KeyboardButton { Text = "Суббота" } },
                new List<KeyboardButton> { new KeyboardButton { Text = "Вся неделя" }, new KeyboardButton { Text = "Вернуться в начало" } }
            }
            };

        }
        private static IReplyMarkup CourceButtons(string profill)
        {
            List<List<KeyboardButton>> Keyboardd = new List<List<KeyboardButton>>();
            using (var db = new KadrsContext())
            {
                foreach (var i in db.Raspisanies.Where(x => x.Profil == profil&&x.Level==level).Select(x => x.Course).Distinct().ToList())
                {
                    Keyboardd.Add(new List<KeyboardButton> { new KeyboardButton { Text = i.ToString() } });
                }
            }
            return new ReplyKeyboardMarkup { Keyboard = Keyboardd };
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                new List<KeyboardButton> { new KeyboardButton { Text = "Физико-Математический Факультет" }}
            }
            };
        }
    }
}
