using System;
using System.Drawing;
using System.Windows.Forms;

var builder = Game.GetBuilder();

builder.SetArgs()
    .SetPlayer<DefaultPlayer>()
    .SetScreen<DefaultScreen>()
    .SetKeyMapping<DefaultKeyMap>();

Game.New(builder);

Game.Current.Run();
