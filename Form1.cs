using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace streamerCompanion
{
    
    public partial class Form1 : Form
    {
        public static bool LiveStatus = false;
        public static bool ChatStatus = false;
        public static bool ConnectionStatus = false;
        public static bool APIStatus = false;
        public static bool PubSubStatus = false;

        private Timer GUIStatusTimer = new Timer();

        Form_Overlay ChildForm = new Form_Overlay();
        Form_About AboutForm = new Form_About();

        
        
        private LiveStreamMonitorService Monitor;


        

        public static List<string> FormLog = new List<string>();

        public Form1()
        {
            InitializeComponent();
            Text = "streamerCompanion v" + Globals.BOT_VERSION + " - Control panel";

            ClientInitialize();
            TimersInitialize();

            GUIStatusTimer.Start();

            //backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            //backgroundWorker1.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);

            //backgroundWorker1.RunWorkerAsync();



        }

        private void TimersInitialize()
        {
            GUIStatusTimer.Tick += new EventHandler(TimerEventProcessor);
            GUIStatusTimer.Interval = 200;
        }

        private void ClientInitialize()
        {

            

            /*Monitor = new LiveStreamMonitorService(API, 60);
            List<string> lst = new List<string> { "45222658"};
            Monitor.SetChannelsById(lst);
            Monitor.OnStreamOnline += Monitor_OnStreamOnline;
            Monitor.OnStreamOffline += Monitor_OnStreamOffline;

            Monitor.Start();*/
        }

        



        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text!= "" && Settings.json_settings.Commands.ContainsKey("!" + textBox1.Text))
            {
                //FormLog.Add("[COMMAND]\t" + textBox1.Text + "\n");
                TwClient.Client.InvokeMessageReceived(
                    "test",
                    "-1",
                    "test",
                    "test",
                    "0000000",
                    Color.Red,
                    null,
                    "!" + textBox1.Text,
                    new UserType(),
                    "",
                    "",
                    false,
                    0,
                    "",
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    new Noisy(),
                    "",
                    "",
                    null,
                    null,
                    0,
                    0.0f);
                textBox1.Text = "";
            }
        }

        /*private void button3_Click(object sender, EventArgs e)
        {
            if (!ChildForm.Visible)
            {
                ChildForm.Show();
                button3.Text = "Hide";
            }
                
            else
            {
                ChildForm.Hide();
                button3.Text = "Show";
            }
                
        }*/

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if(AboutForm != null)
            
            AboutForm.Close();
            AboutForm = new Form_About();
            AboutForm.Show();
        }


        private int i = 0;
        private static int control = 0;
        
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (control == 0)
            {
                label_LiveStatus.Text = "Offline";
                label_LiveStatus.ForeColor = Color.Red;
            }
            else
            {
                label_LiveStatus.Text = "Online";
                label_LiveStatus.ForeColor = Color.DarkGreen;
            }
            //backgroundWorker1.RunWorkerAsync();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            GUIStatusTimer.Stop();
            //label4.Text = txt;

            /*if (control == 0) control = 1;
            else control = 0;
            if (control == 0)
            {
                label_LiveStatus.Text = "Offline";
                label_LiveStatus.ForeColor = Color.Red;
            }
            else
            {
                label_LiveStatus.Text = "Online";
                label_LiveStatus.ForeColor = Color.DarkGreen;
            }*/

            if(LiveStatus == true)
            {
                label_LiveStatus.Text = "Online";
                label_LiveStatus.ForeColor = Color.DarkGreen;
            }
            else
            {
                label_LiveStatus.Text = "Offline";
                label_LiveStatus.ForeColor = Color.Red;
            }

            if(ChatStatus == true)
            {
                ReconnectButton.Enabled = false;
                label4.Text = "Connected";
                label4.ForeColor = Color.DarkGreen;
            }
            else
            {
                ReconnectButton.Enabled = true;
                label4.Text = "Not connected";
                label4.ForeColor = Color.Red;
            }
            if(OverlayNew.connectedUsers.Count > 0)
            {
                label2.Text = "Connected";
                label2.ForeColor = Color.DarkGreen;
            }
            else
            {
                label2.Text = "Not connected";
                label2.ForeColor = Color.Red;
            }

            while(FormLog.Count != 0)
            {
                logTextBox.AppendText(FormLog[0]);
                FormLog.RemoveAt(0);
                logTextBox.ScrollToCaret();
            }
            //FormLog.Clear();
            while (Program.FormLog.Count != 0)
            {
                logTextBox.AppendText(Program.FormLog[0]);
                Program.FormLog.RemoveAt(0);
                logTextBox.ScrollToCaret();
            }
            //backgroundWorker1.RunWorkerAsync();
            GUIStatusTimer.Start();

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void ReconnectButton_Click(object sender, EventArgs e)
        {
            if (!TwClient.Client.IsConnected)
                TwClient.Client.Connect();
            TwClient.PubSubClient.Connect();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {

                if (textBox1.Text != "")
                {

                   



                    //FormLog.Add("[COMMAND]\t" + textBox1.Text + "\n");

                    TwClient.Client.InvokeMessageReceived(
                    "test",
                    "-1",
                    "test",
                    "test",
                    "0000000",
                    Color.Red,
                    null,
                    "!" + textBox1.Text,
                    new UserType(),
                    "",
                    "",
                    false,
                    0,
                    "",
                    true,
                    true,
                    true,
                    true,
                    false,
                    false,
                    false,
                    new Noisy(),
                    "",
                    "",
                    null,
                    null,
                    0,
                    0.0f);

                    textBox1.Text = "";
                }
            }
        }

        private void force_reconnect_Click(object sender, EventArgs e)
        {
            if(TwClient.Client.IsConnected)
            {
                TwClient.Client.Disconnect();
            }
            TwClient.Client.Connect();
            TwClient.PubSubClient.Connect();
        }

        private void reload_settings_Click(object sender, EventArgs e)
        {
            Program.settings = new Settings();
        }
    }
}


