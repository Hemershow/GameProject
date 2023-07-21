using System;
using System.Linq;
using System.Collections.Generic;

public class Game
{  
    private Game() { }
    private static Game crr = null;
    public static Game Current => crr;
    public Player player = null; 
    public Random rnd { get; set; } = new Random();
    public List<Glitch> glitches = new List<Glitch>();
    public Screen screen = null; 
    public KeyMap keymap = null;
    public PlayerArgs playerArgs = null;
    public ScreenArgs screenArgs = null;
    public int startingGlitchs = 3;
    public void Run()
    {   
        this.screen.Start();
    }
    public void SpawnGlitchs(int amount, int xLimit, int yLimit, int charW, int charH)
    {
        for (int i = 0; i < amount; i++)
        {   
            var glitch = new Glitch(
                rnd.Next(0 + charW, xLimit - charW), 
                rnd.Next(0 + charH, yLimit - charH)
            );

            this.glitches.Add(glitch);
        }
    }

    public class GameBuilder
    {
        private Game game = new Game();

        public Game Build()
            => this.game;

        public GameBuilder SetPlayer<T>()
            where T : Player
        {
            this.game.player = (Player)Activator.CreateInstance(typeof(T), new object[] {this.game.playerArgs});
            return this;
        }
        public GameBuilder SetScreen<T>()
            where T : Screen
        {
            this.game.screen = (Screen)Activator.CreateInstance(typeof(T), new object[] {this.game.screenArgs});
            return this;
        }

        public GameBuilder SetKeyMapping<T>()
            where T : KeyMap
        {
            this.game.keymap = (T)Activator.CreateInstance<T>();
            return this;
        }

        public GameBuilder SetArgs()
        {
            this.game.playerArgs = new PlayerArgs(980, 980);
            this.game.screenArgs = new ScreenArgs();
            return this;
        }
    }
    public static GameBuilder GetBuilder()
        => new GameBuilder();

    public static void New(GameBuilder builder)
        => crr = builder.Build();
}

