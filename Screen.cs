using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public abstract class Screen
{
    public Form forms { get; set; }
    public Timer timer { get; set; }
    public int interval { get; set; }
    public DateTime latestChange { get; set; }
    public DateTime now { get; set;} = DateTime.Now;

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
            this.SetDirection();
            this.now = DateTime.Now;

            if ((now - this.latestChange).Milliseconds >= 20)
            {
                this.MovePlayer();   
                this.MoveEnemies();
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
    protected abstract void MovePlayer();
    protected abstract void SetDirection();
    protected abstract void MoveEnemies();
}

public class SpriteScreen : Screen
{
    public Graphics g { get; set; } = null;
    public Bitmap bmp { get; set; }
    public AnimatedSprite player { get; set; } = new PurplePlayerSprite();
    public Sprite monitor { get; set; } = new MonitorSprite();
    public Sprite map { get; set; } = new MapSprite();
    public AnimatedSprite glitch { get; set; } = new GlitchSprite();
    public int glitchColumn { get; set; } = 0;
    public int glitchRow { get; set; } = 0;
    public DateTime glitchLatestChange { get; set; } = DateTime.Now;
    public PictureBox pb { get; set; } = null;
    public int character { get; set; } = 0;
    public Queue<DateTime> queue { get; set; } = new Queue<DateTime>();
    public Random rnd { get; set; }= new Random();
    public int direction { get; set; } 
    public int mapX { get; set; }
    public int mapY { get; set; }
    public int spriteX { get; set; }
    public int spriteY { get; set; }
    public DateTime latestSpriteChange { get; set; } = DateTime.Now;
    public SpriteScreen(ScreenArgs args)
    {
        this.interval = args.interval;
    }
    protected override void SetDirection()
    {
        var keyMap = Game.Current.keymap.keyMapping;
        if (
            !(keyMap[Keys.Down] || 
            keyMap[Keys.Up] ||
            keyMap[Keys.Left] ||
            keyMap[Keys.Right])
        )
            this.direction = this.player.idle;
        else if (keyMap[Keys.Right])
            this.direction = this.player.right;
        else if (keyMap[Keys.Left])
            this.direction = this.player.left;
        else if (keyMap[Keys.Down])
            this.direction = this.player.down;
        else if (keyMap[Keys.Up])
            this.direction = this.player.up;

    }
    protected override void LoadForm()
    {
        this.pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        this.forms.Controls.Add(pb);
        this.direction = this.player.idle;
        this.bmp = new Bitmap(this.pb.Width, this.pb.Height);
        this.queue.Enqueue(DateTime.Now);
        this.g = Graphics.FromImage(bmp);
        this.g.Clear(Color.Black);
        this.pb.Image = bmp;
        this.timer.Start();
        Game.Current.enemies.SpawnGlitchs(3, this.map.spriteW, this.map.spriteH, this.player.spriteW, this.player.spriteH);
    }

    protected override void MovePlayer()
    {
        Game.Current.player.Move(
            this.map.spriteW, 
            this.map.spriteH,
            this.player.spriteW,
            this.player.spriteH
        );
        this.latestChange = DateTime.Now;
    }

    protected override void MoveEnemies()
    {

    }

    protected override void LoadSprites(DateTime now)
    {  
        this.g.Clear(Color.Black);
        this.DrawMap();
        this.DrawEnemies();
        this.DrawPlayer(now, this.latestSpriteChange);
        this.DrawMonitor();
        this.DrawInfo(now);
        this.pb.Refresh();
    }

    private void DrawEnemies()
    {
        if ((this.now - this.glitchLatestChange).Milliseconds > this.glitch.animationLenght)
        {
            if (this.glitchColumn == 3)
            {
                this.glitchColumn = 0;
                this.glitchRow++;
            }
            if (this.glitchRow == 3)
            {
                this.glitchRow = 0;
                this.glitchColumn = 0;
            }
            this.glitchColumn++;
        }

        foreach (var item in Game.Current.enemies.enemiesList.Select(x => x).Where(x => x.range == -1))
        {
            this.g.DrawImage
            (
                this.glitch.image,
                this.mapX + item.x,
                this.mapY + item.y,
                new Rectangle(
                    this.glitch.spriteW * this.glitchColumn,
                    this.glitch.spriteH * this.glitchRow,
                    this.glitch.spriteW,
                    this.glitch.spriteH
                ),
                GraphicsUnit.Pixel
            );
        }
    }

    private void DrawMap()
    {
        int xPosition = Game.Current.player.x - 960 + this.player.spriteW/2;
        int yPosition = Game.Current.player.y - 540 + this.player.spriteH/2;

        if (Game.Current.player.x < 960 - this.player.spriteW/2)
            xPosition = 0;
        else if (Game.Current.player.x > this.map.spriteW - 960)
            xPosition = this.map.spriteW - 1920 + this.player.spriteW/2;

        if (Game.Current.player.y < 540 - this.player.spriteH/2)
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

        if (Game.Current.player.x < xPosition || Game.Current.player.x > this.map.spriteW - xPosition - this.player.spriteW/2)
            xPosition = Game.Current.player.x - this.mapX;

        if (Game.Current.player.y < yPosition || Game.Current.player.y > this.map.spriteH - yPosition - this.player.spriteH/2)
            yPosition = Game.Current.player.y - this.mapY;

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

        if ((now - latestChange).Milliseconds >= this.player.animationLenght/this.player.columns)
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

    private void DrawGlitch()
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
