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
    public Sprite map { get; set; } = new MapSprite();
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
            this.now = DateTime.Now;

            if ((now - this.latestChange).TotalMilliseconds >= 20)
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
    protected abstract void MoveEnemies();
}

public class SpriteScreen : Screen
{
    public Graphics g { get; set; } = null;
    public Bitmap bmp { get; set; }
    public Sprite monitor { get; set; } = new MonitorSprite();
    public BrainSprite brain { get; set; } = new BrainSprite();
    public ButtonXSprite xBtn { get; set; } = new ButtonXSprite();
    public PictureBox pb { get; set; } = null;
    public Queue<DateTime> queue { get; set; } = new Queue<DateTime>();
    public int mapX { get; set; }
    public int mapY { get; set; }
    public int spriteX { get; set; }
    public int spriteY { get; set; }
    public int tempEnemyX { get; set; }
    public int tempEnemyY { get; set; }
    public int tempBtnX { get; set; } = 0;
    public int tempBtnY { get; set; } = 0;
    public DateTime latestSpriteChange { get; set; } = DateTime.Now;
    public int timeAtIfs { get; set; }
    public int totalTimeMap { get; set; }
    public int imageSizeX { get; set; }
    public int imageSizeY { get; set; }
    public SpriteScreen(ScreenArgs args)
    {
        this.interval = args.interval;
    }
    protected override void LoadForm()
    {
        this.pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        this.forms.Controls.Add(pb);
        this.bmp = new Bitmap(this.pb.Width, this.pb.Height);
        this.queue.Enqueue(DateTime.Now);
        this.g = Graphics.FromImage(bmp);
        this.g.Clear(Color.Black);
        this.pb.Image = bmp;
        this.timer.Start();
        Game.Current.SpawnGlitchs(Game.Current.startingGlitchs, this.map.spriteW, this.map.spriteH, Game.Current.player.playerSprite.spriteW, Game.Current.player.playerSprite.spriteH);
    }

    protected override void MovePlayer()
    {
        Game.Current.player.Move(
            this.map.spriteW, 
            this.map.spriteH,
            Game.Current.player.playerSprite.spriteW,
            Game.Current.player.playerSprite.spriteH
        );
        this.latestChange = DateTime.Now;
    }

    protected override void MoveEnemies()
    {
        var glitches = Game.Current.glitches;

        // for (int i = 0; i < glitches.Count; i++)
        // {
        //     for (int j = 0; j < glitches[i].bugs.Count; j++)
        //     {
        //         glitches[i].bugs[j].Move(this.now);
        //         glitches[i].bugs[j].Atack(this.now);
        //     }
        //     glitches[i].Move(this.now);
        //     glitches[i].Atack(this.now);
        // }

        Parallel.For(0, glitches.Count, i => 
        {
            Parallel.For(0, glitches[i].bugs.Count, j => 
            {
                glitches[i].bugs[j].Move(this.now);
                glitches[i].bugs[j].Atack(this.now);
            });

            glitches[i].Move(this.now);
            glitches[i].Atack(this.now);
        });
    }

    protected override void LoadSprites(DateTime now)
    {  
        this.g.Clear(Color.Black);
        Game.Current.player.playerSprite.UpdateSprites();

        this.DrawMap();
        this.DrawEnemies();
        this.DrawPlayer();
        this.DrawStatus();
        this.DrawMonitor();
        this.DrawInfo(now);

        this.pb.Refresh();
    }

    private void DrawStatus()
    {
        this.brain.UpdateSprites(this.now);

        this.g.DrawImage(
            brain.image,
            100,
            900,
            new Rectangle(brain.spriteW * brain.currentColumn, brain.spriteH * brain.currentRow, brain.spriteW, brain.spriteH),
            GraphicsUnit.Pixel
        );

        this.g.DrawString(
            (Game.Current.player.stress.ToString() + "%"),
            new Font("Arial", 25),
            new SolidBrush(Color.White),
            190,
            915,
            new StringFormat()
        );
    }

    private void DrawEnemies()
    {
        for (int i = 0; i < Game.Current.glitches.Count; i++)
        {
            var glitch = Game.Current.glitches[i];

            for (int j = 0; j < glitch.bugs.Count; j++)
                this.DrawEnemy(glitch.bugs[j]);

            if (Game.Current.player.InReachOfEnemy(glitch))
                this.DrawButton(glitch);

            if (glitch.inMinigame)
            {
                this.DrawMinigame(glitch);
                glitch.Debug(this.now);
            }

            this.DrawEnemy(glitch);

            this.tempEnemyX = glitch.x;
            this.tempEnemyY = glitch.y;
        }
    }

    private void DrawMinigame(Enemy enemy)
    {
        this.g.DrawImage
        (
            enemy.miniGame.structure.image,
            enemy.miniGame.x,
            enemy.miniGame.y,
            new Rectangle(0, 0, enemy.miniGame.structure.spriteW, enemy.miniGame.structure.spriteH),
            GraphicsUnit.Pixel
        );

        foreach (var componenet in enemy.miniGame.components)
        {
            this.g.DrawImage
            (
                componenet.image,
                componenet.x,
                componenet.y,
                new Rectangle(0, 0, componenet.spriteW, componenet.spriteH),
                GraphicsUnit.Pixel
            );
        }
    }

