using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;

namespace streamerCompanion
{
    class CssStyles
    {
        [JsonProperty(".mascot|width")]
        public string MascotWidth;

        [JsonProperty(".mascot|left")]
        public string MascotLeft;

        [JsonProperty(".mascot|right")]
        public string MascotRight;

        [JsonProperty(".message|left")]
        public string MessageLeft;

        [JsonProperty(".message|right")]
        public string MessageRight;

        [JsonProperty(".message|transform-origin")]
        public string MessageTransformOrigin;

        [JsonProperty(".mainbox|text-align")]
        public string MainboxTextAlign;

        [JsonProperty(".message::after|display")]
        public string MessageAfterDisplay;

        [JsonProperty(".message div:first-child|display")]
        public string MessageDivFirstChildDisplay;

        [JsonProperty(".message::before|display")]
        public string MessageBeforeDisplay;

        [JsonProperty(".message div:last-child|display")]
        public string MessageDivLastChildDisplay;

        [JsonProperty(".message|backgound-color")]
        public string MessageBackgroundColor;

        [JsonProperty(".message|border-color")]
        public string MessageBorderColor;

        [JsonProperty(".image|border-color")]
        public string ImageBorderColor;

        [JsonProperty(".message div:first-child|border-right-color")]
        public string MessageDivFirstChildBorderRightColor;

        [JsonProperty(".message div:last-child|border-right-color")]
        public string MessageDivLastChildBorderRightColor;

        [JsonProperty(".message|border-width")]
        public int MessageBorderWidth;

        [JsonProperty(".image|border-width")]
        public float ImageBorderWidth;

        [JsonProperty(".message|border-radius")]
        public string MessageBorderRadius;

        [JsonProperty(".image|border-radius")]
        public string ImageBorderRadius;

        [JsonProperty(".message|box-shadow")]
        public string MessageBoxShadow;

        [JsonProperty(".message::after|border-right-color")]
        public string MessageAfterBorderRightColor;

        [JsonProperty(".message::before|border-right-color")]
        public string MessageBeforeBorderRightColor;

        [JsonProperty(".message|font-family")]
        public string MessageFontFamily;

        [JsonProperty(".message|font-size")]
        public string MessageFontSize;

        [JsonProperty(".message|font-weight")]
        public int MessageFontWeight;

        [JsonProperty(".message|color")]
        public string MessageColor;

        [JsonProperty(".user|font-size")]
        public string UserFontSize;

        [JsonProperty(".user|letter-spacing")]
        public int UserLetterSpacing;

        [JsonProperty(".user|color")]
        public string UserColor;

        [JsonProperty(".user|text-shadow")]
        public string UserTextShadow;





    }

    class JSONdataRAW
    {
        [JsonProperty("event")]
        public string Event;
        public JSONdata data;
        //public Style styles;
        public CssStyles styles;
    }
    public class JSONdataMess
    {
        [JsonProperty("event")]
        public string Event;
        public JSONdata data = new JSONdata();
        //public Style styles;
    }
    public class JSONdata
    {
        private static char separator = Path.DirectorySeparatorChar;
        public string mascot = "file:///" + Directory.GetCurrentDirectory() + $"{separator}mascots{separator}" + Settings.json_settings.CurrentMascot + $"{separator}images{separator}idle.gif";
        public string audio = "";
        public string image = "";
        public string message = "";
        public string sender = "";
        public string recipient = "";
        public string bits = "";
        public string months = "";
        public string activity = "";
        public string id = "";
        public int mascotmouth = 0;
        public int time = 0;
        public float volume = 0f;
    }
    class Overlay
    {
        public static List<JSONdataMess> MessageQueue = new List<JSONdataMess>();
        //private static PipeServer _server;
        
