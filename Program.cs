using System;
using System.Drawing;
using System.Windows.Forms;

var builder = Game.GetBuilder();

builder.SetArgs()
    .SetPlayer<DefaultPlayer>()
    .SetScreen<SpriteScreen>();

Game.New(builder);

Game.Current.Run();