    private void DrawButton(Enemy enemy)
    {
        this.g.DrawImage
        (
            this.xBtn.image,
            enemy.x + enemy.enemySprite.spriteW/2 - this.xBtn.spriteW/2 - this.mapX,
            enemy.y - enemy.enemySprite.spriteH * 3 - this.mapY,
            new Rectangle(
                0,
                0,
                this.xBtn.spriteW,
                this.xBtn.spriteH
            ),
            GraphicsUnit.Pixel
        );
        this.tempBtnX = enemy.x + enemy.enemySprite.spriteW/2 - this.xBtn.spriteW/2;
        this.tempBtnY = enemy.y - enemy.enemySprite.spriteH/2;
    }

    private void DrawEnemy(Enemy enemy)
    {
        enemy.enemySprite.UpdateSprites(this.now);

        if (
            enemy.x - mapX > -enemy.enemySprite.spriteW && 
            enemy.x - mapX < (this.mapX + this.map.spriteW) + enemy.enemySprite.spriteW &&
            enemy.y - mapY > -enemy.enemySprite.spriteW &&
            enemy.y - mapY < (this.mapY + this.map.spriteH) + enemy.enemySprite.spriteH
        )
            this.g.DrawImage
            (
                enemy.enemySprite.image,
                enemy.x - this.mapX,
                enemy.y - this.mapY,
                new Rectangle(
                    enemy.enemySprite.spriteW * enemy.enemySprite.currentColumn,
                    enemy.enemySprite.spriteH * enemy.enemySprite.currentRow,
                    enemy.enemySprite.spriteW,
                    enemy.enemySprite.spriteH
                ),
                // new Rectangle(
                //     enemy.enemySprite.spriteW * enemy.enemySprite.currentColumn,
                //     enemy.enemySprite.spriteH * enemy.enemySprite.currentRow,
                //     enemy.enemySprite.spriteW * enemy.enemySprite.currentColumn + enemy.enemySprite.spriteW,
                //     enemy.enemySprite.spriteH * enemy.enemySprite.currentRow + enemy.enemySprite.spriteH
                // ),
                GraphicsUnit.Pixel
            );
    }

    private void DrawMap()
    {
        var startingTime = DateTime.Now;
        var player = Game.Current.player;

        int xPosition = player.x - 960 + player.playerSprite.spriteW/2;
        int yPosition = player.y - 540 + player.playerSprite.spriteH/2;

        var secondTime = DateTime.Now;
        
        if (player.x < 960 - player.playerSprite.spriteW/2)
            xPosition = 0;
        else if (player.x > this.map.spriteW - 960)
            xPosition = this.map.spriteW - 1920 + player.playerSprite.spriteW/2;

        if (player.y < 540 - player.playerSprite.spriteH/2)
            yPosition = 0;
        else if (player.y > this.map.spriteH - 540)
            yPosition = this.map.spriteH - 1080 + player.playerSprite.spriteH/2;
        
        this.timeAtIfs = (int)(DateTime.Now - secondTime).TotalMilliseconds;

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

        this.totalTimeMap = (int)((DateTime.Now - startingTime).TotalMilliseconds);
        this.mapX = xPosition;
        this.mapY = yPosition;
    }

    private void DrawPlayer()
    {
        Game.Current.player.playerSprite.UpdateSprites(this.now);
        var player = Game.Current.player;

        int xPosition = 960 - (player.playerSprite.spriteW / 2), 
            yPosition = 540 - (player.playerSprite.spriteH / 2); 

        if (player.x < xPosition || player.x > this.map.spriteW - xPosition - player.playerSprite.spriteW/2)
            xPosition = player.x - this.mapX;

        if (player.y < yPosition || player.y > this.map.spriteH - yPosition - player.playerSprite.spriteH/2)
            yPosition = player.y - this.mapY;

        this.g.DrawImage(
            player.playerSprite.image, 
            xPosition, 
            yPosition, 
            new Rectangle(
                player.playerSprite.spriteW * player.playerSprite.currentColumn, 
                player.playerSprite.spriteH * player.playerSprite.currentRow, 
                player.playerSprite.spriteW, 
                player.playerSprite.spriteH
            ), 
            GraphicsUnit.Pixel
        );        

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
            // "----- playerX: " + Game.Current.player.x + 
            // "----- playerY: " + Game.Current.player.y +
            "---- mapX: " + this.mapX.ToString() +
            "----- mapY:" + this.mapY.ToString() +
            // "----- spriteX: " + this.spriteX.ToString() +
            // "----- spriteY: " + this.spriteY.ToString() +
            // "----- enemyX: " + this.tempEnemyX.ToString() +
            // "----- enemyY: " + this.tempEnemyY.ToString() +
            "--------- ifs: " + this.timeAtIfs.ToString() + 
            "----- mapTime: " + this.totalTimeMap.ToString();

        this.g.DrawString(
            info,
            new Font("Arial", 25), 
            new SolidBrush(Color.White), 
            new RectangleF(0, 0, 300, 700), 
            new StringFormat()
        );
    }
}