        /*public async static void ConnectSocket(string server, int port)
        {

            _server = new PipeServer();
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.MessageReceived += ServerOnMessageReceived;
            _server.Open();

            while (!_server.Connected);

            JSONdataRAW rawData = new JSONdataRAW();

            rawData.Event = "EVENT_STREAMERCOMPANION";
            rawData.data = new JSONdata();
            rawData.styles = StylesToCss();

            //rawData.styles = Settings.json_settings.Styles;
            //rawData.styles = new CssStyles();
            //rawData.styles.MascotWidth = "80px";
            //rawData.styles.MascotLeft = "0";
            //rawData.styles.MascotRight = "auto";
            //rawData.styles.MessageLeft = "90px";
            //rawData.styles.MessageRight = "auto";
            //rawData.styles.MessageTransformOrigin = "0% 100%";
            //rawData.styles.MainboxTextAlign = "left";
            //rawData.styles.MessageAfterDisplay = "block";
            //rawData.styles.MessageDivFirstChildDisplay = "block";
            //rawData.styles.MessageBeforeDisplay = "none";
            //rawData.styles.MessageDivLastChildDisplay = "none";
            //rawData.styles.MessageBackgroundColor = "#fefeff";
            //rawData.styles.MessageBorderColor = "#69656c";
            //rawData.styles.ImageBorderColor = "#69656c";
            //rawData.styles.MessageDivFirstChildBorderRightColor = "#69656c";
            //rawData.styles.MessageDivLastChildBorderRightColor = "#69656c";
            //rawData.styles.MessageBorderWidth = 4;
            //rawData.styles.ImageBorderWidth = 2.0f;
            //rawData.styles.MessageBorderRadius = "4px";
            //rawData.styles.ImageBorderRadius = "8px";
            //rawData.styles.MessageBoxShadow = "-1px -1px 0 #ffffff, 1px -1px 0 #ffffff, -1px 1px 0 #ffffff, 1px 1px 0 #ffffff";
            //rawData.styles.MessageAfterBorderRightColor = "#ffffff";
            //rawData.styles.MessageBeforeBorderRightColor = "#ffffff";
            //rawData.styles.MessageFontFamily = "Arial";
            //rawData.styles.MessageFontSize = "22px";
            //rawData.styles.MessageFontWeight = 900;
            //rawData.styles.MessageColor = "#69656c";
            //rawData.styles.UserFontSize = "24px";
            //rawData.styles.UserLetterSpacing = 3;
            //rawData.styles.UserColor = "#ca5c67";
            //rawData.styles.UserTextShadow = "-1px -1px 0 #8e4148, 1px -1px 0 #8e4148, -1px 0 0 #8e4148, 1px 0 0 #8e4148, -1px 1px 0 #8e4148, 0 -1px 0 #8e4148, 0 1px 0 #8e4148, 1px 1px 0 #8e4148, 3px 3px 3px #fc938f";

            string ss = JsonConvert.SerializeObject(rawData);

            while(true)
            {
                _server.SendMessage(ss+"$$_end_$$");
                Thread.Sleep(2000);
            }

            //byte[] byData = System.Text.Encoding.ASCII.GetBytes(ss);
            var socket = new Fleck.WebSocketServer("ws://127.0.0.1:3339/");
            socket.ListenerSocket.NoDelay = true;
            socket.RestartAfterListenError = true;
            IWebSocketConnection connection = null;
            socket.Start(conn =>
            {
                conn.OnOpen = () =>
                {
                    connection = conn;
                };
                conn.OnMessage = message =>
                {
                    if (message == "ping test")
                    {
                        conn.Send("pong test");
                    }
                };
                conn.OnClose = () =>
                {
                    // ...
                };
            });
            int ping = 0;
            while (connection == null) ;
            if (connection != null)
            {
                await connection.Send(ss);
            }
            var ssBase = ss;
            while (true)
            {
                try
                {
                    //while (connection == null) ;
                    if (connection != null)
                    {
                        await connection.Send(ssBase);
                        _server.SendMessage(ss);
                    }
                    ping++;
                    if (connection != null)
                    {
                        if (MessageQueue.Count > 0)
                        {
                            JSONdataMess mess = MessageQueue[0];
                            MessageQueue.RemoveAt(0);
                            ss = JsonConvert.SerializeObject(mess);
                            await connection.Send(ss);
                            _server.SendMessage(ss);
                            Program.FormLog.Add("[OVERLAY]\tSend\n");
                            Thread.Sleep(mess.data.time);
                            ping = 0;
                            //await connection.Send(ssBase);
                        }
                        else
                        {
                            if (ping >= 40)
                            {
                                JSONdataMess mess = new JSONdataMess();
                                mess.Event = "EVENT_PING";
                                //ss = JsonConvert.SerializeObject(mess);
                                //connection.Send(ss);
                                await connection.Send("{\"event\":\"EVENT_PING\",\"data\":\"\"}");
                                //await connection.Send(ssBase);
                                ping = 0;
                            }
                        }

                        Thread.Sleep(500);
                    }
                    else
                    {
                    }
                }
                catch(Fleck.ConnectionNotAvailableException)
                {

                }
            }

        }*/
        private static void ServerOnConnect(object sender, EventArgs args)
        {
            //_server.SendMessage("test");
        }

        private static void ServerOnDisconnect(object sender, EventArgs args)
        {
        }

