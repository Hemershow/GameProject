using System;
using System.Drawing;

public class Arrow 
{
    public int angle { get; set; }
    public Point objective { get; set; }
    public int quadrant { get; set; }
    public double deltaX { get; set; }
    public double deltaY { get; set; }
    public Sprite arrowSprite { get; set; } = new ArrowSprite();
    public Bitmap SetAngle()
    {
        this.deltaX = this.objective.X - Game.Current.player.x;
        this.deltaY = this.objective.Y - Game.Current.player.y;

        var arcTg = (Math.Atan(deltaX/deltaY) * (180 / Math.PI));

        quadrant = deltaX > 0 && deltaY < 0 ? 1 : deltaX > 0 && deltaY > 0 ? 4 : deltaX < 0 && deltaY < 0 ? 2 : 3;

        var angle = quadrant % 2 == 0 ? (quadrant * -90) - 45 - (90 - arcTg) : (quadrant * -90) - 45 - arcTg;

        var g = Graphics.FromImage(arrowSprite.image);
        g.RotateTransform((float)(angle));

        return new Bitmap(arrowSprite.spriteW, arrowSprite.spriteH, g);
    }
}