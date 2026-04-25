using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using PermaProg.PermaProgCode.Extensions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;
using BaseLib.Extensions;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;

namespace PermaProg.PermaProgCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class PpRelic : CustomRelicModel
{
    private bool ShouldTrigger { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(1M, ValueProp.Unpowered), new PowerVar<StrengthPower>(1M), new PowerVar<DexterityPower>(1M)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        if (PP.StrengthGainValue > 0)
        {
            Flash();
            var strengthAmount = new PowerVar<StrengthPower>((decimal)PP.StrengthGainValue);
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, strengthAmount.BaseValue, Owner.Creature, null);
        }

        if (PP.DexterityGainValue > 0)
        {
            Flash();
            var dexterityAmount = new PowerVar<DexterityPower>((decimal)PP.DexterityGainValue);
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, dexterityAmount.BaseValue, Owner.Creature, null);
        }
    }

    public override Task BeforeTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        ShouldTrigger = true;
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (!ShouldTrigger)
            return;
        ShouldTrigger = false;
        if (PP.BlockGainValue > 0)
        {
            Flash();
            var blockAmount = new BlockVar((decimal)PP.BlockGainValue, ValueProp.Unpowered);
            await CreatureCmd.GainBlock(Owner.Creature, blockAmount, null);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        CombatState combatState)
    {
        ShouldTrigger = false;
        return Task.CompletedTask;
    }

    //PermaProg/images/relics
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "pp_relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "pp_relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "pp_relic_big.png".BigRelicImagePath();
        }
    }
}