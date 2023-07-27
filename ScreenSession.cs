using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

public abstract class ScreenSession
{
    public int animationLenght { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public ScreenSession nextScreen { get; set; }
    public bool isFinished { get; set; }
    public abstract void DrawScreen(Graphics g, PictureBox pb, DateTime now);
}

public class GameScreen : ScreenSession
{
    public int mapX { get; set; }
    public int mapY { get; set; }
    public int yPositionT { get; set; }
    public int xPositionT { get; set; }
    public AnimatedSprite brain { get; set; } = new BrainSprite();
    public Sprite xBtn { get; set; } = new ButtonXSprite();
    public Sprite monitor { get; set; } = new MonitorSprite();
    public Queue<DateTime> queue { get; set; } = new Queue<DateTime>();
    public DateTime now { get; set; }
    public DateTime latestChange { get; set; }
    public bool drawArrow { get; set; } = true;
    public GameScreen()
    {
        this.queue.Enqueue(DateTime.Now);
    }

    public override void DrawScreen(Graphics g, PictureBox pb, DateTime now)
    {
        this.now = now;

        if ((this.now - this.latestChange).TotalMilliseconds >= 20)
        {
            this.MovePlayer();   
            this.MoveEnemies();
        }

        this.LoadSprites(g, pb);

        if (Game.Current.player.stress >= 100)
        {
            Game.Current.player.stress = 0;
            Game.Current.player.canMove = true;
            this.isFinished = true;
            this.nextScreen = Game.Current.loading;
            this.nextScreen.nextScreen = Game.Current.shop;
            this.nextScreen.nextScreen.isFinished = false;
            this.nextScreen.isFinished = false;
            Game.Current.glitches = new List<Glitch>();
            Game.Current.player.canRun = false;
            Game.Current.player.relaxed = false;
            Game.Current.rent.UpdateRound();
        }

        Game.Current.player.Work(now);
    }
    protected void LoadSprites(Graphics g, PictureBox pb)
    {  
        g.Clear(Color.Black);
        Game.Current.player.playerSprite.UpdateSprites();

        this.DrawMap(g);
        this.DrawEnemies(g);
        this.DrawPlayer(g);
        this.DrawStatus(g);
        this.DrawMonitor(g, pb);
        // this.DrawInfo(g);

        pb.Refresh();
    }

    // private void DrawInfo(Graphics g)
    // {
    //     double fps = 0;
    //     this.queue.Enqueue(this.now);

    //     if (this.queue.Count > 19)
    //     {
    //         DateTime old = this.queue.Dequeue();
    //         var time = this.now - old;
    //         fps = (int)(19 / time.TotalSeconds);
    //     }

    //     string info = "----- FPS: " + fps.ToString();
    //         // "----- nearestEnemyX: " + Game.Current.NearestGlitch().X + 
    //         // "----- nearestEnemyY: " + Game.Current.NearestGlitch().Y +
    //         // "--- angle: " + (int)(Game.Current.player.arrow.angle) +
    //         // "----- quadrant: " + Game.Current.player.arrow.quadrant;

    //     g.DrawString(
    //         info,
    //         new Font("Arial", 25), 
    //         new SolidBrush(Color.White), 
    //         new RectangleF(0, 0, 300, 700), 
    //         new StringFormat()
    //     );
    // }

