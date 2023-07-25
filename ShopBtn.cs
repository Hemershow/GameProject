using System.Media;

public abstract class ShopBtn 
{
    public SoundPlayer tried { get; set; } = new SoundPlayer("./sounds/blocked.wav");
    public SoundPlayer bought { get; set; } = new SoundPlayer("./sounds/bought.wav");
    public Sprite btnSprite { get; set; }
    public int cost { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public abstract void Buy(); 
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
    public override void Buy()
    {
        tried.Play();
    }
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
    public override void Buy()
    {
        bought.Play();
    }
}

public class BuyCourseBtn : ShopBtn
{
    public BuyCourseBtn(int cost)
    {
        this.cost = cost;
        this.btnSprite = new BuyBtnSprite();
        this.x = 1649;
        this.y = 400;
    }
    public override void Buy()
    {
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
    }
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
    }
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
    }
}

public class HuntBtn : ShopBtn
{
    public HuntBtn()
    {
        this.btnSprite = new HuntBtnSprite();
        this.x = 1498;
        this.y = 820;
    }
    public override void Buy()
    {
    }
}