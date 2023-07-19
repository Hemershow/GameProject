using System.Collections.Generic;
using System.Windows.Forms;

public abstract class Player
{
    public int x { get; set;}
    public int y { get; set;}
    private int xMovement { get; set; } = 0;
    private int yMovement { get; set; } = 0;
    private int baseSpeed { get; set; } = 50;
    public void Move(int xLimit, int yLimit, int charW, int charH)
    {
        var keyMap = Game.Current.keymap.keyMapping;

        if (keyMap[Keys.Left] && keyMap[Keys.Right])
            this.xMovement = 0;
        else if (keyMap[Keys.Right]) 
            this.xMovement = this.baseSpeed + (keyMap[Keys.ShiftKey] ? this.baseSpeed : 0);
        else if (keyMap[Keys.Left])
            this.xMovement = -(this.baseSpeed + (keyMap[Keys.ShiftKey] ? this.baseSpeed : 0));

        if (keyMap[Keys.Down] && keyMap[Keys.Up])
            this.yMovement = 0;
        else if (keyMap[Keys.Down])
            this.yMovement = this.baseSpeed + (keyMap[Keys.ShiftKey] ? this.baseSpeed : 0);
        else if (keyMap[Keys.Up])
            this.yMovement = -(this.baseSpeed + (keyMap[Keys.ShiftKey] ? this.baseSpeed : 0));

        if (!(this.x + xMovement < charW/2))
            if (!(this.x + xMovement > xLimit - charW))
                this.x += xMovement;
    
        if (!(this.y + yMovement < charH/2))
            if (!(this.y + yMovement > yLimit - charH))
                this.y += yMovement;

        this.xMovement = 0;
        this.yMovement = 0;
    }
}

public class DefaultPlayer : Player
{
    public DefaultPlayer(PlayerArgs args)
    {
        this.x = args.x;
        this.y = args.y;
    }
}
