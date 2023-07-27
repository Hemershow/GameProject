using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

public class Game
{  
    private Game() { }
    private static Game crr = null;
    public static Game Current => crr;
    public Player player = null; 
    public Random rnd { get; set; } = new Random();
    public List<Glitch> glitches = new List<Glitch>();
    public List<Point> glitchPoints { get; set; } = new List<Point>();
    public Glitch nearestGlitch { get; set; } 
    public Screen screen = null; 
    public KeyMap keymap = null;
    public ComputerHealth pcHealth { get; set; } = new ComputerHealth();
    public PlayerArgs playerArgs = null;
    public ScreenArgs screenArgs = null;
    public ScreenSession gameScreen { get; set; } = new GameScreen();
    public ScreenSession menu { get; set; } = new Menu();
    public ScreenSession gameIntro { get; set; } = new Intro();
    public ScreenSession shop { get; set; } = new Shop();
    public ScreenSession loading { get; set; } = new Loading();
    public int pcLoading { get; set; } = 1600;
    public Sprite map { get; set; } = new MapSprite();
    public Rent rent { get; set; } = new NormalRent(5, 350);
    public Playlist playlist { get; set; } = new Playlist();
    public int startingGlitchs = 3;
    public void Run()
    {   
        this.screen.currentScreen = this.gameIntro;
        this.gameIntro.nextScreen = this.menu;
        this.menu.nextScreen = this.shop;
        this.screen.Start();
    }
    public void SpawnGlitchs(int amount, int xLimit, int yLimit, int charW, int charH, bool random)
    {
        if (!random)
            glitchPoints.Clear();

        for (int i = 0; i < amount; i++)
        {   
            var glitch = new Glitch(
                rnd.Next(0 + charW, xLimit - charW), 
                rnd.Next(0 + charH, yLimit - charH),
                random ? rnd.Next(0, 4) : 0
            );

            this.glitches.Add(glitch);
            this.glitchPoints.Add(new Point(glitch.x, glitch.y));
        }
    }

    public Point NearestGlitch()
    {
        int shortestDistance = int.MaxValue;
        int index = -1;

        for (int i = 0; i < glitchPoints.Count; i++)
        {
            var distanceFromPlayer = Math.Abs(glitchPoints[i].X - player.x) + Math.Abs(glitchPoints[i].Y - player.y); 
            
            if (distanceFromPlayer < shortestDistance)
            {
                shortestDistance = distanceFromPlayer;
                index = i;
            }
        }

        return glitchPoints[index];
        
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

