using System;
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

// public class DefaultScreen : Screen
// {
//     public Panel pl { get; set; }
//     public DefaultScreen(ScreenArgs args)
//     {
//         this.interval = args.interval;
//     }

//     protected override void DetectKeys()
//     {
//         this.forms.KeyDown += (s, e) =>
//         {
//             switch(e.KeyCode)
//             {
//                 case Keys.Escape:
//                     Application.Exit();
//                     break;

//                 case Keys.D:
//                 case Keys.Right:
//                     Game.Current.player.moveRight = true;
//                     break;

//                 case Keys.A:
//                 case Keys.Left:
//                     Game.Current.player.moveLeft = true;
//                     break;

//                 case Keys.W:
//                 case Keys.Up:
//                     Game.Current.player.moveUp = true;
//                     break;

//                 case Keys.S:
//                 case Keys.Down:
//                     Game.Current.player.moveDown = true;
//                     break;

//                 case Keys.ShiftKey:
//                     Game.Current.player.running = true;
//                     break;
//             }
//         };

//         this.forms.KeyUp += (s, e) =>
//         {
//             switch(e.KeyCode)
//             {
//                 case Keys.D:
//                 case Keys.Right:
//                     Game.Current.player.moveRight = false;
//                     break;

//                 case Keys.A:
//                 case Keys.Left:
//                     Game.Current.player.moveLeft = false;
//                     break;

//                 case Keys.W:
//                 case Keys.Up:
//                     Game.Current.player.moveUp = false;
//                     break;

//                 case Keys.S:
//                 case Keys.Down:
//                     Game.Current.player.moveDown = false;
//                     break;

//                 case Keys.ShiftKey:
//                     Game.Current.player.running = false;
//                     break;
//             }
//         };
//     }

//     protected override void InstantiateObjects()
//     {
//         this.pl = new Panel();
//         pl.BackColor = Color.White;
//         pl.Width = 100;
//         pl.Height = 100;
//         this.forms.Controls.Add(pl);
//     }

//     protected override void LoadForm()
//     {
//         this.timer.Start();
//     }

//     protected override void LoadSprites(DateTime now)
//     {
//     }

//     protected override void UpdateKeys()
//     {
//         Game.Current.player.Move();
//         this.pl.Location = new Point(Game.Current.player.x, Game.Current.player.y);
//         this.latestChange = DateTime.Now;
//     }
// }

public class SpriteScreen : Screen
{
    public Graphics g { get; set; } = null;
    public Bitmap bmp { get; set; }
    public PlayerSprite player { get; set; } = new PlayerSprite();
    public PictureBox pb { get; set; } = null;
    public int character { get; set; } = 0;
    public int animationLenght { get; set; } = 500;
    public int direction { get; set; } = 0;
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
        this.g = Graphics.FromImage(bmp);
        this.g.Clear(Color.Black);
        this.pb.Image = bmp;
        this.timer.Start();
    }

    protected override void UpdateKeys()
    {
        Game.Current.player.Move();
        this.latestChange = DateTime.Now;
    }

    protected override void LoadSprites(DateTime now)
    {  
        this.g.Clear(Color.White);

        if (this.character >= this.player.columns)
            this.character = 0;

        this.g.DrawImage(
            this.player.image, 
            Game.Current.player.x, 
            Game.Current.player.y, 
            new Rectangle(this.player.spriteW * this.character, this.player.spriteH * this.direction, this.player.spriteW, this.player.spriteH), 
            GraphicsUnit.Pixel
        );        

        if ((now - this.latestSpriteChange).Milliseconds >= this.animationLenght/this.player.columns)
        {
            this.character++;
            this.latestSpriteChange = DateTime.Now;
        }

        this.pb.Refresh();
    }
}
