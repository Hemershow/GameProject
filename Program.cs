using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

ApplicationConfiguration.Initialize();

var form = new Form();
form.WindowState = FormWindowState.Maximized;

Graphics g = null;
Bitmap bmp = null;
PictureBox pb = new PictureBox();

pb.Dock = DockStyle.Fill;
form.Controls.Add(pb);

Queue<DateTime> queue = new Queue<DateTime>();
queue.Enqueue(DateTime.Now);

var tm = new Timer();
tm.Interval = 20;

var sonic = new Bitmap("../sonic3.gif");
sonic = new Bitmap(sonic, new Size(sonic.Width*3, sonic.Height*3));

int framesInSheet = 8;
int charW = (sonic.Width/framesInSheet);
int charH = (sonic.Height);
int animationLength = 1000;

var character = 0;
var fps = 1;

var lastChange = DateTime.Now;

tm.Tick += delegate
{
    g.Clear(Color.White);
    var now = DateTime.Now;

    queue.Enqueue(now);
    if (queue.Count > 19)
    {
        DateTime old = queue.Dequeue();
        var time = now - old;
        fps = (int)(19 / time.TotalSeconds);
    }

    if (character == framesInSheet)
        character = 0;

    g.DrawImage(sonic, 0, 0, new Rectangle(charW * character, 0, charW, charH), GraphicsUnit.Pixel);

    if ((now - lastChange).TotalMilliseconds >= (animationLength/framesInSheet))
    {
        character++;
        lastChange = DateTime.Now;
    }

    pb.Refresh();
};

form.Load += delegate
{
    bmp = new Bitmap(pb.Width, pb.Height);
    g = Graphics.FromImage(bmp);
    g.Clear(Color.Red);
    pb.Image = bmp;
    tm.Start();
};

form.KeyPreview = true;
form.KeyDown += (s, e) =>
{
    switch(e.KeyCode)
    {
        case Keys.Escape:
            Application.Exit();
            break;
        
        case Keys.Right:
            sonic = new Bitmap(new Bitmap("../sonicAndano.gif"), new Size(sonic.Width, sonic.Height));
            framesInSheet = 8;
            charW = (sonic.Width/framesInSheet);
            charH = (sonic.Height);
            animationLength = 500;
            break;

        case Keys.Left:
            sonic = new Bitmap(new Bitmap("../sonicAndanoProOtoLado.gif"), new Size(sonic.Width, sonic.Height));
            framesInSheet = 8;
            charW = (sonic.Width/framesInSheet);
            charH = (sonic.Height);
            animationLength = 500;
            break;
    }
};

form.KeyUp += (s, e) =>
{
    switch(e.KeyCode)
    {
        case Keys.Right:
            sonic = new Bitmap(new Bitmap("../sonic3.gif"), new Size(sonic.Width, sonic.Height));
            framesInSheet = 8;
            charW = (sonic.Width/framesInSheet);
            charH = (sonic.Height);
            animationLength = 1000;
            break;

        case Keys.Left:
            sonic = new Bitmap(new Bitmap("../sonic3.gif"), new Size(sonic.Width, sonic.Height));
            framesInSheet = 8;
            charW = (sonic.Width/framesInSheet);
            charH = (sonic.Height);
            animationLength = 1000;
            break;
    }
};

Application.Run(form);