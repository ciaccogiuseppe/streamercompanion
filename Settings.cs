using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace streamerCompanion
{
    public class Pose
    {
        public string Image = "";
        //public string Audio = "";
    }
    public class Command
    {
        public string Image = "";
        public string Script = "";
        public string Access = "";
        public bool ViewerOnce = false;
        public int ViewerTimeout = 0;
        public int GlobalTimeout = 0;
        public bool Enabled = true;
        public List<string> Aliases = new List<string>();
        public List<string> Hotkey = new List<string>();


    }

    public class Bits
    {
        public string Name = "";
        public int From = 0;
        public int To = 0;
    }

    public class ScheduledMessage
    {
        public string Name = "";
        public int Timer = 0;
        public int MinLines = 0;
        public bool Enabled = true;
        public string Command = "";
        public string Image = "";
    }

    public class Subs
    {
        public string Name = "";
        public int From = 0;
        public int To = 0;
        public string Tier = "";
    }

    public class Trigger
    {
        public string Name = "";
        public string Filename = "";
        public bool Enabled = true;
        public string Command = "";
        public string Message = "";
        public string Image = "";
    }

    public class MascotImage
    {
        public string Image = "";
        public int MouthHeight = 0;
        public int Time = 0;
    }

    public class MascotAudio
    {
        public List<string> Audio = new List<string>();
        public float Volume = 1.0f;
    }

    public class MascotStyle
    {
        public int MascotMaxWidth = 0;
    }

    class Mascot
    {
        public List<MascotImage> Images = new List<MascotImage>();
        public List<MascotAudio> Audios = new List<MascotAudio>();
        public List<MascotStyle> Styles = new List<MascotStyle>();
    }

    public class Rewards
    {
        public string Name;
    }

    public class JsonData
    {
        public string ClientId = "";
        public string ClientSecret = "";
        public string TwitchChannel = "";
        public string TwitchOAUTH = "";
        public string TwitchBotChannel = "";
        public string TwitchBotOAUTH = "";
        public string ClientOAuth = "";
        public bool UseChatbot = false;
        public string CurrentMascot = "";
        public string AlignMascot = "";
        public string HostMessage = "";
        public string AutohostMessage = "";
        public string FollowMessage = "";
        public int MinBits = 0;
        public bool AutoShoutout = false;
        public int AutoShoutoutTime = 10;
        public string ShoutoutAccess = "";
        public float GlobalVolume = 1.0f;
        public Style Styles;
        public Dictionary<string, List<string>> Activities;
        public Enabled Enabled;
        public Dictionary<string, Command> Commands;
        public Dictionary<string, List<string>> Messages;
        public Dictionary<string, Pose> PoseMapping;
        public List<string> Bots;
        public List<ScheduledMessage> ScheduledMessages;
        public List<Bits> CustomBits;
        public List<Subs> CustomSubs;
        public List<Rewards> CustomRewards;
        public List<Trigger> Watchdog;
    }

    public class Enabled
    {
        public bool new_chatter = true;
        public bool greet = true;
        public bool follow = true;
        public bool raid = true;
        public bool host = true;
        public bool autohost = true;
        public bool sub = true;
        public bool resub = true;
        public bool subgift = true;
        public bool anonsubgift = true;
        public bool bits = true;
        public bool lurk = true;
        public bool shoutout = true;
    }

    public class Style
    {
        public string BackgroundColor = "";
        public string BorderColor = "";
        public int BorderWidth = 0;
        public int BorderRadius = 0;
        public string BorderStrokeColor = "";
        public string TextFontFamily = "";
        public int TextSize = 0;
        public int TextWeight = 0;
        public string TextColor = "";
        public int HighlightTextSize = 0;
        public int HighlightTextSpacing = 0;
        public string HighlightTextColor = "";
        public string HighlightTextStrokeColor = "";
        public string HighlightTextShadowColor = "";
        public int HighlightTextShadowOffset = 0;
    }

    public class MascotData
    {
        public Dictionary<string, MascotImage> mascotImages;
        public Dictionary<string, MascotAudio> mascotAudio;
        public MascotStyle mascotStyles;
    }

    public class Settings
    {
        //public static string  TwitchChannel = "";
        //public static string TwitchOAUTH = "";
        //public static string TwitchBotChannel = "";
        //public static string TwitchBotOAUTH = "";
        //public static string ClientOAuth = "rjhahash2xokt6ixmn3qwug1yk7ac7";
        //public static bool UseChatbot = false;
        //public static string CurrentMascot = "";
        //public static string AlignMascot = "";
        //public static string HostMessage = "";
        //public static string AutohostMessage = "";
        //public static string FollowMessage = "";
        //public static int MinBits = 0;
        //public static bool AutoShoutout = false;
        //public static int AutoShoutoutTime = 10;
        //public static string ShoutoutAccess = "mod";
        //public static double GlobalVolume = 0.2;
        //bool    NanoleafEnabled = false;
        //string  NanoleafIP = "";
        //string  NanoleafToken = "";
        //bool    HueEnabled = false;
        //string  HueIP = "";
        //string  HueToken = "";
        //bool    YeelightEnabled = false;

        //Dictionary<string, string> Styles = new Dictionary<string, string>();
        //Dictionary<string, List<string>> Activities = new Dictionary<string, List<string>>();
        //Dictionary<string, bool> Enabled = new Dictionary<string, bool>();
        //Dictionary<string, Command> Commands = new Dictionary<string, Command>();
        //Dictionary<string, List<string>> Messages = new Dictionary<string, List<String>>();
        //Dictionary<string, Pose> PoseMapping = new Dictionary<string, Pose>();

        //List<string> Bots = new List<string>();
        //List<ScheduledMessage> ScheduledMessages = new List<ScheduledMessage>();

        //List<Bits> CustomBits = new List<Bits>();
        //List<Subs> CustomSubs = new List<Subs>();
        //List<Trigger> Watchdog = new List<Trigger>();
        //Dictionary<string, int> scheduleTable = new Dictionary<string, int>();
        //int scheduleLines = 0;

        public static JsonData json_settings;
        public static MascotData mascot_data;
        public static string twitch_client_id = "";
        public static List<string> commonBots = new List<string>() {"nightbot", "streamlabs", "streamelements", "stay_hydrated_bot", "botisimo", "wizebot",
                           "moobot" };

        public static void Save()
        {
            File.Delete("settings.bak");
            File.Move("settings.json", "settings.bak");
            using (StreamWriter r = new StreamWriter("settings.json"))
            {
                string jsonString = JsonConvert.SerializeObject(json_settings, Formatting.Indented);
                r.Write(jsonString);
                r.Flush();

            }
            //string jsonString = JsonConvert.SerializeObject(json_settings, Formatting.Indented);
        }

        public Settings()
        {
            using (StreamReader r = new StreamReader("settings.json"))
            {
                string jsonString = r.ReadToEnd();


                //dynamic array = JsonConvert.DeserializeObject(jsonString);

                json_settings = JsonConvert.DeserializeObject<JsonData>(jsonString);

                ///TODO: Update json file when changing oauth
                //string s = JsonConvert.SerializeObject(d, Formatting.Indented);
                //StreamWriter w = File.CreateText("newSettings.json");

                //JsonSerializer serializer = new JsonSerializer();
                //serializer.Formatting = Formatting.Indented;
                //serializer.Serialize(w, d);
                //w.Flush();

                //json_settings.Bots.AddRange(commonBots);


                //w.WriteLine(s);
                {
                    //foreach (var item in array)
                    //{
                    //    //Console.WriteLine("{0} {1}", item.Name, item.Value);
                    //    if(item.Name == "TwitchChannel")
                    //    {
                    //        TwitchChannel = item.Value;
                    //    }
                    //    else if(item.Name == "TwitchOAUTH")
                    //    {
                    //        TwitchOAUTH = item.Value;
                    //    }
                    //    else if(item.Name == "TwitchBotChannel")
                    //    {
                    //        TwitchBotChannel = item.Value;
                    //    }
                    //    else if(item.Name == "TwitchBotOAUTH")
                    //    {
                    //        TwitchBotOAUTH = item.Value;
                    //    }
                    //    else if(item.Name == "ClientOAuth")
                    //    {
                    //        ClientOAuth = item.Value;
                    //    }
                    //    else if(item.Name == "UseChatbot")
                    //    {
                    //        UseChatbot = (item.Value.ToString().ToLower()) == "true" ? true:false;
                    //    }
                    //    else if(item.Name == "CurrentMascot")
                    //    {
                    //        CurrentMascot = item.Value;
                    //    }
                    //    else if(item.Name == "AlignMascot")
                    //    {
                    //        AlignMascot = item.Value;
                    //    }
                    //    else if(item.Name == "HostMessage")
                    //    {
                    //        HostMessage = item.Value;
                    //    }
                    //    else if(item.Name == "AutohostMessage")
                    //    {
                    //        AutohostMessage = item.Value;
                    //    }
                    //    else if(item.Name == "FollowMessage")
                    //    {
                    //        FollowMessage = item.Value;
                    //    }
                    //    else if(item.Name == "MinBits")
                    //    {
                    //        MinBits = int.Parse(item.Value.ToString());
                    //    }
                    //    else if(item.Name == "AutoShoutout")
                    //    {
                    //        AutoShoutout = (item.Value.ToString().ToLower()) == "true" ? true : false;
                    //    }
                    //    else if (item.Name == "AutoShoutoutTime")
                    //    {
                    //        AutoShoutoutTime = int.Parse(item.Value.ToString());
                    //    }
                    //    else if (item.Name == "ShoutoutAccess")
                    //    {
                    //        ShoutoutAccess = item.Value;
                    //    }
                    //    else if (item.Name == "GlobalVolume")
                    //    {
                    //        GlobalVolume = double.Parse(item.Value.ToString());
                    //    }
                    //    else if (item.Name == "Styles")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic styleArray = JsonConvert.DeserializeObject(value);
                    //        foreach (var style in styleArray)
                    //        {
                    //            Styles.Add(style.Name, style.Value.ToString());
                    //        }
                    //    }
                    //    else if (item.Name == "Activities")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic activityArray = JsonConvert.DeserializeObject(value);
                    //        foreach (var activity in activityArray)
                    //        {
                    //            var val = activity.Value.First;
                    //            Activities.Add(activity.Name, new List<string>());
                    //            while(val != null)
                    //            {
                    //                Activities[activity.Name].Add(val.ToString());
                    //                val = val.Next;
                    //            }
                    //        }
                    //    }
                    //    else if (item.Name == "Enabled")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic enabledArray = JsonConvert.DeserializeObject(value);
                    //        foreach (var enabled in enabledArray)
                    //        {
                    //            Enabled.Add(enabled.Name, (enabled.Value.ToString().ToLower()) == "true" ? true : false);
                    //        }
                    //    }
                    //    else if (item.Name == "Commands")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic commandArray = JsonConvert.DeserializeObject(value);
                    //        foreach (var command in commandArray)
                    //        {
                    //            Command newCommand = new Command();
                    //            var comm = command.First;
                    //            newCommand.Image = comm["Image"] == null ? "": comm["Image"];
                    //            newCommand.Script = comm["Script"] == null ? "" : comm["Script"];
                    //            newCommand.Enabled = comm["Enabled"] == null ? true : (comm["Enabled"].ToString().ToLower() == "true" ? true : false);
                    //            newCommand.ViewerOnce = comm["ViewerOnce"] == null ? true : (comm["ViewerOnce"].ToString().ToLower() == "true" ? true : false);
                    //            newCommand.ViewerTimeout = comm["ViewerTimeout"] == null ? 0 : int.Parse(comm["ViewerTimeout"].ToString());
                    //            newCommand.GlobalTimeout = comm["GlobalTimeout"] == null ? 0 : int.Parse(comm["GlobalTimeout"].ToString());
                    //            newCommand.Access = comm["Access"] == null ? "" : comm["Access"];

                    //            var Alias = comm["Aliases"].First;
                    //            while (Alias != null)
                    //            {
                    //                newCommand.Aliases.Add(Alias.Value);
                    //                Alias = Alias.Next;
                    //            }

                    //            var Key = comm["Hotkey"].First;
                    //            while (Key != null)
                    //            {
                    //                newCommand.Hotkey.Add(Key.Value);
                    //                Key = Key.Next;
                    //            }

                    //            Commands.Add(command.Name, newCommand);
                    //        }
                    //    }
                    //    else if (item.Name == "Messages")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic messageArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var message in messageArray)
                    //        {
                    //            Messages.Add(message.Name, new List<string>());
                    //            var text = message.First.First;
                    //            while (text != null)
                    //            {
                    //                Messages[message.Name].Add(text.Value);
                    //                text = text.Next;
                    //            }
                    //        }
                    //    }
                    //    else if (item.Name == "PoseMapping")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic poseArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var pose in poseArray)
                    //        {
                    //            Pose newPose = new Pose();
                    //            //var poseConfig = pose.First;
                    //            newPose.Image = pose.First["Image"];
                    //            newPose.Audio = pose.First["Audio"];
                    //            PoseMapping.Add(pose.Name, newPose);
                    //        }

                    //    }
                    //    else if (item.Name == "Bots")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic botsArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var bot in botsArray)
                    //        {
                    //            Bots.Add(bot.Value);
                    //        }

                    //    }
                    //    else if (item.Name == "ScheduledMessages")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic messArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var schedMess in messArray)
                    //        {
                    //            ScheduledMessage newMess = new ScheduledMessage();
                    //            newMess.Name = schedMess["Name"];
                    //            newMess.Timer = int.Parse(schedMess["Timer"].ToString());
                    //            newMess.MinLines = int.Parse(schedMess["MinLines"].ToString());
                    //            newMess.Enabled = schedMess["Enabled"].ToString().ToLower() == "true" ? true : false;
                    //            newMess.Command = schedMess["Command"];
                    //            newMess.Image = schedMess["Image"];

                    //            ScheduledMessages.Add(newMess);
                    //        }
                    //    }
                    //    else if (item.Name == "CustomBits")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic bitsArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var bit in bitsArray)
                    //        {
                    //            Bits newBits = new Bits();
                    //            newBits.Name = bit["Name"];
                    //            newBits.From = int.Parse(bit["From"].ToString());
                    //            newBits.To = int.Parse(bit["To"].ToString());

                    //            CustomBits.Add(newBits);
                    //        }
                    //    }
                    //    else if (item.Name == "CustomSubs")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic subsArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var sub in subsArray)
                    //        {
                    //            Subs newSub = new Subs();
                    //            newSub.Name = sub["Name"];
                    //            newSub.Tier = sub["Tier"];
                    //            newSub.From = int.Parse(sub["From"].ToString());
                    //            newSub.To = int.Parse(sub["To"].ToString());

                    //            CustomSubs.Add(newSub);
                    //        }
                    //    }
                    //    else if (item.Name == "Watchdog")
                    //    {
                    //        dynamic value = item.Value.ToString();
                    //        dynamic triggerArray = JsonConvert.DeserializeObject(value);

                    //        foreach (var trigger in triggerArray)
                    //        {
                    //            Trigger newTrigger = new Trigger();
                    //            newTrigger.Name = trigger["Name"];
                    //            newTrigger.Filename = trigger["Filename"];
                    //            newTrigger.Enabled = trigger["Enabled"].ToString().ToLower() == "true" ? true : false;
                    //            newTrigger.Command = trigger["Command"];
                    //            newTrigger.Message = trigger["Message"];
                    //            newTrigger.Image = trigger["Image"];

                    //            Watchdog.Add(newTrigger);
                    //        }
                    //    }
                    //}
                }
                //Console.WriteLine(jsonString);
            }


            using (StreamReader r = new StreamReader("mascots//" + json_settings.CurrentMascot + "//mascot.json"))
            {
                string jsonString = r.ReadToEnd();
                mascot_data = JsonConvert.DeserializeObject<MascotData>(jsonString);
            }
        }

        public string GetClientID()
        {
            return twitch_client_id;
        }

        public string GetClientOAuth()
        {
            return json_settings.ClientOAuth;
        }
    }
}
