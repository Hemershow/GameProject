using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Enemies
{
    public Random rnd = new Random();
    public List<Enemy> enemiesList = new List<Enemy>();
    public void SpawnGlitchs(int amount, int xLimit, int yLimit, int charW, int charH)
    {
        for (int i = 0; i < amount; i++)
        {   
            var bug = new Bug(
                rnd.Next(0 + charW, xLimit - charW), 
                rnd.Next(0 + charH, yLimit - charH)
            );

            enemiesList.Add(bug);
        }
    }
}

public abstract class Enemy
{
    public bool atack { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int damage { get; set; }
    public int speed { get; set; }
    public int atackSpeed { get; set; }
    public int range { get; set; }
    public abstract void Move();
    public abstract void Atack();
}

public class Glitch : Enemy
{
    public Glitch(int x, int y)
    {
        this.damage = 0;
        this.atackSpeed = 5000;
        this.range = -1;
        this.atack = true;
        this.x = x;
        this.y = y;
    }

    public override void Move() {}
    public override void Atack() 
    {
        Game.Current.enemies.enemiesList.Add(new Bug(this.x, this.y));
    }
    public void Stop()
    {
        this.atack = false;
    }
}

public class Bug : Enemy
{
    public Bug(int x, int y)
    {
        this.damage = 10;
        this.speed = 10;
        this.atackSpeed = 1000;
        this.range = 15;
        this.x = x;
        this.y = y;
        this.atack = false;
    }

    public override void Move() 
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
            
        }
    }
    public override void Atack()
    {
        
    }
}
