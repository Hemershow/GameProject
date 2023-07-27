using System;
using System.Collections.Generic;
using System.Windows.Forms;

public abstract class MiniGame
{
    public Random rnd { get; set; } = new Random();
    public bool finished { get; set; } = false;
    public bool success { get; set; } = false;
    public Sprite structure { get; set; }
    public Sprite[] components { get; set; }
    public int x { get; set; }
    public int y { get; set; } 
    public abstract void CheckInteraction();
    public abstract void Run(DateTime now);
}

public class Reaction : MiniGame
{
    public DateTime latestChange { get; set; } = DateTime.Now;
    public Sprite guide { get; set; }
    public Sprite objective { get; set; }
    public int guideDirection { get; set; }
    public int border { get; set; } = 10;
    public bool lockGuide { get; set; } = false;
    public int waitDelay { get; set; } = 500;
    public Reaction(int GLitchlv)
    {
        var dificulty = (GLitchlv - Game.Current.player.lv);
        this.guideDirection = 30 + 30 * (dificulty > 0 ? dificulty : 0);
        this.structure = new ReactionStructureSprite();
        this.components = new Sprite[] {new ReactionGuideSprite(), new ReactionObjectiveSprite()};
        this.guide = this.components[0];
        this.objective = this.components[1];
        this.x = 960 - this.structure.spriteW/2;
        this.y = 700;
        this.structure.x = this.x;
        this.structure.y = this.y;
        this.guide.x = this.structure.x + this.border;
        this.guide.y = this.structure.y + this.border;
        this.objective.y = this.structure.y + this.border;
        this.objective.x = this.rnd.Next(this.structure.x + this.border, this.structure.x + this.structure.spriteW - this.border - this.objective.spriteW);
    }
    public override void CheckInteraction()
    {
        var keyMap = Game.Current.keymap.keyMapping;

        if (!lockGuide)
        {
            if (keyMap[Keys.Space])
            {
                if (
                    this.guide.x >= this.objective.x && 
                    this.guide.x <= this.objective.x + this.objective.spriteW ||
                    this.guide.x + this.guide.spriteW >= this.objective.x && 
                    this.guide.x + this.guide.spriteW <= this.objective.x + this.objective.spriteW
                )
                    this.success = true;
                this.lockGuide = true;
            }
        }
    }

    public void WaitAndFinish()
    {
        if ((DateTime.Now - this.latestChange).TotalMilliseconds > this.waitDelay)
        {
            this.finished = true;
        }
    }

    public override void Run(DateTime now)
    {
        if (!this.lockGuide)
        {
            if((now - this.latestChange).TotalMilliseconds >= 10)
            {
                if (this.guide.x < this.x + this.border - guideDirection)
                    this.guideDirection *= -1;
                else if (this.guide.x > this.x + this.structure.spriteW - this.border - this.guide.spriteW - guideDirection)
                    this.guideDirection *= -1;
                
                this.guide.x += this.guideDirection;

                this.latestChange = DateTime.Now;
            }
        }
        else
            this.WaitAndFinish();
    }
}