        /*private static void ServerOnMessageReceived(object sender, MessageEventArgs args)
        {
            if (args != null)
            {
                var message = args.Message;
            }
        }*/

        public static CssStyles StylesToCss()
        {
            CssStyles styles = new CssStyles();
            styles.MascotWidth = Settings.mascot_data.mascotStyles.MascotMaxWidth + "px";

            if (Settings.json_settings.AlignMascot == "right")
            {
                styles.MascotLeft = "auto";
                styles.MascotRight = "0";
                styles.MessageRight = (Settings.mascot_data.mascotStyles.MascotMaxWidth + 10) + "px";
                styles.MessageLeft = "auto";
                styles.MessageTransformOrigin = "100% 100%";
                styles.MainboxTextAlign = "right";
                styles.MessageAfterDisplay = "none";
                styles.MessageDivFirstChildDisplay = "none";
                styles.MessageBeforeDisplay = "block";
                styles.MessageDivLastChildDisplay = "block";
            }
            else
            {
                styles.MascotLeft = "0";
                styles.MascotRight = "auto";
                styles.MessageLeft = (Settings.mascot_data.mascotStyles.MascotMaxWidth + 10) + "px";
                styles.MessageRight = "auto";
                styles.MessageTransformOrigin = "0% 100%";
                styles.MainboxTextAlign = "left";
                styles.MessageAfterDisplay = "block";
                styles.MessageDivFirstChildDisplay = "block";
                styles.MessageBeforeDisplay = "none";
                styles.MessageDivLastChildDisplay = "none";
            }

            string highlight_text_stroke_color = "";
            string highlight_text_shadow_color = "";
            string highlight_text_shadow_offset = "";

            styles.MessageBackgroundColor = Settings.json_settings.Styles.BackgroundColor;

            styles.MessageBorderColor = Settings.json_settings.Styles.BorderColor;
            styles.ImageBorderColor = Settings.json_settings.Styles.BorderColor;
            styles.MessageDivFirstChildBorderRightColor = Settings.json_settings.Styles.BorderColor;
            styles.MessageDivLastChildBorderRightColor = Settings.json_settings.Styles.BorderColor;

            styles.MessageBorderWidth = Settings.json_settings.Styles.BorderWidth;
            styles.ImageBorderWidth = Settings.json_settings.Styles.BorderWidth / 2;

            styles.MessageBorderRadius = Settings.json_settings.Styles.BorderRadius + "px";
            styles.ImageBorderRadius = (Settings.json_settings.Styles.BorderRadius * 2) + "px";

            if (Settings.json_settings.Styles.BorderStrokeColor == "")
            {
                styles.MessageBoxShadow = "";
                styles.MessageAfterBorderRightColor = "transparent";
                styles.MessageBeforeBorderRightColor = "transparent";
            }
            else
            {
                string val = Settings.json_settings.Styles.BorderStrokeColor;
                styles.MessageBoxShadow = "-1px -1px 0 " + val + ", 1px -1px 0 " + val + ", -1px 1px 0 " + val + ", 1px 1px 0 " + val;
                styles.MessageAfterBorderRightColor = val;
                styles.MessageBeforeBorderRightColor = val;
            }

            styles.MessageFontFamily = Settings.json_settings.Styles.TextFontFamily;
            styles.MessageFontSize = Settings.json_settings.Styles.TextSize + "px";
            styles.MessageFontWeight = Settings.json_settings.Styles.TextWeight;
            styles.MessageColor = Settings.json_settings.Styles.TextColor;
            styles.UserFontSize = Settings.json_settings.Styles.HighlightTextSize + "px";
            styles.UserLetterSpacing = Settings.json_settings.Styles.HighlightTextSpacing;
            styles.UserColor = Settings.json_settings.Styles.HighlightTextColor;

            highlight_text_stroke_color = Settings.json_settings.Styles.HighlightTextStrokeColor;
            highlight_text_shadow_color = Settings.json_settings.Styles.HighlightTextShadowColor;
            highlight_text_shadow_offset = Settings.json_settings.Styles.HighlightTextShadowOffset.ToString();

            if (highlight_text_stroke_color != "" &&
                highlight_text_shadow_color != "" &&
                highlight_text_shadow_offset != "")
            {
                if (highlight_text_shadow_offset != "0")
                {
                    highlight_text_shadow_offset = highlight_text_shadow_offset + "px";
                }

                styles.UserTextShadow =
                    "-1px -1px 0 " + highlight_text_stroke_color +
                    ", 1px -1px 0 " + highlight_text_stroke_color +
                    ", -1px 0 0 " + highlight_text_stroke_color +
                    ", 1px 0 0 " + highlight_text_stroke_color +
                    ", -1px 1px 0 " + highlight_text_stroke_color +
                    ", 0 -1px 0 " + highlight_text_stroke_color +
                    ", 0 1px 0 " + highlight_text_stroke_color +
                    ", 1px 1px 0 " + highlight_text_stroke_color +
                    ", " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_color;
            }

            return styles;
        }
    }