/*
 *  EventHandler< OnLogArgs > 	OnLog
 	Fires whenever a log write happens. More...
 
EventHandler< OnConnectedArgs > 	OnConnected
 	Fires when Client connects to Twitch. More...
 
EventHandler< OnJoinedChannelArgs > 	OnJoinedChannel
 	Fires when Client joins a channel. More...
 
EventHandler< OnIncorrectLoginArgs > 	OnIncorrectLogin
 	Fires on logging in with incorrect details, returns ErrorLoggingInException. More...
 
EventHandler< OnChannelStateChangedArgs > 	OnChannelStateChanged
 	Fires when connecting and channel state is changed, returns ChannelState. More...
 
EventHandler< OnUserStateChangedArgs > 	OnUserStateChanged
 	Fires when a user state is received, returns UserState. More...
 
EventHandler< OnMessageReceivedArgs > 	OnMessageReceived
 	Fires when a new chat message arrives, returns ChatMessage. More...
 
EventHandler< OnWhisperReceivedArgs > 	OnWhisperReceived
 	Fires when a new whisper arrives, returns WhisperMessage. More...
 
EventHandler< OnMessageSentArgs > 	OnMessageSent
 	Fires when a chat message is sent, returns username, channel and message. More...
 
EventHandler< OnWhisperSentArgs > 	OnWhisperSent
 	Fires when a whisper message is sent, returns username and message. More...
 
EventHandler< OnChatCommandReceivedArgs > 	OnChatCommandReceived
 	Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage, arguments as string, arguments as list. More...
 
EventHandler< OnWhisperCommandReceivedArgs > 	OnWhisperCommandReceived
 	Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage. More...
 
EventHandler< OnUserJoinedArgs > 	OnUserJoined
 	Fires when a new viewer/chatter joined the channel's chat room, returns username and channel. More...
 
EventHandler< OnModeratorJoinedArgs > 	OnModeratorJoined
 	Fires when a moderator joined the channel's chat room, returns username and channel. More...
 
EventHandler< OnModeratorLeftArgs > 	OnModeratorLeft
 	Fires when a moderator joins the channel's chat room, returns username and channel. More...
 
EventHandler< OnNewSubscriberArgs > 	OnNewSubscriber
 	Fires when new subscriber is announced in chat, returns Subscriber. More...
 
EventHandler< OnReSubscriberArgs > 	OnReSubscriber
 	Fires when current subscriber renews subscription, returns ReSubscriber. More...
 
EventHandler 	OnHostLeft
 	Fires when a hosted streamer goes offline and hosting is killed. More...
 
EventHandler< OnExistingUsersDetectedArgs > 	OnExistingUsersDetected
 	Fires when Twitch notifies Client of existing users in chat. More...
 
EventHandler< OnUserLeftArgs > 	OnUserLeft
 	Fires when a PART message is received from Twitch regarding a particular viewer More...
 
EventHandler< OnHostingStartedArgs > 	OnHostingStarted
 	Fires when the joined channel begins hosting another channel. More...
 
EventHandler< OnHostingStoppedArgs > 	OnHostingStopped
 	Fires when the joined channel quits hosting another channel. More...
 
EventHandler< OnDisconnectedEventArgs > 	OnDisconnected
 	Fires when bot has disconnected. More...
 
EventHandler< OnConnectionErrorArgs > 	OnConnectionError
 	Forces when bot suffers conneciton error. More...
 
EventHandler< OnChatClearedArgs > 	OnChatCleared
 	Fires when a channel's chat is cleared. More...
 
EventHandler< OnUserTimedoutArgs > 	OnUserTimedout
 	Fires when a viewer gets timedout by any moderator. More...
 
EventHandler< OnLeftChannelArgs > 	OnLeftChannel
 	Fires when Client successfully leaves a channel. More...
 
EventHandler< OnUserBannedArgs > 	OnUserBanned
 	Fires when a viewer gets banned by any moderator. More...
 
EventHandler< OnModeratorsReceivedArgs > 	OnModeratorsReceived
 	Fires when a list of moderators is received. More...
 
EventHandler< OnChatColorChangedArgs > 	OnChatColorChanged
 	Fires when confirmation of a chat color change request was received. More...
 
EventHandler< OnSendReceiveDataArgs > 	OnSendReceiveData
 	Fires when data is either received or sent. More...
 
EventHandler< OnNowHostingArgs > 	OnNowHosting
 	Fires when Client receives notice that a joined channel is hosting another channel. More...
 
EventHandler< OnBeingHostedArgs > 	OnBeingHosted
 	Fires when the library detects another channel has started hosting the broadcaster's stream. MUST BE CONNECTED AS BROADCASTER. More...
 
EventHandler< OnRaidNotificationArgs > 	OnRaidNotification
 	Fires when a raid notification is detected in chat More...
 
EventHandler< OnGiftedSubscriptionArgs > 	OnGiftedSubscription
 	Fires when a subscription is gifted and announced in chat More...
 
EventHandler< OnCommunitySubscriptionArgs > 	OnCommunitySubscription
 	Fires when a community subscription is announced in chat More...
 
EventHandler< OnMessageThrottledEventArgs > 	OnMessageThrottled
 	Fires when a Message has been throttled. More...
 
EventHandler< OnWhisperThrottledEventArgs > 	OnWhisperThrottled
 	Fires when a Whisper has been throttled. More...
 
EventHandler< OnErrorEventArgs > 	OnError
 	Occurs when an Error is thrown in the protocol Client More...
 
EventHandler< OnReconnectedEventArgs > 	OnReconnected
 	Occurs when a reconnection occurs. More...
 */



