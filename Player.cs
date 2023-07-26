using System;
using System.Collections.Generic;
using System.Windows.Forms;

public abstract class Player
{
    public AnimatedSprite playerSprite { get; set; }
    public Arrow arrow { get; set; } = new Arrow();
    public int lv { get; set; } = 0;
    public int x { get; set;}
    public int mapX { get; set;}
    public int y { get; set;}
    public int mapY { get; set;}
    public int stress { get; set; } = 0;
    private int xMovement { get; set; } = 0;
    private int yMovement { get; set; } = 0;
    public int baseSpeed { get; set; } = 25;
    public bool canMove { get; set; } = true;
    public int reach { get; set; } = 150;
    public float money { get; set; } = 10000;
    public bool canRun { get; set; } = false;
    public bool relaxed { get; set; } = false;
    public bool canUseStackOverflow { get; set; } = false;
    public bool hasHackerVision { get; set; } = false;
    public int mentalResilience { get; set; } = 500;
    public DateTime latestUpdate { get; set; } = DateTime.Now;
    public virtual void Move(int xLimit, int yLimit, int charW, int charH)
    {
        if (this.canMove)
        {
            var keyMap = Game.Current.keymap.keyMapping;

            if (keyMap[Keys.Left] && keyMap[Keys.Right])
                this.xMovement = 0;
            else if (keyMap[Keys.Right]) 
                this.xMovement = this.baseSpeed + ((keyMap[Keys.ShiftKey] && canRun) ? this.baseSpeed : 0);
            else if (keyMap[Keys.Left])
                this.xMovement = -(this.baseSpeed + ((keyMap[Keys.ShiftKey] && canRun) ? this.baseSpeed : 0));

            if (keyMap[Keys.Down] && keyMap[Keys.Up])
                this.yMovement = 0;
            else if (keyMap[Keys.Down])
                this.yMovement = this.baseSpeed + ((keyMap[Keys.ShiftKey] && canRun) ? this.baseSpeed : 0);
            else if (keyMap[Keys.Up])
                this.yMovement = -(this.baseSpeed + ((keyMap[Keys.ShiftKey] && canRun) ? this.baseSpeed : 0));

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

    public abstract bool InReachOfEnemy(Enemy enemy);
    public virtual void Work(DateTime now)
    {
        if ((now - latestUpdate).TotalMilliseconds >= mentalResilience + (relaxed ? mentalResilience : 0))
        {
            latestUpdate = DateTime.Now;
            var luck = Game.Current.rnd.Next(1, 100);
            stress +=  luck < 70 ? 1 : luck < 90 ? 2 : 0;
        }
    }
}

public class DefaultPlayer : Player
{
    public DefaultPlayer(PlayerArgs args)
    {
        this.playerSprite = new PurplePlayerSprite();
        this.x = args.x;
        this.y = args.y;
    }

    public override bool InReachOfEnemy(Enemy enemy)
    {
        if (this.canMove)
        {
            var keyMap = Game.Current.keymap.keyMapping;

            if (
                this.x + this.playerSprite.spriteW + this.reach >= enemy.x &&
                this.x - this.reach <= enemy.x
            )
                if (
                    this.y + this.playerSprite.spriteH + this.reach >= enemy.y &&
                    this.y - this.reach <= enemy.y
                )
                {
                    if (keyMap[Keys.X])
                    {
                        this.canMove = false;
                        enemy.Debug(DateTime.Now);
                        enemy.Stop();
                    }
                    return true;
                }
            return false;
        }
        return false;
    }
}
