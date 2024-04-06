using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace streamerCompanion
{
    public class TwitchGetUserResponse
    {
        public int id = 0;
        public string profile_image_url = "";
        public string display_name = "";
    }

    class TwClient
    {
        public static TwitchClient Client;
        public static TwitchPubSub PubSubClient;
        public static TwitchAPI API;
        public static LiveStreamMonitorService Monitor;
        private static Random random = new Random();
        private static char separator = Path.DirectorySeparatorChar;

        private static Dictionary<string, bool> FollowedUsers = new Dictionary<string, bool>();
        private static Dictionary<string, bool> GreetedUsers = new Dictionary<string, bool>();
        private static List<string> LurkingUsers = new List<string>();
        private static Dictionary<string, List<string>> CommandUsers = new Dictionary<string, List<string>>();
        private static Dictionary<string, long> CommandTimeout = new Dictionary<string, long>();
        private static DateTime dateDate = new DateTime();

        private static string ClientId;
        public static string AccessToken;
        public static string ClientSecret;

        private static string BotChannel;
        private static string BotOAuth;

        private static string ChannelID;
        private static string ChannelName;

        //private static 

        //private static string ChannelID = "45222658";
        //private static string ChannelName = "giusecc";


        private static void CheckOAuth(Object stateInfo)
        {
            var tmp = AccessToken;
            AccessToken = Twitch.TwitchGetOauth(AccessToken, ClientId, ClientSecret).Result;
            if(tmp != AccessToken)
            {
                Settings.json_settings.ClientOAuth = AccessToken;
                Settings.Save();
            }
            
        }


        private static async Task ConfigLiveMonitorAsync()
        {
            Monitor = new LiveStreamMonitorService(API, 60);

            List<string> lst = new List<string> {ChannelID};
            Monitor.SetChannelsById(lst);

            Monitor.OnStreamOnline += Monitor_OnStreamOnline;
            Monitor.OnStreamOffline += Monitor_OnStreamOffline;
            //Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;

            Monitor.Start(); //Keep at the end!

            await Task.Delay(-1);

        }

        public static void ClientConnect()
        {
            InitializeClient();
            while(true)
            {
                Thread.Sleep(500);
            }
        }

        public static void InitializeClient()
        {
            ClientId = Settings.json_settings.ClientId;
            ClientSecret = Settings.json_settings.ClientSecret;
            AccessToken = Settings.json_settings.ClientOAuth;
            AccessToken = Twitch.TwitchGetOauth(AccessToken, ClientId, ClientSecret).Result;
            Settings.json_settings.ClientOAuth = AccessToken;
            Settings.Save();

            Timer OAuthTimer = new Timer(CheckOAuth, new AutoResetEvent(false), 0, 100000);


            BotChannel = Settings.json_settings.TwitchBotChannel;
            BotOAuth = Settings.json_settings.TwitchOAUTH;

            ChannelName = Settings.json_settings.TwitchChannel;
            string jsonString = Twitch.TwitchGetUser(AccessToken, ClientId, ChannelName).Result;
            if (jsonString != "")
            {
                var user_data = JsonConvert.DeserializeObject<TwitchGetUserResponse>(jsonString);
                ChannelID = user_data.id.ToString();
            }

            foreach (var user in Settings.json_settings.Bots)
            {
                jsonString = Twitch.TwitchGetUser(AccessToken, ClientId, user).Result;
                if (jsonString == "") continue;
                var us_data = JsonConvert.DeserializeObject<TwitchGetUserResponse>(jsonString);
                GreetedUsers.Add(us_data.id.ToString(), true);
            }
            if(ChannelID == null)
            {
                return;
            }
            if(ChannelID != null)
                GreetedUsers.Add(ChannelID, true);
            GreetedUsers.Add("-1", true);

            //-------Initialize Client-------

            

            ConnectionCredentials credentials = new ConnectionCredentials(BotChannel, BotOAuth);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials, ChannelName);

            Client.OnLog += Client_OnLog;
            //Client.OnJoinedChannel += Client_OnJoinedChannel;
            Client.OnMessageReceived += Client_OnMessageReceived;
            //Client.OnWhisperReceived += Client_OnWhisperReceived;
            Client.OnNewSubscriber += Client_OnNewSubscriber;
            Client.OnConnected += Client_OnConnected;
            Client.OnDisconnected += Client_OnDisconnected;
            Client.OnReSubscriber += Client_OnReSubscriber;
            Client.OnGiftedSubscription += Client_OnGiftedSubscription;
            Client.OnPrimePaidSubscriber += Client_OnPrimePaidSubscriber;
            Client.OnContinuedGiftedSubscription += Client_OnContinuedGiftedSubscription;
            Client.OnRaidNotification += Client_OnRaidNotification;

            Client.Connect();

            //-------Initialize PubSub-------

            PubSubClient = new TwitchPubSub();
            PubSubClient.OnPubSubServiceConnected += onPubSubServiceConnected;
            PubSubClient.OnBitsReceivedV2 += onBitsReceivedV2;
            PubSubClient.OnListenResponse += onListenResponse;
            PubSubClient.OnStreamUp += onStreamUp;
            PubSubClient.OnStreamDown += onStreamDown;
            PubSubClient.OnFollow += onFollow;
            PubSubClient.OnChannelPointsRewardRedeemed += onChannelPoints;

            PubSubClient.ListenToVideoPlayback(ChannelName);
            //PubSubClient.ListenToBitsEventsV2("476077919");
            //PubSubClient.ListenToFollows("45222658");
            PubSubClient.ListenToFollows(ChannelID);
            //PubSubClient.ListenToChannelPoints(ChannelID);


            PubSubClient.Connect();

            //-------Initialize API-------
            API = new TwitchAPI();
            //TODO: MOVE TO CONFIG FILE
            API.Settings.ClientId = ClientId;
            API.Settings.AccessToken = AccessToken;
            API.Settings.Secret = ClientSecret;

            
            var streams = API.Helix.Streams.GetStreamsAsync(null, null, 1, null, null, "all", new List<string> { ChannelID });
            streams.Wait();
            if (streams.Result.Streams.Length > 0)
            {
                Form1.LiveStatus = true;
            }
            else
            {
                Form1.LiveStatus = false;
            }

            if (Client.IsConnected)
            {
                Form1.ChatStatus = true;
            }
            else
            {
                Form1.ChatStatus = false;
            }

            Task.Run(() => ConfigLiveMonitorAsync());
        }


        private static void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            Form1.LiveStatus = true;
        }
        private static void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
        {
            Form1.LiveStatus = false;
        }




        private static void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private static void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Form1.ChatStatus = true;
            //Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private static void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Form1.ChatStatus = false;
            //Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private static void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            //Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            //Client.SendMessage(e.Channel, "[" + DateTime.Now +"]" + " Hey guys! I am a bot connected via TwitchLib!");
        }


        


        //private string txt = "";
        private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            /*if(e.ChatMessage.CustomRewardId != null)
            {
                string reward = Twitch.TwitchGetReward(AccessToken, ClientId, ChannelID, e.ChatMessage.CustomRewardId).Result.ToLower();
                foreach (Rewards r in Settings.json_settings.CustomRewards)
                {
                    if (r.Name == reward)
                    {
                        Form1.FormLog.Add("[POINTS]\t" + reward + "\n");
                        JSONdataMess msg = new JSONdataMess();
                        if (Settings.json_settings.Messages.ContainsKey(reward))
                            msg.data.message = RandomizeString(Settings.json_settings.Messages[reward][new Random().Next(0, Settings.json_settings.Messages[reward].Count - 1)]);
                        string img = RandomizeString(Settings.json_settings.PoseMapping[reward].Image);
                        msg.data.sender = e.ChatMessage.Username;
                        msg.data.mascot = "file:///" + Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}" + Settings.mascot_data.mascotImages[img].Image;
                        msg.data.mascotmouth = Settings.mascot_data.mascotImages[img].MouthHeight;
                        msg.data.time = Settings.mascot_data.mascotImages[img].Time;
                        msg.data.image = "";
                        msg.Event = "EVENT_STREAMERCOMPANION";

                        Overlay.MessageQueue.Add(msg);
                        return;
                    }
                }

                return;
            }*/

            //TODO: COMMAND PARSER
            if (e.ChatMessage.Message.StartsWith("!") && e.ChatMessage.Message.Length > 1)
            {
                string Command = e.ChatMessage.Message.Split('!')[1].Split(' ')[0].ToLower();
                string CommandParameter = "";
                
                if(e.ChatMessage.Message.Split(' ').Length > 1)
                    CommandParameter = e.ChatMessage.Message.Split(' ')[1].ToLower(); //TODO FIX
                

                if(Command == "so" || Command == "shoutout")
                {

                    if (!Settings.json_settings.Enabled.shoutout) return;
                    Form1.FormLog.Add("[SHOUTOUT]\t" + CommandParameter + "\n");
                    switch (Settings.json_settings.ShoutoutAccess.ToLower())
                    {
                        case "sub":
                            if (e.ChatMessage.IsSubscriber == false && e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "vip":
                            if (e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "mod":
                            if (e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "broadcaster":
                            if (e.ChatMessage.IsBroadcaster == false) return;
                            break;
                    }
                    if (CommandParameter == "") return;

                    if (CommandParameter.Contains("@"))
                    {
                        CommandParameter = CommandParameter.Split('@')[1];
                    }

                    //string activity = Twitch.TwitchGetLastActivity(AccessToken, ClientId, CommandParameter).Result;
                    string jsonString = Twitch.TwitchGetUser(AccessToken, ClientId, CommandParameter).Result;
                    if (jsonString == "") return;
                    var user_data = JsonConvert.DeserializeObject<TwitchGetUserResponse>(jsonString);

                    JSONdataMess msg = new JSONdataMess();
                    GetMessage("shoutout", msg);
                    //msg.data.message = RandomizeString(Settings.json_settings.Messages["shoutout"][new Random().Next(0, Settings.json_settings.Messages["shoutout"].Count - 1)]);
                    msg.data.sender = e.ChatMessage.Username;

                    msg.data.recipient = CommandParameter;
                    //mess.data.activity = activity_data.game_name;
                    /*if(activity_data.game_name!="")
                    {
                        if(Settings.json_settings.Activities.ContainsKey(activity_data.game_name))
                        {

                        }
                    }*/
                    string imgg = RandomizeString(Settings.json_settings.PoseMapping["shoutout"].Image);
                    msg.data.mascot = "file:///"+ Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}" + Settings.mascot_data.mascotImages[imgg].Image;
                    msg.data.mascotmouth = Settings.mascot_data.mascotImages[imgg].MouthHeight;
                    msg.data.time = Settings.mascot_data.mascotImages[imgg].Time;
                    msg.data.image = "";
                    if (Settings.json_settings.PoseMapping.ContainsKey("shoutout_" + user_data.id))
                    {
                        imgg = RandomizeString(Settings.json_settings.PoseMapping["shoutout_" + user_data.id].Image);
                        msg.data.mascot = "file:///"+ Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}" + Settings.mascot_data.mascotImages[imgg].Image;
                        msg.data.mascotmouth = Settings.mascot_data.mascotImages[imgg].MouthHeight;
                        msg.data.time = Settings.mascot_data.mascotImages[imgg].Time;
                    }
                    msg.data.id = "shoutout";
                    msg.Event = "EVENT_STREAMERCOMPANION";

                    msg.data.volume = 0f;
                    msg.data.image = user_data.profile_image_url;

                    Overlay.MessageQueue.Add(msg);
                    return;
                }
                else if (Command == "lurk")
                {
                    if (!Settings.json_settings.Enabled.lurk) return;
                    if (LurkingUsers.Contains(e.ChatMessage.UserId)) return;
                    Form1.FormLog.Add("[LURK]\t\t" + Command + "\n");
                    JSONdataMess msg = new JSONdataMess();
                    msg.data.sender = e.ChatMessage.Username;
                    GetMascotAnimation("lurk", msg);
                    GetMessage("lurk", msg);
                    //msg.data.message = RandomizeString(Settings.json_settings.Messages["lurk"][new Random().Next(0, Settings.json_settings.Messages["lurk"].Count - 1)]);
                    msg.Event = "EVENT_STREAMERCOMPANION";
                    msg.data.volume = 0f;
                    LurkingUsers.Add(e.ChatMessage.UserId);
                    Overlay.MessageQueue.Add(msg);
                }
                else if (Command == "greet")
                {
                    if (!Settings.json_settings.Enabled.shoutout) return;
                    
                    switch (Settings.json_settings.ShoutoutAccess.ToLower())
                    {
                        case "sub":
                            if (e.ChatMessage.IsSubscriber == false && e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "vip":
                            if (e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "mod":
                            if (e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                            break;
                        case "broadcaster":
                            if (e.ChatMessage.IsBroadcaster == false) return;
                            break;
                    }

                    if (CommandParameter == "") return;
                    Form1.FormLog.Add("[GREET]\t\t" + CommandParameter + "\n");
                    if (CommandParameter.Contains("@"))
                    {
                        CommandParameter = CommandParameter.Split('@')[1];
                    }

                    string jsonString = Twitch.TwitchGetUser(AccessToken, ClientId, CommandParameter).Result;
                    if (jsonString == "") return;

                    var user_data = JsonConvert.DeserializeObject<TwitchGetUserResponse>(jsonString);

                    JSONdataMess msg = new JSONdataMess();
                    msg.data.sender = user_data.display_name;

                    msg.data.recipient = CommandParameter;
                    GetMascotAnimation("greet", msg);
                    GetMessage("greet", msg);
                    //msg.data.message = Settings.json_settings.Messages["greet"][0];
                    msg.data.image = "";
                    if (Settings.json_settings.PoseMapping.ContainsKey("viewer_" + user_data.id))
                    {
                        GetMascotAnimation("viewer_" + user_data.id, msg);
                        GetMessage("viewer_" + user_data.id, msg);
                        //msg.data.message = Settings.json_settings.Messages["viewer_" + user_data.id][new Random().Next(0, Settings.json_settings.Messages["viewer_" + user_data.id].Count - 1)];
                    }
                    msg.Event = "EVENT_STREAMERCOMPANION";

                    msg.data.volume = 0f;

                    Overlay.MessageQueue.Add(msg);
                }
                if (!Settings.json_settings.Commands.ContainsKey("!" + Command)) return;
                if (!Settings.json_settings.Commands["!" + Command].Enabled) return;
                Form1.FormLog.Add("[COMMAND]\t" + Command + "\n");
                string access = Settings.json_settings.Commands["!" + Command].Access;
                switch (access)
                {
                    case "sub":
                        if (e.ChatMessage.IsSubscriber == false && e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                        break;
                    case "vip":
                        if (e.ChatMessage.IsVip == false && e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                        break;
                    case "mod":
                        if (e.ChatMessage.IsModerator == false && e.ChatMessage.IsBroadcaster == false) return;
                        break;
                    case "broadcaster":
                        if (e.ChatMessage.IsBroadcaster == false) return;
                        break;
                }

                if (!CommandUsers.ContainsKey(Command)) CommandUsers.Add(Command, new List<string>());
                if (Settings.json_settings.Commands["!" + Command].ViewerOnce)
                {
                    if (CommandUsers[Command].Contains(e.ChatMessage.UserId)) return;
                    CommandUsers[Command].Add(e.ChatMessage.UserId);
                }

                if (Settings.json_settings.Commands["!" + Command].ViewerTimeout > 0)
                {
                    long result = dateDate.Year * 10000000000 + dateDate.Month * 100000000 + dateDate.Day * 1000000 + dateDate.Hour * 10000 + dateDate.Minute * 100 + dateDate.Second;
                    if (!CommandTimeout.ContainsKey(Command + e.ChatMessage.UserId))
                    {
                        CommandTimeout.Add(Command + e.ChatMessage.UserId, result);
                    }
                    else
                    {
                        if (result - CommandTimeout[Command + e.ChatMessage.UserId] > Settings.json_settings.Commands["!" + Command].ViewerTimeout)
                        {
                            CommandTimeout.Add(Command + e.ChatMessage.UserId, result);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                if (Settings.json_settings.Commands["!" + Command].GlobalTimeout > 0)
                {
                    long result = dateDate.Year * 10000000000 + dateDate.Month * 100000000 + dateDate.Day * 1000000 + dateDate.Hour * 10000 + dateDate.Minute * 100 + dateDate.Second;
                    if (!CommandTimeout.ContainsKey(Command))
                    {
                        CommandTimeout.Add(Command, result);
                    }
                    else
                    {
                        if (result - CommandTimeout[Command] > Settings.json_settings.Commands["!" + Command].GlobalTimeout)
                        {
                            CommandTimeout.Add(Command, result);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                JSONdataMess mess = new JSONdataMess();
                mess.data.image = "";
                if (Settings.json_settings.Commands["!" + Command].Image != "")
                {
                    mess.data.image = "//images//" + Settings.json_settings.Commands["!" + Command].Image;

                }

                GetMascotAnimation("!" + Command, mess);




                if (Settings.json_settings.Messages.ContainsKey("!" + Command))
                    GetMessage("!" + Command, mess);
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages["!" + Command][new Random().Next(0, Settings.json_settings.Messages["!" + Command].Count - 1)]);
                mess.data.sender = e.ChatMessage.DisplayName;
                mess.Event = "EVENT_STREAMERCOMPANION";
                Overlay.MessageQueue.Add(mess);
            }

            if (e.ChatMessage.Bits > 0)
            {
                Form1.FormLog.Add("[BITS]\t\t[" + e.ChatMessage.Bits + "] " + e.ChatMessage.Username + "\n");

                JSONdataMess mess = new JSONdataMess();

                GetMascotAnimation("bits", mess);
                GetMessage("bits", mess);
                //mess.data.message = RandomizeString(Settings.json_settings.Messages["bits"][new Random().Next(0, Settings.json_settings.Messages["bits"].Count - 1)]);

                mess.data.sender = e.ChatMessage.DisplayName;
                mess.data.bits = e.ChatMessage.Bits.ToString();
                
                foreach (var customBit in Settings.json_settings.CustomBits)
                {
                    if (e.ChatMessage.Bits >= customBit.From && e.ChatMessage.Bits <= customBit.To)
                    {
                        GetMascotAnimation(customBit.Name, mess);
                        GetMessage(customBit.Name, mess);
                        //mess.data.message = RandomizeString(Settings.json_settings.Messages[customBit.Name][new Random().Next(0, Settings.json_settings.Messages[customBit.Name].Count - 1)]);

                        break;
                    }
                }

                mess.Event = "EVENT_STREAMERCOMPANION";

                Overlay.MessageQueue.Add(mess);

            }
            //txt = e.ChatMessage.Message;
            if (!GreetedUsers.ContainsKey(e.ChatMessage.UserId))
            {
                GreetedUsers.Add(e.ChatMessage.UserId, true);
                Form1.FormLog.Add("[GREET]\t\t" + e.ChatMessage.Username + "\n");
                JSONdataMess mess = new JSONdataMess();

                mess.data.sender = e.ChatMessage.DisplayName;


                GetMascotAnimation("greet", mess);
                GetMessage("greet", mess);
                //mess.data.message = RandomizeString(Settings.json_settings.Messages["greet"][new Random().Next(0, Settings.json_settings.Messages["greet"].Count - 1)]);
                mess.data.image = "";

                if (e.ChatMessage.IsVip == true)
                {
                    GetMascotAnimation("VIP_DEFAULT", mess);
                    GetMessage("VIP_DEFAULT", mess);
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages["VIP_DEFAULT"][new Random().Next(0, Settings.json_settings.Messages["VIP_DEFAULT"].Count - 1)]);
                }

                if (Settings.json_settings.PoseMapping.ContainsKey("viewer_" + e.ChatMessage.UserId))
                {
                    GetMascotAnimation("viewer_" + e.ChatMessage.UserId, mess);
                    GetMessage("viewer_" + e.ChatMessage.UserId, mess);
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages["viewer_" + e.ChatMessage.UserId][new Random().Next(0, Settings.json_settings.Messages["viewer_" + e.ChatMessage.UserId].Count - 1)]);
                }
                mess.Event = "EVENT_STREAMERCOMPANION";
                Overlay.MessageQueue.Add(mess);
            }
        }

        private static string RandomizeString(string message)
        {
            string mess = message;
            while (mess.Contains('['))
            {
                var tmp = mess.Split(']')[0].Split('[')[1];
                var tmpArr = tmp.Split(';');
                tmp = tmpArr[random.Next(0, tmpArr.Length)];
                mess = mess.Split('[')[0] + tmp + mess.Split(new[] { ']' }, 2)[1];
            }
            return mess;
        }

        private static void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    Client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private static void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            Form1.FormLog.Add("[SUB]\t\t" + e.Subscriber.DisplayName + "\n");
            JSONdataMess mess = new JSONdataMess();
            
            string sub_tier = "";
            switch (e.Subscriber.SubscriptionPlan)
            {
                case TwitchLib.Client.Enums.SubscriptionPlan.Prime:
                    sub_tier = "prime";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier1:
                    sub_tier = "1";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier2:
                    sub_tier = "2";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier3:
                    sub_tier = "3";
                    break;
            }

            GetMascotAnimation("sub", mess);
            GetMessage("sub", mess);
            mess.data.months = e.Subscriber.MsgParamCumulativeMonths;
            mess.data.sender = e.Subscriber.DisplayName;
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["sub"][new Random().Next(0, Settings.json_settings.Messages["sub"].Count - 1)]);


            foreach (var customSub in Settings.json_settings.CustomSubs)
            {
                if (customSub.Tier == sub_tier && int.Parse(e.Subscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.Subscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.Subscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.Subscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);

                    break;
                }
                else if(customSub.Tier == "" && int.Parse(e.Subscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.Subscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.Subscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.Subscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);
                    break;
                }
            }

            mess.Event = "EVENT_STREAMERCOMPANION";

            Overlay.MessageQueue.Add(mess);
            //mess.data.
        }

        private static void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            Form1.FormLog.Add("[RESUB]\t\t" + e.ReSubscriber.DisplayName + "\n");

            JSONdataMess mess = new JSONdataMess();

            string sub_tier = "";
            switch (e.ReSubscriber.SubscriptionPlan)
            {
                case TwitchLib.Client.Enums.SubscriptionPlan.Prime:
                    sub_tier = "prime";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier1:
                    sub_tier = "1";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier2:
                    sub_tier = "2";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier3:
                    sub_tier = "3";
                    break;
            }
            GetMascotAnimation("sub", mess);
            GetMessage("resub", mess);
            mess.data.months = e.ReSubscriber.MsgParamCumulativeMonths;
            mess.data.sender = e.ReSubscriber.DisplayName;
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["resub"][new Random().Next(0, Settings.json_settings.Messages["resub"].Count - 1)]);


            foreach (var customSub in Settings.json_settings.CustomSubs)
            {
                if (customSub.Tier == sub_tier && int.Parse(e.ReSubscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.ReSubscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.ReSubscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.ReSubscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);
                    break;
                }
                else if (customSub.Tier == "" && int.Parse(e.ReSubscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.ReSubscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.ReSubscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.ReSubscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);

                    break;
                }
            }

            mess.Event = "EVENT_STREAMERCOMPANION";

            Overlay.MessageQueue.Add(mess);

        }

        private static void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            Form1.FormLog.Add("[GIFTSUB]\t" + e.GiftedSubscription.MsgParamRecipientDisplayName + "\n");

            JSONdataMess mess = new JSONdataMess();

            GetMascotAnimation("subgift", mess);
            GetMessage("subgift", mess);
            mess.data.months = e.GiftedSubscription.MsgParamMonths;
            mess.data.sender = e.GiftedSubscription.DisplayName;
            mess.data.recipient = e.GiftedSubscription.MsgParamRecipientUserName;
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["subgift"][new Random().Next(0, Settings.json_settings.Messages["subgift"].Count - 1)]);

            mess.Event = "EVENT_STREAMERCOMPANION";
            Overlay.MessageQueue.Add(mess);
        }

        private static void Client_OnPrimePaidSubscriber(object sender, OnPrimePaidSubscriberArgs e)
        {
            Form1.FormLog.Add("[PRSUB]\t\t" + e.PrimePaidSubscriber.DisplayName + "\n");


            JSONdataMess mess = new JSONdataMess();

            string sub_tier = "";
            switch (e.PrimePaidSubscriber.SubscriptionPlan)
            {
                case TwitchLib.Client.Enums.SubscriptionPlan.Prime:
                    sub_tier = "prime";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier1:
                    sub_tier = "1";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier2:
                    sub_tier = "2";
                    break;
                case TwitchLib.Client.Enums.SubscriptionPlan.Tier3:
                    sub_tier = "3";
                    break;
            }
            GetMascotAnimation("sub", mess);
            GetMessage("sub", mess);
            mess.data.months = e.PrimePaidSubscriber.MsgParamCumulativeMonths;
            mess.data.sender = e.PrimePaidSubscriber.DisplayName;
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["sub"][new Random().Next(0, Settings.json_settings.Messages["sub"].Count - 1)]);


            foreach (var customSub in Settings.json_settings.CustomSubs)
            {
                if (customSub.Tier == sub_tier && int.Parse(e.PrimePaidSubscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.PrimePaidSubscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.PrimePaidSubscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.PrimePaidSubscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);
                    break;
                }
                else if (customSub.Tier == "" && int.Parse(e.PrimePaidSubscriber.MsgParamCumulativeMonths) >= customSub.From && int.Parse(e.PrimePaidSubscriber.MsgParamCumulativeMonths) <= customSub.To)
                {
                    GetMascotAnimation(customSub.Name, mess);
                    GetMessage(customSub.Name, mess);
                    mess.data.months = e.PrimePaidSubscriber.MsgParamCumulativeMonths;
                    mess.data.sender = e.PrimePaidSubscriber.DisplayName;
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customSub.Name][new Random().Next(0, Settings.json_settings.Messages[customSub.Name].Count - 1)]);

                    break;
                }
            }

            mess.Event = "EVENT_STREAMERCOMPANION";

            Overlay.MessageQueue.Add(mess);

        }

        private static void Client_OnContinuedGiftedSubscription(object sender, OnContinuedGiftedSubscriptionArgs e)
        {
            Form1.FormLog.Add("[CONTSUB]\t\t" + e.ContinuedGiftedSubscription.DisplayName + "\n");
            //TODO: CHECK
        }

        private static void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            Form1.FormLog.Add("[RAID]\t\t" + e.RaidNotification.DisplayName + "\n");

            JSONdataMess mess = new JSONdataMess();
            GetMascotAnimation("raid", mess);
            GetMessage("raid", mess);
            mess.data.sender = e.RaidNotification.DisplayName;
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["raid"][new Random().Next(0, Settings.json_settings.Messages["raid"].Count - 1)]);


            mess.Event = "EVENT_STREAMERCOMPANION";

            Overlay.MessageQueue.Add(mess);
        }

        private static void onPubSubServiceConnected(object sender, EventArgs e)
        {
            // SendTopics accepts an oauth optionally, which is necessary for some topics
            PubSubClient.SendTopics("oauth:"+ AccessToken);
        }

        private static void onListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Response}");
        }

        private static void onStreamUp(object sender, OnStreamUpArgs e)
        {
            Console.WriteLine($"Stream just went up! Play delay: {e.PlayDelay}, server time: {e.ServerTime}");
            Form1.LiveStatus = true;
        }

        private static void onStreamDown(object sender, OnStreamDownArgs e)
        {
            Console.WriteLine($"Stream just went down! Server time: {e.ServerTime}");
            Form1.LiveStatus = false;
        }

        private static void onFollow(object sender, OnFollowArgs e)
        {
            if (!FollowedUsers.ContainsKey(e.UserId))
            {
                Form1.FormLog.Add("[FOLLOW]\t" + e.Username + "\n");
                FollowedUsers.Add(e.UserId, true);

                JSONdataMess mess = new JSONdataMess();
                //var message = Settings.json_settings.Messages["follow"][random.Next(0, Settings.json_settings.Messages["follow"].Count)];
                GetMascotAnimation("greet", mess);
                GetMessage("follow", mess);
                //mess.data.message = RandomizeString(message);
                mess.data.sender = e.Username;
                mess.data.image = "";
                mess.Event = "EVENT_STREAMERCOMPANION";
                Overlay.MessageQueue.Add(mess);
            }

            /*FormLog.Add("[FOLLOW]\t" + e.Username+"\n");
            FollowedList.Add(e.UserId);*/
        }

        private static void onChannelPoints(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Form1.FormLog.Add("[POINTS]\t" + e.RewardRedeemed.Redemption.Id + "\n");
        }

        private static void onBitsReceivedV2(object sender, OnBitsReceivedV2Args e)
        {
            Form1.FormLog.Add("[BITS]\t\t[" + e.BitsUsed + "] " + e.UserName + "\n");

            JSONdataMess mess = new JSONdataMess();
            GetMascotAnimation("bits", mess);
            GetMessage("bits", mess);
            mess.data.sender = e.UserName;
            mess.data.bits = e.BitsUsed.ToString();
            //mess.data.message = RandomizeString(Settings.json_settings.Messages["bits"][new Random().Next(0, Settings.json_settings.Messages["bits"].Count - 1)]);

            foreach (var customBit in Settings.json_settings.CustomBits)
            {
                if (e.BitsUsed >= customBit.From && e.BitsUsed <= customBit.To)
                {
                    GetMascotAnimation(customBit.Name, mess);
                    GetMessage(customBit.Name, mess);
                    //mess.data.message = RandomizeString(Settings.json_settings.Messages[customBit.Name][new Random().Next(0, Settings.json_settings.Messages[customBit.Name].Count - 1)]);

                    break;
                }
            }

            mess.Event = "EVENT_STREAMERCOMPANION";

            Overlay.MessageQueue.Add(mess);
        }

        private static void GetMascotAnimation(string name, JSONdataMess mess)
        {
            if (!Settings.json_settings.PoseMapping.ContainsKey(name))
            {
                mess.data.mascot = "file:///" + Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}" + Settings.mascot_data.mascotImages[Settings.json_settings.PoseMapping["DEFAULT"].Image].Image;
                mess.data.mascotmouth = Settings.mascot_data.mascotImages[Settings.json_settings.PoseMapping["DEFAULT"].Image].MouthHeight;
                mess.data.time = Settings.mascot_data.mascotImages[Settings.json_settings.PoseMapping["DEFAULT"].Image].Time;
            }
            else
            {
                string img = RandomizeString(Settings.json_settings.PoseMapping[name].Image);
                mess.data.mascot = "file:///" + Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}" + Settings.mascot_data.mascotImages[img].Image;
                mess.data.mascotmouth = Settings.mascot_data.mascotImages[img].MouthHeight;
                mess.data.time = Settings.mascot_data.mascotImages[img].Time;
            }
        }

        private static void GetMessage(string name, JSONdataMess mess)
        {
            if(!Settings.json_settings.Messages.ContainsKey(name))
            {
                mess.data.message = "";
            }
            else
            {
                mess.data.message = RandomizeString(Settings.json_settings.Messages[name][new Random().Next(0, Settings.json_settings.Messages[name].Count - 1)]);
            }
            
        }
    }
}
