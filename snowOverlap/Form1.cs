using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32;

namespace snowOverlap
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public int[,] points = new int[150, 5];
        int maxV = 2;
        int maxW = 10;
        int maxG = 2;
        Bitmap bmp;
        Graphics g;
        Form settings = new Form();
        TrackBar maxSize, maxSpeed, maxGravity;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
            int initialStyle = GetWindowLongPtr(this.Handle, -20);
            SetWindowLong32(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            //SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            this.TopMost = true;
            SetForegroundWindow(this.Handle);
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, canvas, new object[] { true });
            generatePoints();

            settings.FormBorderStyle = FormBorderStyle.FixedSingle;
            settings.Icon = Properties.Resources.snowflake_7Qb_icon;
            settings.BackColor = Color.DimGray;
            settings.Width = 500;
            settings.Height = 500;

            maxSize = new TrackBar();
            maxSize.BackColor = Color.DimGray;
            maxSize.Maximum = 50;
            maxSize.Minimum = 2;
            maxSize.Width = 300;
            maxSize.Value = 10;
            maxSize.Location = new Point(100, 10);
            maxSize.ValueChanged += maxSize_ValueChanged;

            Label maxSizeLabel = new Label();
            maxSizeLabel.Text = "Max Snow Size";
            maxSizeLabel.Location = new Point(10, 15);
            maxSizeLabel.ForeColor = Color.White;

            maxSpeed = new TrackBar();
            maxSpeed.BackColor = Color.DimGray;
            maxSpeed.Maximum = 50;
            maxSpeed.Minimum = 2;
            maxSpeed.Width = 300;
            maxSpeed.Value = 10;
            maxSpeed.Location = new Point(100, 60);
            maxSpeed.ValueChanged += maxSpeed_ValueChanged;

            Label maxWindLabel = new Label();
            maxWindLabel.Text = "Wind Speed";
            maxWindLabel.Location = new Point(10, 65);
            maxWindLabel.ForeColor = Color.White;

            maxGravity = new TrackBar();
            maxGravity.BackColor = Color.DimGray;
            maxGravity.Maximum = 50;
            maxGravity.Minimum = 0;
            maxGravity.Width = 300;
            maxGravity.Value = 10;
            maxGravity.Location = new Point(100, 110);
            maxGravity.ValueChanged += maxGravity_ValueChanged;

            Label maxGravityLabel = new Label();
            maxGravityLabel.Text = "Gravity";
            maxGravityLabel.Location = new Point(10, 115);
            maxGravityLabel.ForeColor = Color.White;


            settings.Controls.Add(maxSizeLabel);
            settings.Controls.Add(maxWindLabel);
            settings.Controls.Add(maxSize);
            settings.Controls.Add(maxSpeed);
            settings.Controls.Add(maxGravity);
            settings.Controls.Add(maxGravityLabel);
            settings.FormClosing += settings_FormClosing;

        }

        public void settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            settings.Hide();
        }

        public void generatePoints()
        {
            Random rnd = new Random();
            for (int i = 0; i < points.GetLength(0); i++)
            {
                points[i, 0] = rnd.Next(1, canvas.Width);
                points[i, 1] = rnd.Next(1, canvas.Height);
                points[i, 4] = rnd.Next(2, maxW);
                points[i, 2] = rnd.Next(0, maxV);
                points[i, 3] = rnd.Next(points[i, 4] - 2, points[i, 4] + maxG);
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < points.GetLength(0); i++)
            {
                e.Graphics.FillEllipse(Brushes.White, points[i, 0], points[i, 1], points[i, 4], points[i, 4]);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateSnow();
        }

        public void updateSnow()
        {
            for (int i = 0; i < points.GetLength(0); i++)
            {
                points[i, 0] += points[i, 2];
                points[i, 1] += points[i, 3];
                if (points[i, 0] > canvas.Width)
                {
                    points[i, 0] = 0;
                }
                if (points[i, 1] > canvas.Height)
                {
                    points[i, 1] = 0;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //canvas.BackgroundImage = bmp;
            this.Invalidate();
        }
        // Urm wasnt expecting you to look down here, this isn't really that bad
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            System.Environment.Exit(1);
        }
        

        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            settings.Show(this);
        }

        public void maxSize_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("close");
            maxW = maxSize.Value;
            generatePoints();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hideToolStripMenuItem.Text == "Hide")
            {
                this.Visible = false;
                timer1.Stop();
                timer2.Stop();
                hideToolStripMenuItem.Text = "Show";
            } else
            {
                this.Visible = true;
                hideToolStripMenuItem.Text = "Hide";
                timer1.Start();
                timer2.Start();
            }

        }

        public void maxSpeed_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("close");
            maxV = maxSpeed.Value;
            generatePoints();
        }

        public void maxGravity_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("close");
            maxG = maxGravity.Value;
            generatePoints();
        }

    }
}
