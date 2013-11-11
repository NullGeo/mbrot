using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbrot
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(1000, 1000);

            var watch = Stopwatch.StartNew();

            FractalBuilder builder = new FractalBuilder(1000, 1000, new FractalOptions());
            this.BackgroundImage = builder.Build();

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            this.Text += " - " + elapsedMs + " milliseconds";
        }

    }
}