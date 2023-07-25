using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public abstract class KeyMap 
{
    public bool mouseClick { get; set; } = false;
    public Point cursorLocation { get; set; }
    public Dictionary<Keys, bool> keyMapping { get; set; }
    public abstract void SetAction(Form form, PictureBox pb);
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
            {Keys.ShiftKey, false},
            {Keys.X, false},
            {Keys.Space, false},
            {Keys.K, false}
        };
    }

    public override void SetAction(Form form, PictureBox pb)
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

        pb.MouseDown += (s, e) =>
        {
            this.mouseClick = true; 
        };
        
        pb.MouseUp += (s, e) =>
        {
            this.mouseClick = false;
        };

        pb.MouseMove += (s, e) =>
        {
            this.cursorLocation = e.Location;
        };
    }
}