using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public abstract class Screen
{
    public Form forms { get; set; }
    public Timer timer { get; set; }
    public int interval { get; set; }
    public DateTime latestChange { get; set; }
    public Graphics g { get; set; } = null;
    public PictureBox pb { get; set; } = null;
    public DateTime now { get; set;} = DateTime.Now;
    public ScreenSession currentScreen { get; set; }

    public void Start()
    {
        ApplicationConfiguration.Initialize();
        this.forms = new Form();
        this.forms.WindowState = FormWindowState.Maximized;
        this.forms.FormBorderStyle = FormBorderStyle.None;
        this.forms.BackColor = Color.Black;
        this.timer = new Timer();
        this.timer.Interval = this.interval;

        this.forms.KeyPreview = true;

        Game.Current.keymap.SetAction(this.forms);

        this.timer.Tick += delegate
        {
            this.now = DateTime.Now;
            currentScreen.DrawScreen(this.g, this.pb, this.now);

            if (this.currentScreen.isFinished)
            {
                this.currentScreen = currentScreen.nextScreen;
                this.currentScreen.isFinished = false;
            }

            this.pb.Refresh();
        };

        this.forms.Load += delegate 
        {
            this.LoadForm();
        };

        Application.Run(forms);
    }

    protected abstract void LoadForm();
}

public class DefaultScreen : Screen
{
    public Bitmap bmp { get; set; }
    
    public DefaultScreen(ScreenArgs args)
    {
        this.interval = args.interval;
    }
    protected override void LoadForm()
    {
        this.pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        this.forms.Controls.Add(pb);
        this.bmp = new Bitmap(this.pb.Width, this.pb.Height);
        this.g = Graphics.FromImage(bmp);
        this.g.Clear(Color.Black);
        this.pb.Image = bmp;
        this.timer.Start();
    }
}
