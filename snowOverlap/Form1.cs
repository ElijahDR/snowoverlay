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
        int maxV = 5;
        int maxW = 10;
        Bitmap bmp;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
            int initialStyle = GetWindowLongPtr(this.Handle, -20);
            SetWindowLong32(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            //SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            SetForegroundWindow(this.Handle);
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, canvas, new object[] { true });
            Random rnd = new Random();
            for (int i = 0; i < points.GetLength(0); i++)
            {
                points[i, 0] = rnd.Next(1, canvas.Width);
                points[i, 1] = rnd.Next(1, canvas.Height);
                points[i, 2] = rnd.Next(1, maxV);
                points[i, 3] = rnd.Next(1, maxV);
                points[i, 4] = rnd.Next(1, maxW);
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
    }
}
