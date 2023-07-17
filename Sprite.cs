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
