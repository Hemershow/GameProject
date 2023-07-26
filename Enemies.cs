using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public abstract class Enemy
{
    public MiniGame miniGame { get; set; }
    public int value { get; set; }
    public int lv { get; set; } = 0;
    public AnimatedSprite enemySprite { get; set; }
    public Random rnd { get; set; }
    public bool atack { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int damage { get; set; }
    public int speed { get; set; }
    public int atackSpeed { get; set; }
    public int range { get; set; }
    public DateTime latestUpdate { get; set; } = DateTime.Now;
    public DateTime latestAtack { get; set; }
    public abstract void Move(DateTime now);
    public abstract void Atack(DateTime now);
    public virtual bool InRange()
    {
        var enemyX = this.x - Game.Current.screen.currentScreen.x;
        var enemyY = this.y - Game.Current.screen.currentScreen.y;
        
        if (
            enemyX + this.enemySprite.spriteW + this.range >= Game.Current.player.mapX &&
            enemyX - this.range <= Game.Current.player.mapX
        )
            if (
                enemyY + this.enemySprite.spriteH + this.range >= Game.Current.player.mapY &&
                enemyY - this.range <= Game.Current.player.mapY
            )
                return true;
        return false;
    }

    public virtual void Debug(DateTime now) {}
    public virtual void Stop() => 
        this.atack = false;
}

public class Glitch : Enemy
{
    public int distanceFromPlayer { get; set; }
    public bool inMinigame { get; set; } = false;
    public List<Enemy> bugs { get; set; } = new List<Enemy>();
    public Glitch(int x, int y)
    {
        this.miniGame = new Reaction();
        this.latestAtack = DateTime.Now;
        this.enemySprite = new GlitchSprite();
        this.damage = 0;
        this.atackSpeed = 5000;
        this.range = -1;
        this.atack = true;
        this.x = x;
        this.y = y;
        this.value = Game.Current.rnd.Next(15, 30);
    }

    public override void Move(DateTime now) {}
    public override void Atack(DateTime now) 
    {
        if ((now - this.latestAtack).TotalMilliseconds > this.atackSpeed)
        {
            this.bugs.Add(new Bug(this.x, this.y));
            this.latestAtack = DateTime.Now;
        }
    }

    public override void Debug(DateTime now)
    {
        this.miniGame.CheckInteraction();
        this.miniGame.Run(now);

        if (this.miniGame.finished)
        {
            this.atack = true;
        
            inMinigame = false;
        
            Game.Current.player.canMove = true;
            Game.Current.SpawnGlitchs(
                3, 
                Game.Current.map.spriteW, 
                Game.Current.map.spriteH, 
                Game.Current.player.playerSprite.spriteW, 
                Game.Current.player.playerSprite.spriteH
            );
        
            foreach (var glitch in Game.Current.glitches)
            {
                foreach (var bug in glitch.bugs)
                    bug.atack = !this.miniGame.success;
            }

            if (this.miniGame.success)
            {
                Game.Current.glitches.RemoveAll(x => (x.x == this.x && x.y == this.y));
                Game.Current.glitchPoints.RemoveAll(x => (x.X == this.x && x.Y == this.y));
                Game.Current.player.money += this.value;
            }
            
            this.miniGame = new Reaction();
        }
        else 
            inMinigame = true;
    }
}

public class Bug : Enemy
{
    public int xObjective { get; set; } = -1;
    public int yObjective { get; set; } = -1;
    public Bug(int x, int y)
    {
        this.enemySprite = new BugSprite();
        this.rnd = new Random();
        this.damage = 10;
        this.speed = 10;
        this.atackSpeed = 1000;
        this.range = 15;
        this.x = x;
        this.y = y;
        this.atack = false;
        this.xObjective = this.rnd.Next(0, Game.Current.map.spriteW);
        this.yObjective = this.yObjective = this.rnd.Next(0, Game.Current.map.spriteH);
    }

    public override void Move(DateTime now) 
    {
        if (this.atack)
        {
            if (Math.Abs(Game.Current.player.x - this.x) > this.range)
                this.x += Game.Current.player.x > this.x ? this.speed : -this.speed;
            if (Math.Abs(Game.Current.player.y - this.y) > this.range)
                this.y += Game.Current.player.y > this.y ? this.speed : -this.speed;
        }
        else
        {
            if (
                this.xObjective - this.speed >= this.x && 
                this.xObjective + this.speed <= this.x
            )
                this.xObjective = this.rnd.Next(this.x - 540, this.x + 540);
            else 
                this.x += this.xObjective > this.x ? this.speed : -this.speed;

            if (
                this.yObjective - this.speed >= this.y && 
                this.yObjective + this.speed <= this.y
            )
                this.yObjective = this.rnd.Next(this.y - 960, this.y + 960);
            else
                this.y += this.yObjective > this.y ? this.speed : -this.speed;
        }
    }
    public override void Atack(DateTime now)
    {
        if (this.atack)
        {
            if (InRange())
            {
                Game.Current.player.stress += this.damage;
                if (Game.Current.player.stress >= 100)
                    Game.Current.pcHealth.health += Game.Current.pcHealth.health - 1 > 0 ? -1 : 0;
                    
                this.atack = false;
            }       
        }
    }
}
