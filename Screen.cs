using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public abstract class Screen
{
    public Form forms { get; set; }
    public Timer timer { get; set; }
    public int interval { get; set; }
    public DateTime latestChange { get; set; }
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

        this.DetectKeys();
        this.InstantiateObjects();

        this.timer.Tick += delegate
        {
            var now = DateTime.Now;

            if ((now - this.latestChange).Milliseconds >= 20)
            {
                this.UpdateKeys();   
            }

            this.LoadSprites(now);
        };

        this.forms.Load += delegate 
        {
            this.LoadForm();
        };

        Application.Run(forms);
    }

    protected abstract void LoadForm();
    protected abstract void LoadSprites(DateTime now);
    protected abstract void UpdateKeys();
    protected abstract void InstantiateObjects();
    protected abstract void DetectKeys();
}

public class SpriteScreen : Screen
{
    public Graphics g { get; set; } = null;
    public Bitmap bmp { get; set; }
    public PlayerSprite player { get; set; } = new PlayerSprite();
    public Monitor monitor { get; set; } = new Monitor();
    public Map map { get; set; } = new Map();
    public PictureBox pb { get; set; } = null;
    public int character { get; set; } = 0;
    public Queue<DateTime> queue { get; set; }= new Queue<DateTime>();
    public int animationLenght { get; set; } = 300;
    public int direction { get; set; } = 0;
    public int mapX { get; set; }
    public int mapY { get; set; }
    public int spriteX { get; set; }
    public int spriteY { get; set; }
    public DateTime latestSpriteChange { get; set; } = DateTime.Now;
    public SpriteScreen(ScreenArgs args)
    {
        this.interval = args.interval;
    }
    protected override void DetectKeys()
    {
        this.forms.KeyDown += (s, e) =>
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;

                case Keys.D:
                case Keys.Right:
                    Game.Current.player.moveRight = true;
                    this.direction = 3;
                    break;

                case Keys.A:
                case Keys.Left:
                    Game.Current.player.moveLeft = true;
                    this.direction = 2;
                    break;

                case Keys.W:
                case Keys.Up:
                    Game.Current.player.moveUp = true;
                    this.direction = 1;
                    break;

                case Keys.S:
                case Keys.Down:
                    Game.Current.player.moveDown = true;
                    this.direction = 0;
                    break;

                case Keys.ShiftKey:
                    Game.Current.player.running = true;
                    break;
            }
        };

        this.forms.KeyUp += (s, e) =>
        {
            switch(e.KeyCode)
            {
                case Keys.D:
                case Keys.Right:
                    Game.Current.player.moveRight = false;
                    break;

                case Keys.A:
                case Keys.Left:
                    Game.Current.player.moveLeft = false;
                    break;

                case Keys.W:
                case Keys.Up:
                    Game.Current.player.moveUp = false;
                    break;

                case Keys.S:
                case Keys.Down:
                    Game.Current.player.moveDown = false;
                    break;

                case Keys.ShiftKey:
                    Game.Current.player.running = false;
                    break;
            }
        };
    }

    protected override void InstantiateObjects()
    {
        this.pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        this.forms.Controls.Add(pb);
    }

    protected override void LoadForm()
    {
        this.bmp = new Bitmap(this.pb.Width, this.pb.Height);
        this.queue.Enqueue(DateTime.Now);
        this.g = Graphics.FromImage(bmp);
        this.g.Clear(Color.Black);
        this.pb.Image = bmp;
        this.timer.Start();
    }

    protected override void UpdateKeys()
    {
        Game.Current.player.Move(
            this.map.spriteW, 
            this.map.spriteH,
            this.player.spriteW,
            this.player.spriteH
        );
        this.latestChange = DateTime.Now;
    }

    protected override void LoadSprites(DateTime now)
    {  
        this.g.Clear(Color.Black);

        this.DrawMap();
        this.DrawPlayer(now, this.latestSpriteChange);
        this.DrawMonitor();
        this.DrawInfo(now);

        this.pb.Refresh();
    }

    private void DrawMap()
    {
        int xPosition = Game.Current.player.x - 960 + this.player.spriteW/2;
        int yPosition = Game.Current.player.y - 540 + this.player.spriteH/2;

        if (Game.Current.player.x < 960)
            xPosition = 0;
        else if (Game.Current.player.x > this.map.spriteW - 960)
            xPosition = this.map.spriteW - 1920 + this.player.spriteW/2;

        if (Game.Current.player.y < 540)
            yPosition = 0;
        else if (Game.Current.player.y > this.map.spriteH - 540)
            yPosition = this.map.spriteH - 1080 + this.player.spriteH/2;
        

        this.g.DrawImage(
            this.map.image,
            0,
            0,
            new Rectangle(
                xPosition, 
                yPosition, 
                this.monitor.spriteW, 
                this.monitor.spriteH
            ),
            GraphicsUnit.Pixel
        );

        this.mapX = xPosition;
        this.mapY = yPosition;
    }

    private void DrawPlayer(DateTime now, DateTime latestChange)
    {
        int xPosition = 960 - (this.player.spriteW / 2), 
            yPosition = 540 - (this.player.spriteH / 2); 

        if (Game.Current.player.x < xPosition)
            xPosition = Game.Current.player.x - this.player.spriteW/2;
        
        if (Game.Current.player.x > this.map.spriteW - xPosition)
            // xPosition = 960 + this.map.spriteW - Game.Current.player.x - this.player.spriteW/2;

        if (Game.Current.player.y < yPosition || Game.Current.player.y > this.map.spriteH - yPosition)
            yPosition = Game.Current.player.y - (this.player.spriteH / 2);

        if (this.character >= this.player.columns)
            this.character = 0;

        this.g.DrawImage(
            this.player.image, 
            xPosition, 
            yPosition, 
            new Rectangle(
                this.player.spriteW * this.character, 
                this.player.spriteH * this.direction, 
                this.player.spriteW, this.player.spriteH
            ), 
            GraphicsUnit.Pixel
        );        

        if ((now - latestChange).Milliseconds >= this.animationLenght/this.player.columns)
        {
            this.character++;
            this.latestSpriteChange = DateTime.Now;
        }

        this.spriteX = xPosition;
        this.spriteY = yPosition;
    }

    private void DrawMonitor()
    {
        this.g.DrawImage
        (
            this.monitor.image,
            0,
            0,
            new Rectangle(0, 0, this.monitor.spriteW, this.monitor.spriteH),
            GraphicsUnit.Pixel
        );
    }

    private void DrawInfo(DateTime now)
    {
        double fps = 0;
        this.queue.Enqueue(now);

        if (this.queue.Count > 19)
        {
            DateTime old = this.queue.Dequeue();
            var time = now - old;
            fps = (int)(19 / time.TotalSeconds);
        }
        
        string info = "----- FPS: " + fps.ToString() + 
            "----- playerX: " + 
            Game.Current.player.x + 
            "----- playerY: " + 
            Game.Current.player.y +
            "---- mapX: " + this.mapX.ToString() +
            "----- mapY:" + this.mapY.ToString() +
            "----- spriteX: " + this.spriteX.ToString() +
            "----- spriteY: " + this.spriteY.ToString();

        this.g.DrawString(
            info,
            new Font("Arial", 25), 
            new SolidBrush(Color.White), 
            new RectangleF(0, 0, 300, 700), 
            new StringFormat()
        );
    }
}
