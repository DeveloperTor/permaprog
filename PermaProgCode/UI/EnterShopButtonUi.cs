using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using Godot;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using PermaProg.PermaProgCode.Extensions;

namespace PermaProg.PermaProgCode.UI;


public partial class EnterShopButtonUi : NButton
{
    private MegaLabel? _label;
    private TextureRect _tex;

    public override void _Ready()
    {
        GD.Print("In readyyyyyyyyyyyyyyy");
        _label = new MegaLabel();
        _label.Name = "EnterShopLabel";
        _label.MinFontSize = 32;
        _label.MaxFontSize = 36;
        var font = PreloadManager.Cache.GetAsset<Font>("res://themes/kreon_bold_shared.tres");
        _label.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, Colors.DarkSeaGreen);
        _label.AddThemeColorOverride(ThemeConstants.Label.FontShadowColor, new Color(0.0f, 0.0f, 0.0f, 0.188235f));
        _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, Colors.Brown);
        _label.AddThemeConstantOverride(ThemeConstants.Label.OutlineSize, 16);
        _label.AddThemeFontSizeOverride(ThemeConstants.Label.FontSize, 36);
        _label.SetTextAutoSize("PermaProg Shop");
        AddChild(_label);

        _tex = new TextureRect();
        // _tex.Texture


        ConnectSignals();
    }

    protected override void OnFocus()
    {
        GD.Print("DSADSADSADSADSADSA");
        _label?.SetTextAutoSize("hej hej");
        base.OnFocus();
    }

}