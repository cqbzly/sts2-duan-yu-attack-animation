using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace DuanYuAttackAnimation;

internal readonly record struct AttackStyle(Color Tint, float Arc, int Strands, float Width, bool Finger = true)
{
    internal static AttackStyle ForCard(string cardId) => cardId switch
    {
        "CQBZLY_DY_B05" => new(new Color("85f8dd"), -12, 1, 7),
        "CQBZLY_DY_T01" => new(new Color("e5c276"), -42, 1, 15),
        "CQBZLY_DY_T02" => new(new Color("8af4f1"), -22, 2, 6),
        "CQBZLY_DY_T03" => new(new Color("d0adff"), -62, 3, 8),
        "CQBZLY_DY_T04" => new(new Color("9affbb"), 40, 1, 12),
        "CQBZLY_DY_T05" => new(new Color("b7dfff"), 12, 2, 5),
        "CQBZLY_DY_T06" => new(new Color("ffac93"), 48, 2, 7),
        _ => new(new Color("9ee1ba"), -26, 1, 19, Finger: false)
    };
}

internal static class AttackPresentation
{
    private static readonly ConditionalWeakTable<Sprite2D, PoseState> BodyStates = new();
    private static readonly Texture2D[] Poses = new Texture2D[5];

    internal static void LoadPoses()
    {
        for (int frame = 0; frame < Poses.Length; frame++)
        {
            using var image = new Image();
            Error result = image.LoadSvgFromString(AttackPoseArt.Render(frame));
            if (result != Error.Ok) throw new InvalidOperationException($"Attack pose {frame}: {result}");
            Poses[frame] = ImageTexture.CreateFromImage(image);
        }
    }

    internal static void WindUp(Creature attacker, AttackStyle style)
    {
        if (NCombatRoom.Instance?.GetCreatureNode(attacker)?.Body is not Sprite2D body) return;
        PoseState state = BodyStates.GetValue(body, sprite => new PoseState(sprite.Texture));
        state.ResetTimer?.Kill();
        state.Original = Array.IndexOf(Poses, body.Texture) < 0 ? body.Texture : state.Original;
        state.AwaitingRelease = true;
        body.Texture = Poses[style.Finger ? 2 : 0];
        state.ResetTimer = body.CreateTween();
        state.ResetTimer.TweenInterval(0.4);
        state.ResetTimer.TweenCallback(Callable.From(() => Restore(body, state)));
    }

    internal static Node2D? Hit(Creature attacker, Creature target, AttackStyle style)
    {
        var room = NCombatRoom.Instance;
        if (room?.GetCreatureNode(attacker)?.Body is not Sprite2D body ||
            room.GetCreatureNode(target) is not { } targetNode ||
            !BodyStates.TryGetValue(body, out PoseState? state))
        {
            return null;
        }

        if (state.AwaitingRelease)
        {
            state.AwaitingRelease = false;
            state.ResetTimer?.Kill();
            Texture2D release = Poses[style.Finger ? 3 : 1];
            body.Texture = release;
            state.ResetTimer = body.CreateTween();
            state.ResetTimer.TweenInterval(0.10);
            state.ResetTimer.TweenCallback(Callable.From(() =>
            {
                if (body.Texture == release) body.Texture = Poses[4];
            }));
            state.ResetTimer.TweenInterval(0.10);
            state.ResetTimer.TweenCallback(Callable.From(() => Restore(body, state)));
        }

        Vector2 hand = style.Finger ? new(112, -15) : new(108, -13);
        if (body.FlipH) hand.X = -hand.X;
        return AttackTrail.Create(body.ToGlobal(hand), targetNode.VfxSpawnPosition, style);
    }

    private static void Restore(Sprite2D body, PoseState state)
    {
        if (Array.IndexOf(Poses, body.Texture) >= 0) body.Texture = state.Original;
        state.AwaitingRelease = false;
        state.ResetTimer = null;
    }

    private sealed class PoseState(Texture2D? original)
    {
        public Texture2D? Original = original;
        public Tween? ResetTimer;
        public bool AwaitingRelease;
    }
}
