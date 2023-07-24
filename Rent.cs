public abstract class Rent
{
    public int cost { get; set; }
    public int currentRound { get; set; } = 0;
    public int rounds { get; set; }
    public abstract void IncreaseRent();
    public abstract void UpdateRound();
}

public class NormalRent : Rent
{
    public NormalRent(int rounds, int startingCost)
    {
        this.cost = startingCost;
        this.rounds = 5;
    }
    public override void IncreaseRent()
    {
        this.cost += Game.Current.rnd.Next(100, 500);
    }

    public override void UpdateRound()
    {
        if (this.currentRound == rounds - 1)
        {
            this.currentRound = -1;
            Game.Current.player.money -= this.cost;
            IncreaseRent();
        }

        this.currentRound++;
    }
}