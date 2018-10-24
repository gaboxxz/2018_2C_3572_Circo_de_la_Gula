using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Model;
using static TGC.Core.Sound.TgcMp3Player;

namespace TGC.Group.Form
{
    public partial class GameForm : System.Windows.Forms.Form
    {
        private Viewer Viewer { get; set; }

        public GameForm()
        {
            InitializeComponent();
        }

        /** Form Events **/
        private void GameForm_Load(object sender, EventArgs e)
        {
            Viewer = new Viewer(this, panel3D);
            Viewer.Run();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Viewer.ApplicationRunning)
            {
                Viewer.ShutDown();
            }
        }
    }
}