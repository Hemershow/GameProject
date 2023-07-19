using System.Collections.Generic;
using System.Windows.Forms;

public abstract class KeyMap 
{
    public Dictionary<Keys, bool> keyMapping { get; set; }
    public Dictionary<string, bool> actions { get; set; }
    public abstract void SetAction(Form form);
}

public class DefaultKeyMap : KeyMap
{
    public DefaultKeyMap()
    {
        this.keyMapping = new Dictionary<Keys, bool>()
        {
            {Keys.W, false},
            {Keys.A, false},
            {Keys.S, false},
            {Keys.D, false},
            {Keys.Right, false},
            {Keys.Left, false},
            {Keys.Up, false},
            {Keys.Down, false},
            {Keys.ShiftKey, false}
        };
    }

    public override void SetAction(Form form)
    {
        form.KeyDown += (s, e) =>
        {
            if (keyMapping.ContainsKey(e.KeyCode))
                keyMapping[e.KeyCode] = true;
            else if(e.KeyCode == Keys.Escape)
                Application.Exit();
        };

        form.KeyUp += (s, e) =>
        {
            if (keyMapping.ContainsKey(e.KeyCode))
                keyMapping[e.KeyCode] = false;
        };
    }
}