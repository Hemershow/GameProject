using System;
using System.Drawing;

public class Arrow 
{
    public int angle { get; set; }
    public Point objective { get; set; }
    public int quadrant { get; set; }
    public double deltaX { get; set; }
    public double deltaY { get; set; }
    public void SetAngle()
    {
        this.deltaX = this.objective.X - Game.Current.player.x;
        this.deltaY = this.objective.Y - Game.Current.player.y;

        var arcTg = (Math.Atan(deltaX/deltaY) * (180 / Math.PI));

        
    }
}