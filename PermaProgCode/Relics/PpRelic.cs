using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using PermaProg.PermaProgCode.Extensions;

namespace PermaProg.PermaProgCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class PpRelic : CustomRelicModel {
  public override RelicRarity Rarity => RelicRarity.Starter;

  protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(20M)];

  public override async Task AfterCombatVictory(CombatRoom _) {
    var ppRelic = this;
    if (ppRelic.Owner.Creature.IsDead)
      return;
    ppRelic.Flash();
    await CreatureCmd.Heal(ppRelic.Owner.Creature, ppRelic.DynamicVars.Heal.BaseValue);
  }

  //ContentMod/images/relics
  public override string PackedIconPath {
    get {
      var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
      return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
    }
  }

  protected override string PackedIconOutlinePath {
    get {
      var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
      return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
    }
  }

  protected override string BigIconPath {
    get {
      var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
      return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
    }
  }
}