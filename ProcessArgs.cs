public class ProcessArgs
{
    private static ProcessArgs empty = new ProcessArgs();
    public static ProcessArgs Empty => empty;
}

public class PlayerArgs : ProcessArgs
{
    public int x { get; set;} = 0;
    public int y { get; set;} = 0;
}

public class ScreenArgs : ProcessArgs
{
    public int interval { get; set; } = 20;
}