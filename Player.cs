using System.Collections.Generic;

public abstract class Player
{
    public int x { get; set;}
    public int y { get; set;}
    public bool moveRight { get; set; } = false;
    public bool moveLeft {get; set; } = false; 
    public bool moveDown { get; set; } = false;
    public bool moveUp { get; set; } = false;
    public bool running {get; set; } = false;
    private int xMovement { get; set; } = 0;
    private int yMovement { get; set; } = 0;
    private int baseSpeed { get; set; } = 10;
    public void Move()
    {
        if (this.moveLeft)
            this.xMovement = -(this.baseSpeed + (this.running ? this.baseSpeed : 0));

        if (this.moveRight) 
            this.xMovement = this.baseSpeed + (this.running ? this.baseSpeed : 0);
                
        if (this.moveDown)
            this.yMovement = this.baseSpeed + (this.running ? this.baseSpeed : 0);
        
        if (this.moveUp)
            this.yMovement = -(this.baseSpeed + (this.running ? this.baseSpeed : 0));

        this.x += xMovement;
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
