using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace PermaProg.PermaProgCode.Model;

public partial class TopBarCurrencyModel(MegaLabel goldLabelCopy) : MegaLabel
{

    public override void _Ready()
    {
        GD.Print("in own readyyyyyyyyyyyyyyyyyyyyyyyyy");
        MinFontSize = 32;
        SetTextAutoSize("SDADSA");
    }
}