using System;
using System.Drawing;
using System.Windows.Forms;

public class ComputerHealth
{
    public Sprite adSprite { get; set; } = new AdSprite();
    public DateTime latestChange { get; set; } = DateTime.Now;
    public DateTime latestAd { get; set; } = DateTime.Now;
    public (Point, Point ) addX { get; set; }
    public bool DrawBlackScreen { get; set; } = false;
    public bool drawAd { get; set; } = false;
    public int health { get; set; } = 3;
    public bool mouseClick { get; set; } = false;
    public Point cursorLocation { get; set; }

    public ComputerHealth()
    {
        var point1 = new Point(1409, 212);
        var point2 = new Point(1442, 245);
        addX = (point1, point2); 
    }
    public void Update(PictureBox pb)
    {
        if (this.health > 1)
            this.drawAd = false;
        
        if (this.health > 2)
            this.DrawBlackScreen = false;

        this.cursorLocation = Game.Current.keymap.cursorLocation;
        this.mouseClick = Game.Current.keymap.mouseClick;

        if (
            this.mouseClick &&
            this.cursorLocation.X >= addX.Item1.X &&
            this.cursorLocation.X <= addX.Item2.X &&
            this.cursorLocation.Y >= addX.Item1.Y &&
            this.cursorLocation.Y <= addX.Item2.Y &&
            drawAd
        )
        {
            this.drawAd = false;
        };
    }
    public void DrawError(Graphics g)
    {
        if ((DateTime.Now - this.latestChange).TotalMilliseconds >= 5000)
        {
            this.latestChange = DateTime.Now;
            
            if (Game.Current.rnd.Next(0, 3) > 1)
                this.DrawBlackScreen = true; 
        }
        if (this.DrawBlackScreen)
        {
            g.Clear(Color.Black);
        }
        if ((DateTime.Now - this.latestChange).TotalMilliseconds >= 500 && this.DrawBlackScreen)
        {
            this.DrawBlackScreen = false;
        }
    }

    public void DrawAd(Graphics g)
    {
        if ((DateTime.Now - this.latestAd).TotalMilliseconds >= 15000)
        {
            this.drawAd = true;
            this.latestAd = DateTime.Now;
        }

        if (this.drawAd)
        {
            g.DrawImage
            (
                this.adSprite.image,
                (960 - (this.adSprite.spriteW/2)),
                (540 - (this.adSprite.spriteH/2)),
                new Rectangle(0, 0, this.adSprite.spriteW, this.adSprite.spriteH),
                GraphicsUnit.Pixel
            );
        }
    }
}