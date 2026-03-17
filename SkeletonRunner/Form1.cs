using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Media;

namespace SkeletonRunner
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer chanceTimer = new System.Windows.Forms.Timer();
        Random random = new Random();
        SoundPlayer skeletonSound;

        public Form1()
        {
            InitializeComponent();

            skeletonSound = new SoundPlayer(Path.Combine(Application.StartupPath, "assets", "skeleton.wav"));

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;


            SpawnSkeleton();

            chanceTimer.Interval = 1000; // 1 segundo
            chanceTimer.Tick += ChanceTimer_Tick;
            chanceTimer.Start();
        }
        private void ChanceTimer_Tick(object sender, EventArgs e)
        {
            int roll = random.Next(500); // 0 a 1999

            if (roll == 0)
            {
                SpawnSkeleton();
            }
        }
        private void SpawnSkeleton()
        {
            PictureBox skeleton = new PictureBox();
            skeleton.SizeMode = PictureBoxSizeMode.AutoSize;
            skeleton.Image = Image.FromFile(Path.Combine(Application.StartupPath, "assets", "skeleton.gif")); // Imagem ou Gif :3
            skeleton.BackColor = Color.Transparent;

            bool fromLeft = random.Next(2) == 0;
            int direction = fromLeft ? 1 : -1;

            int maxY = Screen.PrimaryScreen.Bounds.Height - skeleton.Height;
            int y = random.Next(Math.Max(1, maxY));

            int startX = fromLeft ? -skeleton.Width : Screen.PrimaryScreen.Bounds.Width;
            skeleton.Location = new Point(startX, y);
            this.Controls.Add(skeleton);
            skeleton.BringToFront();
            skeletonSound.Play();

            Image img = skeleton.Image;

            if (!fromLeft)
            {
                img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            System.Windows.Forms.Timer moveTimer = new System.Windows.Forms.Timer();
            moveTimer.Interval = 16; // 60fps

            moveTimer.Tick += (s, e) =>
            {
                skeleton.Left += 100 * direction;

                if ((direction == 1 && skeleton.Left > Screen.PrimaryScreen.Bounds.Width) ||
                    (direction == -1 && skeleton.Right < 0))
                {
                    moveTimer.Stop();
                    moveTimer.Dispose();
                    skeleton.Dispose();
                }
            };
            skeleton.Tag = moveTimer;
            moveTimer.Start();
        }

        private void SetStartup(bool enable)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key == null)
            {
                MessageBox.Show("Não foi possível acessar o registro para configurar a inicialização automática.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (enable)
            {
                string appPath = Application.ExecutablePath;
                key.SetValue("SkeletonRunner", Application.ExecutablePath);
            }
            else
            {
                key.DeleteValue("SkeletonRunner", false);
            }
            key.Close();
        }
    }
}