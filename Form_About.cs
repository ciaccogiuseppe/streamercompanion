using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace streamerCompanion
{
    public partial class Form_About : Form
    {
        public Form_About()
        {
            InitializeComponent();
            this.Size = new Size(200, 150);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            this.Text = "streamerCompanion v" + Globals.BOT_VERSION + " - About";



            int a = pictureBox1.Width - pictureBox1.Image.Width;
            int b = pictureBox1.Height - pictureBox1.Image.Height;
            Padding p = new Padding();
            p.Left = a / 2;
            p.Top = b / 2;
            pictureBox1.Padding = p;

            //pictureBox1.Anchor = AnchorStyles.None;

            label1.AutoSize = false;
            label1.TextAlign = ContentAlignment.BottomCenter;
            label1.Dock = DockStyle.Fill;
            //label1.Location = new Point(100, 65);
            
            label1.Text = "streamerCompanion v" + Globals.BOT_VERSION + "\nby giusec\nfajacopo\n";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
