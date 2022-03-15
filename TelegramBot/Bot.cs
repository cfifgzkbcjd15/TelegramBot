using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBot.Data;
using TelegramBot.Models;

namespace TelegramBot
{

    public class MultipleModels
    {
        public Raspisanie Raspisanies { get; set; }
        public User User { get; set; }

    }
    class Bot
    {
        [Obsolete]
        public static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            //Scaffold-DbContext "Server=DESKTOP-4312I2K;Database=TelegramBot;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer
            var client = Program.client;
            var September = new DateTime(2019, 9, 01);//2019
            var msg = e.Message;
            using (var db = new TelegramBotContext())
            {
                string[] Days = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
                var now_Data = DateTime.Now;
                var q = Convert.ToInt32(September.DayOfWeek);
                for (var i = 0; i <= 7; i++)
                {
                    if (q - i == 1)
                    {
                        q = i;
                    }
                }
                var c = September.DayOfYear - q;
                var week = Convert.ToInt32((now_Data.DayOfYear - c) / 7) + 1;
                string one_down, two_zvezda_one_group, one_zvezda_two_group, two_zvezda_two_group, one_group, two_group, one_zvezda_one_group_2, one_zvezda_two_group_2, one_zvezda, gg = "";
                var zvezda = "";
                if (week % 2 == 0)
                {
                    zvezda = "**";
                }
                else
                {
                    zvezda = "*";
                }
                var Today = DateTime.Now.ToString("dddd");
                var Tomorrow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+1).ToString("dddd");
                if (msg.Text != null)
                {
                    if (msg.Text == "/start")
                    {
                        if (db.Users.Where(x => x.TelegramUserId == msg.From.Id).Count() == 0)
                        {
                            db.Users.Add(new User() { TelegramUserId = msg.From.Id, Course = 1, Specialization = "1", Faculty = "1", Level = "1" });
                            //await db.SaveChangesAsync();
                        }
                        await client.SendPhotoAsync(
                                  chatId: msg.Chat.Id,
                                  photo: "https://sun9-25.userapi.com/impg/wjMtGXUI1_-D_X_EaLtYIj_wyl9AIGXEEg3V8A/GKShLW8oG1c.jpg?size=450x450&quality=96&sign=718a97b2c586d26264e820d356962fcd&type=album",
                                  caption: "<b>Добро пожаловать!</b>Чтобы выполнить поиск преподавателя,просто введите его фамилию после выбора факультета",
                                  parseMode: ParseMode.Html,
                                  replyMarkup: Program.GetButtons()
);

                    }

                    

                        var pop = db.Users.Where(x => x.TelegramUserId == msg.From.Id).ToList();
                    if (pop.Count() != 0)
                    {
                        var first = pop.First();
                        var rasp = db.Raspisanies.FirstOrDefault(x => x.Profil == first.Specialization);
                        string prepod = "";
                        var user = db.Users.Single(x => x.TelegramUserId == msg.From.Id);
                        var proverka_prepod = db.Raspisanies.FirstOrDefault(x => x.OneGruppa.Contains(msg.Text));
                        var proverka="";
                        var proverka2 = "";
                        if (proverka_prepod != null) {
                        proverka = Regex.Match(proverka_prepod.OneGruppa, @"[А-Я]?(?<=\,).+\.").ToString().Replace(" ", "");
                        proverka2 = Regex.Match(proverka_prepod.OneGruppa, @"[А-Я]?(?<=\,).+?(?=\s)").ToString().Replace(" ","");
                        }
                        //РечновА.В.

                        try
                        {
                            if (proverka==msg.Text.Replace(" ","")||proverka2==msg.Text)
                            {
                                foreach (var j in Days)
                                {
                                    prepod += "\u23F0"+ j + "\n";
                                    foreach (var i in db.Raspisanies.Where(x => (x.OneGruppa.Contains(msg.Text) || x.TwoGruppa.Contains(msg.Text)) && x.Day.Replace(" ","") == j&&x.Faculty==first.Faculty).OrderBy(x => x.Para))
                                    {
                                        string para_prepod = "";
                                        string para_prepod_2 = "";
                                        var FIO_up_1 = Regex.Match(i.OneGruppa, @"^.+?(?<=\d{3})").ToString();
                                        var FIO_up_prepod_1 = Regex.Match(FIO_up_1, @"[А-Я]?(?<=\,\s).+\.").ToString();
                                        var FIO_up_prepod_bezkabineta= Regex.Match(i.OneGruppa, @"^.+[А-Я]?(?<=\,\s).+\.").ToString();
                                        //.+(?<=\d{3}).+
                                        //.+\n
                                        var FIO_down_1 = Regex.Match(i.OneGruppa, "\n.+").ToString();
                                        var para_1 = Regex.Match(i.OneGruppa, @"\n\*\s+[А-я]{2}\s+\d{3}").ToString();
                                        //[А-я].+
                                        //\n.+
                                        para_prepod = FIO_up_prepod_bezkabineta +"\n";
                                        if (FIO_up_1.Contains(msg.Text))
                                        {
                                            para_prepod = FIO_up_1 + "\n\n";
                                        }
                                        if (FIO_down_1.Contains(msg.Text))
                                        {
                                            para_prepod = FIO_down_1.Replace("\n", "") + "\n\n";
                                        }
                                        if (para_1 != "")
                                        {
                                            para_prepod = i.OneGruppa + "\n\n";
                                        }
                                        if (FIO_up_1.Contains(msg.Text) && FIO_down_1.Contains(msg.Text))
                                        {
                                            para_prepod = FIO_up_1 + "\n" + FIO_down_1.Replace("\n", "") + "\n\n";
                                        }
                                        var FIO_up_2 = Regex.Match(i.TwoGruppa, @"^.+?(?<=\d{3})").ToString();
                                        var FIO_up_prepod_2 = Regex.Match(FIO_up_2, @"[А-Я]?(?<=\,).+\.").ToString();
                                        //.+(?<=\d{3}).+
                                        //.+\n
                                        //.Replace("\n", "")
                                        var FIO_down_2 = Regex.Match(i.TwoGruppa, "\n.+").ToString();
                                        if (FIO_up_2.Contains(msg.Text))
                                        {
                                            para_prepod_2 = " 2 Группа:" + FIO_up_2 + "\n";
                                        }
                                        if (FIO_down_2.Contains(msg.Text))
                                        {
                                            para_prepod_2 = " 2 Группа:" + FIO_down_2 + "\n";
                                        }
                                        if (FIO_up_2.Contains(msg.Text) && FIO_down_2.Contains(msg.Text))
                                        {
                                            para_prepod_2 = " 2 Группа:" + FIO_up_2 + "\n" + FIO_down_2 + "\n";
                                        }
                                        prepod += "\u270f" + i.Time + "\n" + para_prepod + para_prepod_2;
                                        
                                    }
                                    var proverka_null = Regex.Match(prepod, $"\u23F0{j}\n.+").ToString();
                                    if (proverka_null == "")
                                        prepod = prepod.Replace("\u23F0" + j + "\n", "");
                                }
                                if(prepod!="")
                                await client.SendTextMessageAsync(msg.Chat.Id,
                                                                     prepod);
                                //replyMarkup: Program.LevelButtons(first.Faculty)
                            }
                            if (db.Raspisanies.FirstOrDefault(x=>x.Faculty==msg.Text)!=null)
                            {//и так для каждого
                                user.Faculty = msg.Text;

                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите уровень подготовки",
                                        replyMarkup: Program.LevelButtons(first.Faculty));
                            
                            }
                            //if (msg.Text == "/start")
                            //{
                            //    await client.SendTextMessageAsync(
                            //                    msg.Chat.Id,
                            //                    "Выберите уровень подготовки",
                            //                    replyMarkup: Program.LevelButtons(first.Faculty));
                            //}
                            if(db.Raspisanies.FirstOrDefault(x => x.Faculty == first.Faculty && x.Level == msg.Text)!=null)
                            if ((msg.Text == "Магистратура" || msg.Text == "Бакалавр") && db.Users.FirstOrDefault(x => x.TelegramUserId == msg.From.Id).Faculty != "1"&&db.Raspisanies.FirstOrDefault(x=>x.Faculty==first.Faculty&&x.Level==msg.Text).Level!="")
                            {
                                user.Level = msg.Text;//и так для каждого
                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите специальность",
                                        replyMarkup: Program.specializationButtons(msg.Text,first.Faculty));
                            }
                            //if(db.Raspisanies.Where(x => x.Profil == msg.Text)!=null)
                            foreach (var i in db.Raspisanies.Where(x => x.Profil == msg.Text&&x.Faculty==first.Faculty&&first.Level==x.Level).Select(x => x.Profil).Distinct().ToList())
                            {
                                if (i == msg.Text)
                                {
                                    user.Specialization = msg.Text;//и так для каждого
                                    await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите курс",
                                        replyMarkup: Program.CourceButtons(first.Specialization, first.Level));

                                }
                            }
                            //profil&&level
                            foreach (var i in db.Raspisanies.Where(x => x.Level == first.Level && x.Profil == first.Specialization&&x.Faculty==first.Faculty).Select(x => x.Course).Distinct())
                            {
                                if (msg.Text == i.ToString())
                                {

                                    int smoll = Convert.ToInt32(msg.Text);
                                    user.Course = smoll;
                                    await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        "Выберите день недели",
                                        replyMarkup: Program.DayButtons());
                                }
                            }
                            //level&&profil&&course
                            if (msg.Text == "Вся неделя" && pop.First().Course != 0)
                            {
                                foreach (var i in Days)
                                {
                                    gg += $"\u23F0 {i}\n";
                                    foreach (var j in db.Raspisanies.Where(x => x.Level == first.Level && x.Profil == first.Specialization && x.Day.Replace(" ","") == i && x.Course == first.Course).OrderBy(x => x.Para).ToList())
                                    {
                                        var a = "";
                                        one_group = Regex.Match(j.OneGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                        one_down = Regex.Match(j.OneGruppa, @"\n+\*.+").ToString();
                                        two_zvezda_one_group = Regex.Match(j.OneGruppa, @"^(\*\*)+.+").ToString();
                                        one_zvezda_one_group_2 = Regex.Match(j.OneGruppa, @".+?(?<=\*).").ToString();
                                        one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                        string one_up = Regex.Match(j.OneGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                        string one2 = Regex.Match(j.OneGruppa, @"^.+?(?<=\.).+\.").ToString();//.+?(?<=\.).+\.
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
                                                    var onee = Regex.Match(j.OneGruppa, @"^.+?(?=[А-я]{2}\s+\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.
                                                    //.+?(?=[А-я]{2}\s\d{3})
                                                    //.+?(?=[А-я]{2}).+\s\d{3}

                                                    if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                    {
                                                        a = $"\u270f {j.Para} Пара {j.Time}\n{one_down.Replace("\n", "")}";
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
                                        if (j.TwoGruppa != "" && j.OneGruppa==""||j.TwoGruppa!=""&&j.OneGruppa!="")
                                        {
                                            two_group = Regex.Match(j.TwoGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                            one_zvezda_two_group = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                            two_zvezda_two_group = Regex.Match(j.TwoGruppa, @"^(\*\*)+.+").ToString();
                                            one_zvezda_two_group_2 = Regex.Match(j.TwoGruppa, @".+?(?<=\*).").ToString();
                                            one_up = Regex.Match(j.TwoGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                            one_down = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                            one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                            one2 = Regex.Match(j.TwoGruppa, @"^.+?(?<=\.).+\.").ToString();
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
                                            if (Regex.Match(j.TwoGruppa, @"^\d\-[A-z].+").ToString() != "")
                                            {
                                                a += $"2 Группа:{j.TwoGruppa}";
                                            }
                                            if ((one_zvezda_two_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_two_group_2.Replace(" ", "") != "")
                                            {
                                                if (zvezda == "**" && zvezda == one_zvezda_two_group_2.Replace(" ", ""))
                                                {
                                                    a += $"2 Группа:{two_zvezda_two_group}";
                                                }

                                                if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                                {
                                                    var onee = Regex.Match(j.TwoGruppa, @"^.+?(?=[А-я]{2}\s+\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.

                                                    if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                    {
                                                        a += $"2 Группа:{one_down}";
                                                    }

                                                    if (one_down.Replace("\n", "").Replace(" ", "") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                    {
                                                        var one1_add = one_down.Insert(one2_index, one2_remove);
                                                        a += $"2 Группа:{one1_add}";
                                                    }
                                                }
                                                if (one_up == "*")
                                                {
                                                    a += $"2 Группа:{j.TwoGruppa}";
                                                }
                                            }
                                        }
                                        if (j.TwoGruppa == "День самостоятельной работы")
                                        {
                                            a = j.TwoGruppa;
                                        }

                                        if(a!="")
                                        gg = gg + a + "\n\n";

                                    }
                                }
                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        text: gg,
                                        replyMarkup: Program.DayButtons());
                            }
                            if (msg.Text == "Вернуться в начало")
                            {
                                await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        text: "Выберите факультет",
                                        replyMarkup: Program.GetButtons());
                            }
                            if ((msg.Text == "Сегодня" || msg.Text == "Завтра") && first.Course != 0) {
                                if (msg.Text == "Завтра" && DateTime.Now.DayOfWeek + 1==DayOfWeek.Monday) {
                                    var Data = DateTime.Now.DayOfYear + 1;
                                    q = Convert.ToInt32(September.DayOfWeek);
                                    for (var i = 0; i <= 7; i++)
                                    {
                                        if (q - i == 1)
                                        {
                                            q = i;
                                        }
                                    }
                                    c = September.DayOfYear - q;
                                    week = Convert.ToInt32((Data - c) / 7) + 1;
                                }
                                if ((week) % 2 == 0)
                                {
                                    zvezda = "**";
                                }
                                else
                                {
                                    zvezda = "*";
                                }
                                string button_day = "";
                                if (msg.Text == "Сегодня")
                                {
                                    button_day = Today;
                                }
                                if (msg.Text == "Завтра")
                                {
                                    button_day = Tomorrow;
                                }
                                var day = "\u23F0" + button_day+ "\n";
                                if (button_day=="воскресенье")
                                {
                                    gg = "День самоподготовки";
                                }
                                else
                            //if (button_day == db.Raspisanies.FirstOrDefault(x => x.Day == button_day).Day)
                            //    {
                                    
                                    foreach (var j in db.Raspisanies.Where(x => x.Level == first.Level && x.Day.Replace(" ", "") == button_day && x.Course == first.Course && x.Profil == first.Specialization).OrderBy(x => x.Para).ToList())
                                    {

                                        var a = "";
                                        one_group = Regex.Match(j.OneGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                        one_down = Regex.Match(j.OneGruppa, @"\n+\*.+").ToString();//косяк
                                        two_zvezda_one_group = Regex.Match(j.OneGruppa, @"^(\*\*)+.+").ToString();
                                        one_zvezda_one_group_2 = Regex.Match(j.OneGruppa, @".+?(?<=\*).").ToString();
                                        one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                        string one_up = Regex.Match(j.OneGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                        string one2 = Regex.Match(j.OneGruppa, @"^.+?(?<=\.).+\.").ToString();//.+?(?<=\.).+\.
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
                                                    var onee = Regex.Match(j.OneGruppa, @"^.+?(?=[А-я]{2}\s+\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.
                                                    //.+?(?=[А-я]{2}\s\d{3})
                                                    //.+?(?=[А-я]{2}).+\s\d{3}

                                                    if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                    {
                                                        a = $"\u270f {j.Para} Пара {j.Time}\n{one_down.Replace("\n", "")}";
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
                                                if (a == $"\u270f {j.Para} Пара {j.Time}\n")
                                                    a = "";
                                            }

                                        }
                                        if (j.TwoGruppa != "")
                                        {
                                            two_group = Regex.Match(j.TwoGruppa, @"^[А-я].+").ToString().Replace("\n", "");
                                            one_zvezda_two_group = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                            two_zvezda_two_group = Regex.Match(j.TwoGruppa, @"^(\*\*)+.+").ToString();
                                            one_zvezda_two_group_2 = Regex.Match(j.TwoGruppa, @".+?(?<=\*).").ToString();
                                            one_up = Regex.Match(j.TwoGruppa, @"^.+? (?<=\s)").ToString().Replace(" ", "");
                                            one_down = Regex.Match(j.TwoGruppa, @"\n+\*.+").ToString();
                                            one_zvezda = Regex.Match(one_down, @"\*").ToString();//\n и получается если *
                                            one2 = Regex.Match(j.OneGruppa, @"^.+?(?<=\.).+\.").ToString();
                                            if (one2 != "")
                                            {
                                                one2_remove = one2.Remove(0, 2);//**
                                                one2_index = one_zvezda_two_group.IndexOf("\n");//\n...
                                                one1_add_if = Regex.Match(j.TwoGruppa, @"\n.\s+[А-я]{2}\s+\d{3}").ToString();
                                            }
                                            if (two_group != "")
                                            {
                                                a += $"2 Группа:{two_group}";
                                            }
                                            if (Regex.Match(j.TwoGruppa, @"^\d\-[A-z].+").ToString() != "")
                                            {
                                                a += $"2 Группа:{j.TwoGruppa}";
                                            }
                                            if ((one_zvezda_two_group_2.Replace(" ", "") == zvezda || one_zvezda == zvezda) && one_zvezda_two_group_2.Replace(" ", "") != "")
                                            {
                                                if (zvezda == "**" && zvezda == one_zvezda_two_group_2.Replace(" ", ""))
                                                {
                                                    a += $"2 Группа:{two_zvezda_two_group}";
                                                }

                                                if (zvezda == "*" && zvezda == one_zvezda.Replace(" ", ""))
                                                {
                                                    var onee = Regex.Match(j.TwoGruppa, @"^.+?(?=[А-я]{2}\s+\d{3})").ToString().Remove(0, 2);//Проектирование информационных систем Богомолов А. В.

                                                    if (one2_remove.Replace(" ", "") == onee.Replace(" ", ""))
                                                    {
                                                        a += $"2 Группа:{one_down}";
                                                    }

                                                    if (one_down.Replace("\n", "").Replace(" ", "") == one1_add_if.Replace(" ", "") && one2_index >= 0)
                                                    {
                                                        var one1_add = one_down.Insert(one2_index, one2_remove);
                                                        a += $"2 Группа:{one1_add}";
                                                    }
                                                }
                                                if (one_up == "*")
                                                {
                                                    a += $"2 Группа:{j.TwoGruppa}";
                                                }
                                            }
                                        }
                                        if (j.TwoGruppa == "День самостоятельной работы")
                                        {
                                            a = j.TwoGruppa;
                                        }
                                        day = day + a + "\n\n";

                                    }
                                    
                                //}
                                await client.SendTextMessageAsync(
                                            msg.Chat.Id,
                                            day += gg,
                                            replyMarkup: Program.DayButtons());
                            }
                        }
                        catch
                        {
                            await client.SendTextMessageAsync(
                                           msg.Chat.Id,
                                           "Произошла ошибка",
                                           replyMarkup: Program.GetButtons());
                        }
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
