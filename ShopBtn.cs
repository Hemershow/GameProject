using System.Media;
using System.Windows.Media;

public abstract class ShopBtn 
{
    public Sprite btnSprite { get; set; }
    public int cost { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public virtual void Buy()
    {
        if (Game.Current.player.money >= this.cost)
        {
            Game.Current.player.money -= this.cost;
            GetBtnUpgrade();
        }
    } 
    public abstract void GetBtnUpgrade();
}

public class UpgradePcBtn : ShopBtn
{
    public UpgradePcBtn(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 516;
        this.y = 400;
    }

    public override void GetBtnUpgrade() => Game.Current.loading.animationLenght /= 2;
}
public class UpgradeMonitorBtn : ShopBtn
{
    public UpgradeMonitorBtn(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 1082;
        this.y = 400;
    }

    public override void GetBtnUpgrade() 
    {
        Game.Current.pcHealth.healthLimit++;
        Game.Current.pcHealth.health++;
    }
}
public class BuyCourseBtn : ShopBtn
{
    public int[] values { get; set; } = new int[]{500, 750, 1000, 2000};
    public BuyCourseBtn(int cost)
    {
        this.cost = values[0];
        this.btnSprite = new BuyBtnSprite();
        this.x = 1649;
        this.y = 400;
    }

    public override void Buy()
    {
        if (Game.Current.player.money >= this.cost && Game.Current.player.lv < 3)
        {
            Game.Current.player.money -= this.cost;
            GetBtnUpgrade();
        }
    } 

    public override void GetBtnUpgrade()
    {
        Game.Current.player.lv++;
        this.cost = values[(Game.Current.player.lv)];

        switch (Game.Current.player.lv)
        {
            case 1:
                Game.Current.player.canUseStackOverflow = true;
                break;
            
            case 2:
                Game.Current.player.hasHackerVision = true;
                break;

            case 3:
                Game.Current.player.baseSpeed *= 2;
                break;

            default:
                break;
        }

        Game.Current.player.mentalResilience *= 2;
    }
}
public class RepairPcBtn : ShopBtn
{
    public RepairPcBtn(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 516;
        this.y = 728;
    }
    public override void Buy()
    {
        if (Game.Current.player.money >= this.cost && Game.Current.pcHealth.healthLimit > Game.Current.pcHealth.health)
        {
            Game.Current.player.money -= this.cost;
            GetBtnUpgrade();
        }
    } 

    public override void GetBtnUpgrade() => Game.Current.pcHealth.health++;
}
public class BuyCoffeBtn : ShopBtn
{
    public BuyCoffeBtn(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 1082;
        this.y = 728;
    }

    public override void Buy()
    {
        if (Game.Current.player.money >= this.cost && !Game.Current.player.canRun)
        {
            Game.Current.player.money -= this.cost;
            GetBtnUpgrade();
        }
    } 
    public override void GetBtnUpgrade() => Game.Current.player.canRun = true;
}
public class BuyCinemaTicket : ShopBtn
{
    public BuyCinemaTicket(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 1649;
        this.y = 728;
    }
    public override void Buy()
    {
        if (Game.Current.player.money >= this.cost && !Game.Current.player.relaxed)
        {
            Game.Current.player.money -= this.cost;
            GetBtnUpgrade();
        }
    } 
    public override void GetBtnUpgrade() => Game.Current.player.relaxed = true;
}

public class HuntBtn : ShopBtn
{
    public HuntBtn()
    {
        this.btnSprite = new HuntBtnSprite();
        this.x = 1498;
        this.y = 820;
    }

    public override void GetBtnUpgrade()
    {
    }
}