    class OverlayNew
    {
        public static Dictionary<string, UserContext> connectedUsers = new Dictionary<string, UserContext>();
        public static Dictionary<string, bool> pingUsers = new Dictionary<string, bool>();
        public static System.Timers.Timer pingTimer = new System.Timers.Timer();
        public static void OverlayConnect()
        {
            pingTimer.Interval = 2000;
            pingTimer.Elapsed += CheckDeadConnections;
            var server = new Alchemy.WebSocketServer(3339, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)

            };

            server.Start();

            JSONdataRAW rawData = new JSONdataRAW();

            


            rawData.Event = "EVENT_STREAMERCOMPANION";
            rawData.data = new JSONdata();
            rawData.styles = StylesToCss();
            string ssBase = JsonConvert.SerializeObject(rawData);
            string ss;


            while (true)
            {
                if(connectedUsers.Count > 0)
                {
                    if(!pingTimer.Enabled)
                    {
                        foreach (UserContext usr in connectedUsers.Values)
                        {
                            usr.Send("__ping__");
                        }
                        pingTimer.Enabled = true;
                        pingTimer.Start();
                    }

                    if (Overlay.MessageQueue.Count > 0)
                    {
                        JSONdataMess mess = Overlay.MessageQueue[0];
                        Overlay.MessageQueue.RemoveAt(0);
                        ss = JsonConvert.SerializeObject(mess);
                        foreach (UserContext usr in connectedUsers.Values)
                        {
                            usr.Send(ss);
                        }
                        Program.FormLog.Add("[OVERLAY]\tSend\n");
                        Thread.Sleep(mess.data.time);
                        foreach (UserContext usr in connectedUsers.Values)
                        {
                            usr.Send(ssBase);
                        }
                        
                        //await connection.Send(ssBase);
                    }
                    else
                    {
                        foreach (UserContext usr in connectedUsers.Values)
                        {
                            //usr.Send(ssBase);
                        }
                    }
                    
                }
                Thread.Sleep(500);
            }
        }

        public static void OnReceive(UserContext context)
        {
            if (context.DataFrame.ToString() == "__ping__")
            {
                context.Send("__pong__");
                Console.WriteLine("SendPong - " + context.ClientAddress.ToString());
            }


            if (context.DataFrame.ToString() == "__pong__")
            {
                pingUsers[context.ClientAddress.ToString()] = true;
                Console.WriteLine("ReceivedPong - " + context.ClientAddress.ToString());
            }
        }

        public static void CheckDeadConnections(Object source, System.Timers.ElapsedEventArgs e)
        {
            foreach(UserContext usr in connectedUsers.Values)
            {
                if(pingUsers[usr.ClientAddress.ToString()] == false)
                {
                    connectedUsers.Remove(usr.ClientAddress.ToString());
                    pingUsers.Remove(usr.ClientAddress.ToString());
                }
            }
            pingTimer.Enabled = false;

        }

        public static void OnSend(UserContext context)
        {

        }
        public static void OnConnect(UserContext context)
        {
            JSONdataRAW rawData = new JSONdataRAW();


            rawData.Event = "EVENT_STREAMERCOMPANION";
            rawData.data = new JSONdata();
            rawData.styles = StylesToCss();
            string ssBase = JsonConvert.SerializeObject(rawData);

            Console.WriteLine(context.ClientAddress.ToString());
            connectedUsers.Add(context.ClientAddress.ToString(), context);
            pingUsers.Add(context.ClientAddress.ToString(), true);
            context.Send(ssBase);

        }
        public static void OnDisconnect(UserContext context)
        {
            Console.WriteLine(context.ClientAddress.ToString());
            connectedUsers.Remove(context.ClientAddress.ToString());
            pingUsers.Remove(context.ClientAddress.ToString());
        }

