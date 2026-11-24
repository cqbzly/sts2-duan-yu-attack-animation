using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace DuanYuAttackAnimation;

[ModInitializer(nameof(Initialize))]
public static class MainFile
{
    private const string ModId = "CQBZLY_DUAN_YU_ATTACK_ANIMATION";

    public static void Initialize()
    {
        AttackPresentation.LoadPoses();
        new Harmony(ModId).CreateClassProcessor(typeof(CardAttackPatch)).Patch();
        GD.Print("Duan Yu attack animation initialized.");
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