    private void DrawMap(Graphics g)
    {
        var startingTime = DateTime.Now;
        var player = Game.Current.player;

        int xPosition = player.x - this.monitor.spriteW/2 + player.playerSprite.spriteW/2;
        int yPosition = player.y - this.monitor.spriteH/2 + player.playerSprite.spriteH/2;

        var secondTime = DateTime.Now;
        
        if (player.x < this.monitor.spriteW/2 - player.playerSprite.spriteW/2)
            xPosition = 0;
        else if (player.x > Game.Current.map.spriteW - this.monitor.spriteW/2)
            xPosition = Game.Current.map.spriteW - this.monitor.spriteW + player.playerSprite.spriteW/2;

        if (player.y < this.monitor.spriteH/2 - player.playerSprite.spriteH/2)
            yPosition = 0;
        else if (player.y > Game.Current.map.spriteH - this.monitor.spriteH/2)
            yPosition = Game.Current.map.spriteH - this.monitor.spriteH + player.playerSprite.spriteH/2;

        g.DrawImage(
            Game.Current.map.image,
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
        this.x = mapX;
        this.mapY = yPosition;
        this.y = mapY;
    }

    private void DrawPlayer(Graphics g)
    {
        Game.Current.player.playerSprite.UpdateSprites(this.now);
        var player = Game.Current.player;

        int xPosition = this.monitor.spriteW/2 - (player.playerSprite.spriteW / 2), 
            yPosition = this.monitor.spriteH/2 - (player.playerSprite.spriteH / 2); 

        if (player.x < xPosition || player.x > Game.Current.map.spriteW - xPosition - player.playerSprite.spriteW/2)
            xPosition = player.x - this.mapX;

        if (player.y < yPosition || player.y > Game.Current.map.spriteH - yPosition - player.playerSprite.spriteH/2)
            yPosition = player.y - this.mapY;

        if (Game.Current.player.canUseStackOverflow && drawArrow)
        {
            var nearestEnemy = Game.Current.NearestGlitch();
            Game.Current.player.arrow.objective = nearestEnemy;
            var arrow = Game.Current.player.arrow.SetAngle();
    
            g.DrawImage
            (
                arrow,
                xPosition + Game.Current.player.playerSprite.spriteW/2 - arrow.Width/2, 
                yPosition + Game.Current.player.playerSprite.spriteH/2 - arrow.Height/2, 
                new Rectangle
                (
                    0,
                    0,
                    arrow.Width,
                    arrow.Height
                ),
                GraphicsUnit.Pixel
            );
            
        }
        this.drawArrow = true;

        g.DrawImage(
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

        Game.Current.player.mapY = yPosition;
        Game.Current.player.mapX = xPosition;
    }

    protected void MovePlayer()
    {
        Game.Current.player.Move(
            Game.Current.map.spriteW, 
            Game.Current.map.spriteH,
            Game.Current.player.playerSprite.spriteW,
            Game.Current.player.playerSprite.spriteH
        );
        this.latestChange = DateTime.Now;
    }

    protected void MoveEnemies()
    {
        var glitches = Game.Current.glitches;

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

    private void DrawEnemies(Graphics g)
    {
    
        for (int i = 0; i < Game.Current.glitches.Count; i++)
        {
            var glitch = Game.Current.glitches[i];

            for (int j = 0; j < glitch.bugs.Count; j++)
                this.DrawEnemy(glitch.bugs[j], g);

            if (Game.Current.player.InReachOfEnemy(glitch))
            {
                this.DrawButton(glitch, g);
                this.drawArrow = false;
            
                if (Game.Current.player.hasHackerVision)
                {
                    this.DrawEnemyInfo(glitch, g);
                }
            }   


            if (glitch.inMinigame)
            {
                this.drawArrow = false;
                this.DrawMinigame(glitch, g);
                glitch.Debug(this.now);
            }

            this.DrawEnemy(glitch, g);
        }
    }

    private void DrawMinigame(Enemy enemy, Graphics g)
    {
        g.DrawImage
        (
            enemy.miniGame.structure.image,
            enemy.miniGame.x,
            enemy.miniGame.y,
            new Rectangle(0, 0, enemy.miniGame.structure.spriteW, enemy.miniGame.structure.spriteH),
            GraphicsUnit.Pixel
        );

        foreach (var componenet in enemy.miniGame.components)
        {
            g.DrawImage
            (
                componenet.image,
                componenet.x,
                componenet.y,
                new Rectangle(0, 0, componenet.spriteW, componenet.spriteH),
                GraphicsUnit.Pixel
            );
        }
    }

    private void DrawButton(Enemy enemy, Graphics g)
    {
        g.DrawImage
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
    }

    private void DrawEnemy(Enemy enemy, Graphics g)
    {
        enemy.enemySprite.UpdateSprites(this.now);

        if (
            enemy.x - mapX > -enemy.enemySprite.spriteW && 
            enemy.x - mapX < (this.mapX + Game.Current.map.spriteW) + enemy.enemySprite.spriteW &&
            enemy.y - mapY > -enemy.enemySprite.spriteW &&
            enemy.y - mapY < (this.mapY + Game.Current.map.spriteH) + enemy.enemySprite.spriteH
        )
            g.DrawImage
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
                GraphicsUnit.Pixel
            );
    }

    private void DrawEnemyInfo(Enemy enemy, Graphics g)
    {
        g.DrawString
        (
            $"Lv: {enemy.lv} - Reward: $ {enemy.value}",
            new Font("Arial", 15),
            new SolidBrush(Color.White),
            enemy.x + enemy.enemySprite.spriteW/2 - this.mapX - 100,
            enemy.y - enemy.enemySprite.spriteH * 3 + 100 - this.mapY,
            new StringFormat()
        );
    }

    private void DrawMonitor(Graphics g, PictureBox pb)
    {
        Game.Current.pcHealth.Update(pb);

        switch (Game.Current.pcHealth.health)
        {
            case 2: 
                Game.Current.pcHealth.DrawError(g);
                break;

            case 1:
                Game.Current.pcHealth.DrawError(g);
                Game.Current.pcHealth.DrawAd(g);
                break;

            case 0:
                Game.Current.pcHealth.DrawError(g);
                Game.Current.pcHealth.DrawAd(g);
                break;

            default:
                break;
        }

        g.DrawImage
        (
            this.monitor.image,
            0,
            0,
            new Rectangle(0, 0, this.monitor.spriteW, this.monitor.spriteH),
            GraphicsUnit.Pixel
        );
    }

    private void DrawStatus(Graphics g)
    {
        this.brain.UpdateSprites(this.now);

        g.DrawImage(
            brain.image,
            100,
            900,
            new Rectangle(brain.spriteW * brain.currentColumn, brain.spriteH * brain.currentRow, brain.spriteW, brain.spriteH),
            GraphicsUnit.Pixel
        );

        g.DrawString(
            (Game.Current.player.stress.ToString() + "%"),
            new Font("Arial", 25),
            new SolidBrush(Color.White),
            190,
            915,
            new StringFormat()
        );
    }
}

public class Intro : ScreenSession
{
    public int currentFrame { get; set; } = 1;
    public int totalFrames { get; set; } = 200;
    public bool videoStarted { get; set; } = false;
    public DateTime latestUpdate { get; set; } = DateTime.Now;
    public SoundPlayer simpleSound { get; set; } = new SoundPlayer("./introFiles/bg.wav");
    public DateTime start { get; set; }
    public override void DrawScreen(Graphics g, PictureBox pb, DateTime now)
    {
        if (!videoStarted)
        {
            this.videoStarted = true;
            this.simpleSound.Play();
            this.start = DateTime.Now;
        }
        else 
        {
            var keyMap = Game.Current.keymap;
            var index = ((this.currentFrame < 10 ? "00" : this.currentFrame < 100 ? "0" : "") + this.currentFrame.ToString());
            var frame = new Bitmap(new Bitmap($"./introFiles/{index}.png"), 1920, 1080);

            if (keyMap.keyMapping[Keys.Space] || (DateTime.Now - this.start).TotalMilliseconds >= 22000)
            {
                this.simpleSound.Stop();
                this.isFinished = true;
            }

            g.DrawImage
            (
                frame,
                0,
                0,
                new Rectangle(
                    0, 
                    0, 
                    frame.Width,
                    frame.Height
                ),
                GraphicsUnit.Pixel
            );

            if ((DateTime.Now - this.latestUpdate).TotalMilliseconds >= 74.5)
            {
                this.currentFrame++;
                this.latestUpdate = DateTime.Now;
            }

            this.currentFrame = this.currentFrame >= this.totalFrames ? this.totalFrames : this.currentFrame;
        }
    }
}

public class Menu : ScreenSession
{
    public AnimatedSprite menu { get; set; } = new MenuSprite();
    public Sprite playBtn { get; set; } = new PlayBtnSprite();
    public bool mouseClick { get; set; } = false;
    public Point cursorLocation { get; set; }
    public int currentFrame { get; set; } = 0;
    public bool musicStarted { get; set; } = false;
    public DateTime musicStart { get; set; }
    public SoundPlayer outSoud { get; set; } = new SoundPlayer("./sounds/outMenu.wav");
    public SoundPlayer bgMusic { get; set; } = new SoundPlayer("./sounds/bgMenu.wav");
    public override void DrawScreen(Graphics g, PictureBox pb, DateTime now)
    {
        
        if (!musicStarted)
        {
            musicStart = DateTime.Now;
            musicStarted = true;
            this.bgMusic.Play();
        }

        if ((now - musicStart).TotalMilliseconds >= 349000)
            musicStarted = false;

        pb.MouseDown += (s, e) =>
        {
            this.mouseClick = true;
        };
        
        pb.MouseUp += (s, e) =>
        {
            this.mouseClick = false;
        };

        pb.MouseMove += (s, e) =>
        {
            this.cursorLocation = e.Location;
        };

        menu.UpdateSprites(now);

        g.DrawImage
        (
            menu.image,
            0,
            0,
            new Rectangle(
                menu.spriteW * menu.currentColumn, 
                menu.spriteH * menu.currentRow, 
                menu.spriteW,
                menu.spriteH
            ),
            GraphicsUnit.Pixel
        );

        g.DrawImage
        (
            playBtn.image,
            960 - playBtn.spriteW/2,
            540 - playBtn.spriteH/2,
            new Rectangle
            (
                0,
                0,
                playBtn.spriteW,
                playBtn.spriteH
            ),
            GraphicsUnit.Pixel
        );

        if (
            this.mouseClick &&
            this.cursorLocation.X <= (960 - playBtn.spriteW/2 + playBtn.spriteW) &&
            this.cursorLocation.X >= (960 - playBtn.spriteW/2) &&
            this.cursorLocation.Y <= (540 - playBtn.spriteH/2 + playBtn.spriteH) &&
            this.cursorLocation.Y >= (540 - playBtn.spriteH/2)
        )
        {
            this.isFinished = true;
            this.bgMusic.Stop();
            this.outSoud.Play();
        };
    }
}

public class Shop : ScreenSession
{
    public Sprite monitor { get; set; } = new MonitorSprite();
    public Sprite shopBg { get; set; } = new ShopSprite();
    public ShopBtn huntBtn { get; set; } = new HuntBtn();
    public ShopBtn[] buyButtons { get; set; } = new ShopBtn[6]{ 
        new UpgradeMonitorBtn(225), 
        new UpgradePcBtn(250), 
        new BuyCourseBtn(500), 
        new RepairPcBtn(150), 
        new BuyCoffeBtn(20), 
        new BuyCinemaTicket(25)
    };
    public bool mouseClick { get; set; } = false;
    public Point cursorLocation { get; set; }
    public override void DrawScreen(Graphics g, PictureBox pb, DateTime now)
    {
        pb.MouseDown += (s, e) =>
        {
            this.mouseClick = true;
        };
        
        pb.MouseUp += (s, e) =>
        {
            this.mouseClick = false;
        };

        pb.MouseMove += (s, e) =>
        {
            this.cursorLocation = e.Location;
        };

        if (
            mouseClick &&
            this.cursorLocation.X <= (huntBtn.x + huntBtn.btnSprite.spriteW) &&
            this.cursorLocation.X >= huntBtn.x &&
            this.cursorLocation.Y <= (huntBtn.y + huntBtn.btnSprite.spriteH) &&
            this.cursorLocation.Y >= huntBtn.y
        )
        {
            this.isFinished = true;
            this.nextScreen = Game.Current.loading;
            this.nextScreen.nextScreen = Game.Current.gameScreen;
            this.nextScreen.isFinished = false;
            this.nextScreen.nextScreen.isFinished = false;

            Game.Current.SpawnGlitchs(
                Game.Current.startingGlitchs, 
                Game.Current.map.spriteW, 
                Game.Current.map.spriteH, 
                Game.Current.player.playerSprite.spriteW, 
                Game.Current.player.playerSprite.spriteH, 
                false
            );
        }

        g.DrawImage
        (
            shopBg.image,
            66,
            66,
            new Rectangle
            (
                0,
                0,
                shopBg.spriteW,
                shopBg.spriteH
            ),
            GraphicsUnit.Pixel
        );

        var brush = new SolidBrush(Color.Black);

        foreach (var btn in buyButtons)
        {
            g.DrawImage
            (
                btn.btnSprite.image,
                btn.x,
                btn.y,
                new Rectangle
                (
                    0,
                    0,
                    btn.btnSprite.spriteW,
                    btn.btnSprite.spriteH
                ),
                GraphicsUnit.Pixel
            );

            if (
                mouseClick &&
                this.cursorLocation.X <= (btn.x + btn.btnSprite.spriteW) &&
                this.cursorLocation.X >= btn.x &&
                this.cursorLocation.Y <= (btn.y + btn.btnSprite.spriteH) &&
                this.cursorLocation.Y >= btn.y
            )
            {
                btn.Buy();
            }

            g.DrawString
            (
                ("$" + btn.cost.ToString()),
                new Font("Arial", 30),
                brush,
                btn.x - 350,
                btn.y,
                new StringFormat()
            );
        }

        g.DrawString
        (
            Game.Current.player.money.ToString(),
            new Font("Arial", 80),
            new SolidBrush(Color.White),
            210,
            830,
            new StringFormat()
        );

        g.DrawString
        (
            $"Rent: ${Game.Current.rent.cost} - Payday in {Game.Current.rent.rounds - Game.Current.rent.currentRound} rounds",
            new Font("Arial", 25),
            new SolidBrush(Color.White),
            210,
            800,
            new StringFormat()
        );

        g.DrawImage
        (
            huntBtn.btnSprite.image,
            huntBtn.x,
            huntBtn.y,
            new Rectangle(0, 0, huntBtn.btnSprite.spriteW, huntBtn.btnSprite.spriteH),
            GraphicsUnit.Pixel
        );

        g.DrawImage
        (
            monitor.image,
            0,
            0,
            new Rectangle(0, 0, monitor.spriteW, monitor.spriteH),
            GraphicsUnit.Pixel
        );
    }
}


public class Loading : ScreenSession
{
    public AnimatedSprite loading { get; set; } = new LoadingSprite();
    public Sprite monitor { get; set; } = new MonitorSprite();
    public override void DrawScreen(Graphics g, PictureBox pb, DateTime now)
    {
        loading.UpdateSprites(now);

        g.DrawImage
        (
            loading.image,
            0,0,
            new Rectangle(
                loading.spriteW * loading.currentColumn, 
                loading.spriteH * loading.currentRow, 
                loading.spriteW,
                loading.spriteH
            ),
            GraphicsUnit.Pixel
        );

        g.DrawImage
        (
            this.monitor.image,
            0,
            0,
            new Rectangle(0, 0, this.monitor.spriteW, this.monitor.spriteH),
            GraphicsUnit.Pixel
        );

        if (loading.currentColumn == loading.columns - 1 && loading.currentRow == loading.rows - 1)
        {
            this.isFinished = true;
            loading.currentRow = 0;
            loading.currentColumn = 0;
        }
    }
}