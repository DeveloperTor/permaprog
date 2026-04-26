using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using BaseLib.Config.UI;
using Godot;

namespace PermaProg.PermaProgCode.UI;

public partial class EnterShopButtonUi : NButton
{
    private MegaLabel? _label;
    private NMainMenu? _mainMenu;

    public override void _Ready()
    {
        var font = PreloadManager.Cache.GetAsset<Font>("res://themes/kreon_bold_shared.tres");
        _mainMenu = GetNode<NMainMenu>("/root/Game/RootSceneContainer/MainMenu");

        _label = new MegaLabel();
        _label.Name = "EnterShopLabel";
        _label.MinFontSize = 32;
        _label.MaxFontSize = 36;
        _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, new Color(0x00000066));
        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, new Color(0xEFC851FF));
        _label.AddThemeConstantOverride(ThemeConstants.Label.OutlineSize, 16);
        _label.AddThemeFontSizeOverride(ThemeConstants.Label.FontSize, 36);
        _label.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        _label.SetTextAutoSize("PermaProg Shop");

        var btn = new Button();
        btn.Size = new Vector2(240, 50);
        btn.AddChild(_label);
        (btn.GetChild(0) as MegaLabel)!.Position = new Vector2(10, 4);
        btn.Pressed += OpenModMenu;
        AddChild(btn);

        GlobalPosition = new Vector2(840, 1023);
    }

    private void OpenModMenu()
    {
        _mainMenu?.SubmenuStack.PushSubmenuType<NModConfigSubmenu>();
    }
}