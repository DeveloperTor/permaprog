using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Assets;
using BaseLib.Config.UI;
using Godot;

namespace PermaProg.PermaProgCode.UI;

public partial class EnterShopButtonUi : NButton
{
    private MegaLabel? _label;
    private NMainMenu? _mainMenu;
    private Button? _button;
    private Tween? _tween;

    public override void _Ready()
    {
        var font = PreloadManager.Cache.GetAsset<Font>("res://themes/kreon_bold_shared.tres");
        _mainMenu = GetNode<NMainMenu>("/root/Game/RootSceneContainer/MainMenu");
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", Vector2.One, 0.5f); // To avoid an error print when killed without tweener

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

        _button = new Button();
        _button.AddChild(_label);
        _button.Size = new Vector2(240, 50);
        (_button.GetChild(0) as MegaLabel)!.Position = new Vector2(10, 4);
        _button.Pressed += OpenModMenu;
        _button.MouseEntered += OnHover;
        _button.MouseExited += OnUnhover;
        AddChild(_button);

        PivotOffset = new Vector2(120, 25);
        GlobalPosition = new Vector2(840, 1023);
    }

    private void OpenModMenu()
    {
        _mainMenu?.SubmenuStack.PushSubmenuType<NModConfigSubmenu>();
    }

    private void OnHover()
    {
        _tween?.Kill();
        Scale = new Vector2(1.1f, 1.1f);
        SfxCmd.Play("event:/sfx/ui/clicks/ui_hover");
    }

    private void OnUnhover()
    {
        _tween?.Kill();
        _tween = CreateTween().SetParallel();
        _tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        _tween.TweenProperty(this, "scale", Vector2.One, 0.5f);
    }
}