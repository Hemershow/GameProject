using System;
using System.Drawing;

public class Arrow 
{
    public double angle { get; set; }
    public Point objective { get; set; }
    public int quadrant { get; set; }
    public double deltaX { get; set; }
    public double deltaY { get; set; }
    public Sprite arrowSprite { get; set; } = new ArrowSprite();
    public Bitmap SetAngle()
    {
        this.deltaX = (this.objective.X - Game.Current.player.x) - Game.Current.player.playerSprite.spriteW/2;
        this.deltaY = (this.objective.Y - Game.Current.player.y) - Game.Current.player.playerSprite.spriteH/2;

        var arcTg = (Math.Atan(deltaX/deltaY) * (180 / Math.PI));
        
        if (deltaX < 0 && deltaY < 0)
            quadrant = 2;

        if (deltaX > 0 && deltaY < 0)
            quadrant = 1;

        if (deltaX > 0 && deltaY > 0)
            quadrant = 4;

        if (deltaX < 0 && deltaY > 0)
            quadrant = 3;

        switch (quadrant)
        {
            case 1:
                angle = 270 + -(arcTg);
                break;

            case 2:
                angle = 270 - arcTg;
                break;

            case 3:
                angle = 90 + -(arcTg);
                break;

            default:
                angle = 90 - arcTg;
                break;
                
        }

        Bitmap bmp = new Bitmap(arrowSprite.spriteW * 5, arrowSprite.spriteH * 5);
        Graphics gfx = Graphics.FromImage(bmp);
        gfx.Clear(Color.Transparent);
        gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
        gfx.RotateTransform((float)(angle - 45));
        gfx.DrawImage(arrowSprite.image, new Point(0,0));

        return bmp;
    }
}