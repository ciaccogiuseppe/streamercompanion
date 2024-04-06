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
    public partial class Form_Overlay : Form
    {
        public List<OverlayEvent> OverlayEvents = new List<OverlayEvent>();  
        public Form_Overlay()
        {
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            this.Text = "streamerCompanion";

            ///webBrowser1.AllowWebBrowserDrop = false;
            ///webBrowser1.Url = new Uri("https://www.google.com");
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            //this.BackColor = Color.FromArgb(0, 0, 0, 0);
            //BackColor = Color.Lime;
            //TransparencyKey = Color.Lime;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        /*protected override void OnPaintBackground(PaintEventArgs e)
{
e.Graphics.FillRectangle(Brushes.Magenta, e.ClipRectangle);
}*/
    }
}
