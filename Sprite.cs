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

public class Map : Sprite
{
    public Map()
    {
        this.image = new Bitmap(new Bitmap("./sprites/map.png"), 5760, 11520);
        this.columns = 1;
        this.rows = 1;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
    }
}

public class Monitor : Sprite
{
    public Monitor()
    {
        this.image = new Bitmap(new Bitmap("./sprites/monitor.png"), 1925, 1085);
        this.columns = 1;
        this.rows = 1;
        this.spriteW = this.image.Width/this.columns;
        this.spriteH = this.image.Height/this.rows;
    }
}
