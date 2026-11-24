using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace DuanYuAttackAnimation;

[ModInitializer(nameof(Initialize))]
public static class MainFile
{
    private const string ModId = "CQBZLY_DUAN_YU_ATTACK_ANIMATION";

    public static void Initialize()
    {
        AttackPresentation.LoadPoses();
        SelectionPresentation.Initialize();
        var harmony = new Harmony(ModId);
        harmony.CreateClassProcessor(typeof(CardAttackPatch)).Patch();
        harmony.CreateClassProcessor(typeof(MerchantAnimationPatch)).Patch();
        GD.Print("Duan Yu attack animation initialized.");
    }
}

[HarmonyPatch]
internal static class MerchantAnimationPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(NMerchantCharacter), nameof(NMerchantCharacter._Ready));
        yield return AccessTools.Method(typeof(NMerchantCharacter), nameof(NMerchantCharacter.PlayAnimation));
    }

    private static bool Prefix(NMerchantCharacter __instance)
    {
        var visual = __instance.GetNodeOrNull<Node2D>("DuanYuMerchant");
        return visual?.GetNodeOrNull<Sprite2D>("Visuals") is null;
    }
}

[HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.FromCard))]
internal static class CardAttackPatch
{
    private static void Postfix(AttackCommand __instance, CardModel card)
    {
        var attacker = __instance.Attacker;
        if (attacker?.Player?.Character.Id.Entry != "CQBZLY_DUAN_YU") return;

        AttackStyle style = AttackStyle.ForCard(card.Id.Entry);
        __instance.WithAttackerFx(() =>
        {
            AttackPresentation.WindUp(attacker, style);
            return null;
        });
        __instance.WithHitVfxNode(target => AttackPresentation.Hit(attacker, target, style));
    }
}