/*EventHandler 	OnPubSubServiceConnected
 	Fires when PubSub Service is connected. More...
 
EventHandler< OnPubSubServiceErrorArgs > 	OnPubSubServiceError
 	Fires when PubSub Service has an error. More...
 
EventHandler 	OnPubSubServiceClosed
 	Fires when PubSub Service is closed. More...
 
EventHandler< OnListenResponseArgs > 	OnListenResponse
 	Fires when PubSub receives any response. More...
 
EventHandler< OnTimeoutArgs > 	OnTimeout
 	Fires when PubSub receives notice a viewer gets a timeout. More...
 
EventHandler< OnBanArgs > 	OnBan
 	Fires when PubSub receives notice a viewer gets banned. More...
 
EventHandler< OnUnbanArgs > 	OnUnban
 	Fires when PubSub receives notice a viewer gets unbanned. More...
 
EventHandler< OnUntimeoutArgs > 	OnUntimeout
 	Fires when PubSub receives notice a viewer gets a timeout removed. More...
 
EventHandler< OnHostArgs > 	OnHost
 	Fires when PubSub receives notice that the channel being listened to is hosting another channel. More...
 
EventHandler< OnSubscribersOnlyArgs > 	OnSubscribersOnly
 	Fires when PubSub receives notice that Sub-Only Mode gets turned on. More...
 
EventHandler< OnSubscribersOnlyOffArgs > 	OnSubscribersOnlyOff
 	Fires when PubSub receives notice that Sub-Only Mode gets turned off. More...
 
EventHandler< OnClearArgs > 	OnClear
 	Fires when PubSub receives notice that chat gets cleared. More...
 
EventHandler< OnEmoteOnlyArgs > 	OnEmoteOnly
 	Fires when PubSub receives notice that Emote-Only Mode gets turned on. More...
 
EventHandler< OnEmoteOnlyOffArgs > 	OnEmoteOnlyOff
 	Fires when PubSub receives notice that Emote-Only Mode gets turned off. More...
 
EventHandler< OnR9kBetaArgs > 	OnR9kBeta
 	Fires when PubSub receives notice that the chat option R9kBeta gets turned on. More...
 
EventHandler< OnR9kBetaOffArgs > 	OnR9kBetaOff
 	Fires when PubSub receives notice that the chat option R9kBeta gets turned off. More...
 
EventHandler< OnBitsReceivedArgs > 	OnBitsReceived
 	Fires when PubSub receives notice of a bit donation. More...
 
EventHandler< OnChannelCommerceReceivedArgs > 	OnChannelCommerceReceived
 	Fires when PubSub receives notice of a commerce transaction. More...
 
EventHandler< OnStreamUpArgs > 	OnStreamUp
 	Fires when PubSub receives notice that the stream of the channel being listened to goes online. More...
 
EventHandler< OnStreamDownArgs > 	OnStreamDown
 	Fires when PubSub receives notice that the stream of the channel being listened to goes offline. More...
 
EventHandler< OnViewCountArgs > 	OnViewCount
 	Fires when PubSub receives notice view count has changed. More...
 
EventHandler< OnWhisperArgs > 	OnWhisper
 	Fires when PubSub receives a whisper. More...
 
EventHandler< OnChannelSubscriptionArgs > 	OnChannelSubscription
 	Fires when PubSub receives notice when the channel being listened to gets a subscription. More...
 
EventHandler< OnChannelExtensionBroadcastArgs > 	OnChannelExtensionBroadcast
 	Fires when PubSub receives a message sent to the specified extension on the specified channel. More...
 
EventHandler< OnFollowArgs > 	OnFollow
    Fires when PubSub receives notice when a user follows the designated channel. More...
 */
