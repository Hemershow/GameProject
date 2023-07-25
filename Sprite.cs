using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public abstract class Sprite 
{
    public Bitmap image { get; set; }
    public int spriteW { get; set; }
    public int spriteH { get; set; }
    public int x { get; set; }
    public int y { get; set; } 
}

public abstract class AnimatedSprite 
{
    public Bitmap image { get; set; }
    public int spriteW { get; set; }
    public int spriteH { get; set; }
    public int columns { get; set; }
    public int rows { get; set; }
    public int up { get; set; } 
    public int down { get; set; }
    public int left { get; set; }
    public int right { get; set; }
    public int idle { get; set; }
    public int animationLenght { get; set; }
    public int currentRow { get; set; } = 0;
    public int currentColumn { get; set; } = 0;
    public DateTime latestChange { get; set; } = DateTime.Now;
    public virtual void UpdateSprites(DateTime now)
    {
        if ((now - this.latestChange).TotalMilliseconds > this.animationLenght/this.columns)
        {
            this.currentColumn++;
            if (this.currentColumn == this.columns)
            {
                this.currentColumn = 0;
                this.currentRow++;
            }
            if (this.currentRow == this.rows)
            {
                this.currentRow = 0;
            }
            this.latestChange = DateTime.Now;
        }
    }

    public virtual void UpdateSprites(){}
}

public class PurplePlayerSprite : AnimatedSprite
{
    public PurplePlayerSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/purplePlayer.png"), 320, 641);
        this.columns = 3;
        this.rows = 5;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.up = 3;
        this.down = 0;
        this.left = 1;
        this.right = 2;
        this.idle = 4;
        this.animationLenght = 300;
        this.currentRow = this.idle;
    }

    public override void UpdateSprites()
    {
        var keyMap = Game.Current.keymap.keyMapping;
        if (
            !(keyMap[Keys.Down] || 
            keyMap[Keys.Up] ||
            keyMap[Keys.Left] ||
            keyMap[Keys.Right])
        )
            this.currentRow = this.idle;
        else if (keyMap[Keys.Right])
            this.currentRow = this.right;
        else if (keyMap[Keys.Left])
            this.currentRow = this.left;
        else if (keyMap[Keys.Down])
            this.currentRow = this.down;
        else if (keyMap[Keys.Up])
            this.currentRow = this.up;
    }

    public override void UpdateSprites(DateTime now)
    {
        if ((now - this.latestChange).TotalMilliseconds >= this.animationLenght/this.columns)
        {
            this.currentColumn++;
            this.latestChange = DateTime.Now;
        }

        if (this.currentColumn >= this.columns)
            this.currentColumn = 0;
    }
}

public class MapSprite : Sprite
{
    public MapSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/map.png"), 5760, 11520);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class ButtonXSprite : Sprite
{
    public ButtonXSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/button.png"), 25, 25);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class MonitorSprite : Sprite
{
    public MonitorSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/monitor.png"), 1920, 1080);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class GlitchSprite : AnimatedSprite
{
    public GlitchSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/glitch.png"), 50, 50);
        this.columns = 3;
        this.rows = 3;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 100;
    }
}

public class BugSprite : AnimatedSprite
{
    public BugSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/bug.png"), 100, 100);
        this.columns = 1;
        this.rows = 1;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 3000;
    }

    public override void UpdateSprites(DateTime now){}
}
public class MenuSprite : AnimatedSprite
{
    public MenuSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/menu.png"), 3840, 2160);
        this.columns = 2;
        this.rows = 2;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 500;
    }
}
public class BrainSprite : AnimatedSprite
{
    public BrainSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/brain.png"), 728, 220);
        this.columns = 4;
        this.rows = 3;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 500;
    }

    public override void UpdateSprites(DateTime now)
    {
        if ((now - this.latestChange).TotalMilliseconds > this.animationLenght/this.columns)
        {
            this.currentColumn++;
            
            if (this.currentColumn == this.columns)
            {
                this.currentColumn = 0;
            }

            var stress = Game.Current.player.stress;

            switch (stress)
            {
                case < 50:
                    this.currentRow = 0;
                    break;

                case < 75:
                    this.currentRow = 1;
                    break;

                case < 100:
                    this.currentRow = 2;
                    break;
            };

            this.latestChange = DateTime.Now;
        }
    }
}

public class ReactionStructureSprite : Sprite
{
    public ReactionStructureSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/minigameBar.png"), 500, 150);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class ReactionGuideSprite : Sprite
{
    public ReactionGuideSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/guide.png"), 55, 130);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class ReactionObjectiveSprite : Sprite
{
    public ReactionObjectiveSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/objective.png"), 55, 130);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class PlayBtnSprite : Sprite
{
    public PlayBtnSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/playBtn.png"), 300, 150);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class ShopSprite : Sprite
{
    public ShopSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/shop.png"), 1788, 928);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class HuntBtnSprite : Sprite
{
    public HuntBtnSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/huntBtn.png"), 252, 141);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class BuyBtnSprite : Sprite
{
    public BuyBtnSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/buyBtn.png"), 114, 44);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class AdSprite : Sprite
{
    public AdSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/ad.png"), 976, 664);
        this.spriteW = this.image.Width;
        this.spriteH = this.image.Height;
    }
}

public class LoadingSprite : AnimatedSprite
{
    public LoadingSprite()
    {
        this.image = new Bitmap("./sprites/loading.png");
        this.columns = 5;
        this.rows = 5;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 800;
    }
}