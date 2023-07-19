using System;
using System.Collections.Generic;
using System.Drawing;

public abstract class Sprite 
{
    public Bitmap image { get; set; }
    public int spriteW { get; set; }
    public int spriteH { get; set; }
    public int columns { get; set; }
    public int rows { get; set; }
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
    }
}

public class PlayerSprite : Sprite
{
    public PlayerSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/player.png"), 320, 513);
        this.columns = 3;
        this.rows = 4;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
    }
}

public class MapSprite : Sprite
{
    public MapSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/map.png"), 5760, 11520);
        this.columns = 1;
        this.rows = 1;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
    }
}

public class MonitorSprite : Sprite
{
    public MonitorSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/monitor.png"), 1925, 1085);
        this.columns = 1;
        this.rows = 1;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
    }
}

public class GlitchSprite : AnimatedSprite
{
    public GlitchSprite()
    {
        this.image = new Bitmap(new Bitmap("./sprites/glitch.png"), 900, 900);
        this.columns = 3;
        this.rows = 3;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
        this.animationLenght = 300;
    }
}