        public static CssStyles StylesToCss()
        {
            CssStyles styles = new CssStyles();
            styles.MascotWidth = Settings.mascot_data.mascotStyles.MascotMaxWidth + "px";

            if (Settings.json_settings.AlignMascot == "right")
            {
                styles.MascotLeft = "auto";
                styles.MascotRight = "0";
                styles.MessageRight = (Settings.mascot_data.mascotStyles.MascotMaxWidth + 10) + "px";
                styles.MessageLeft = "auto";
                styles.MessageTransformOrigin = "100% 100%";
                styles.MainboxTextAlign = "right";
                styles.MessageAfterDisplay = "none";
                styles.MessageDivFirstChildDisplay = "none";
                styles.MessageBeforeDisplay = "block";
                styles.MessageDivLastChildDisplay = "block";
            }
            else
            {
                styles.MascotLeft = "0";
                styles.MascotRight = "auto";
                styles.MessageLeft = (Settings.mascot_data.mascotStyles.MascotMaxWidth + 10) + "px";
                styles.MessageRight = "auto";
                styles.MessageTransformOrigin = "0% 100%";
                styles.MainboxTextAlign = "left";
                styles.MessageAfterDisplay = "block";
                styles.MessageDivFirstChildDisplay = "block";
                styles.MessageBeforeDisplay = "none";
                styles.MessageDivLastChildDisplay = "none";
            }

            string highlight_text_stroke_color = "";
            string highlight_text_shadow_color = "";
            string highlight_text_shadow_offset = "";

            styles.MessageBackgroundColor = Settings.json_settings.Styles.BackgroundColor;

            styles.MessageBorderColor = Settings.json_settings.Styles.BorderColor;
            styles.ImageBorderColor = Settings.json_settings.Styles.BorderColor;
            styles.MessageDivFirstChildBorderRightColor = Settings.json_settings.Styles.BorderColor;
            styles.MessageDivLastChildBorderRightColor = Settings.json_settings.Styles.BorderColor;

            styles.MessageBorderWidth = Settings.json_settings.Styles.BorderWidth;
            styles.ImageBorderWidth = Settings.json_settings.Styles.BorderWidth / 2;

            styles.MessageBorderRadius = Settings.json_settings.Styles.BorderRadius + "px";
            styles.ImageBorderRadius = (Settings.json_settings.Styles.BorderRadius * 2) + "px";

            if (Settings.json_settings.Styles.BorderStrokeColor == "")
            {
                styles.MessageBoxShadow = "";
                styles.MessageAfterBorderRightColor = "transparent";
                styles.MessageBeforeBorderRightColor = "transparent";
            }
            else
            {
                string val = Settings.json_settings.Styles.BorderStrokeColor;
                styles.MessageBoxShadow = "-1px -1px 0 " + val + ", 1px -1px 0 " + val + ", -1px 1px 0 " + val + ", 1px 1px 0 " + val;
                styles.MessageAfterBorderRightColor = val;
                styles.MessageBeforeBorderRightColor = val;
            }

            styles.MessageFontFamily = Settings.json_settings.Styles.TextFontFamily;
            styles.MessageFontSize = Settings.json_settings.Styles.TextSize + "px";
            styles.MessageFontWeight = Settings.json_settings.Styles.TextWeight;
            styles.MessageColor = Settings.json_settings.Styles.TextColor;
            styles.UserFontSize = Settings.json_settings.Styles.HighlightTextSize + "px";
            styles.UserLetterSpacing = Settings.json_settings.Styles.HighlightTextSpacing;
            styles.UserColor = Settings.json_settings.Styles.HighlightTextColor;

            highlight_text_stroke_color = Settings.json_settings.Styles.HighlightTextStrokeColor;
            highlight_text_shadow_color = Settings.json_settings.Styles.HighlightTextShadowColor;
            highlight_text_shadow_offset = Settings.json_settings.Styles.HighlightTextShadowOffset.ToString();

            if (highlight_text_stroke_color != "" &&
                highlight_text_shadow_color != "" &&
                highlight_text_shadow_offset != "")
            {
                if (highlight_text_shadow_offset != "0")
                {
                    highlight_text_shadow_offset = highlight_text_shadow_offset + "px";
                }

                styles.UserTextShadow =
                    "-1px -1px 0 " + highlight_text_stroke_color +
                    ", 1px -1px 0 " + highlight_text_stroke_color +
                    ", -1px 0 0 " + highlight_text_stroke_color +
                    ", 1px 0 0 " + highlight_text_stroke_color +
                    ", -1px 1px 0 " + highlight_text_stroke_color +
                    ", 0 -1px 0 " + highlight_text_stroke_color +
                    ", 0 1px 0 " + highlight_text_stroke_color +
                    ", 1px 1px 0 " + highlight_text_stroke_color +
                    ", " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_offset +
                    " " + highlight_text_shadow_color;
            }

            return styles;
        }
    }


}
