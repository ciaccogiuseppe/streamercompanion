using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace streamerCompanion
{
    
    static class Program
    {
        public static Settings settings;
        public static List<string> FormLog = new List<string>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            settings = new Settings();
            Thread overlayThread = new Thread(new ThreadStart(OverlayThread));
            Thread clientThread = new Thread(new ThreadStart(ClientThread));
            overlayThread.Start();
            clientThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            overlayThread.Abort();
            clientThread.Abort();
        }

        static void OverlayThread()
        {
            //Overlay.ConnectSocket("127.0.0.1", 3339);
            OverlayNew.OverlayConnect();
        }

        static void ClientThread()
        {
            TwClient.ClientConnect();
        }
    }

    

    static class Globals
    {
        public static string BOT_VERSION = "2.0pre";
    }
}
