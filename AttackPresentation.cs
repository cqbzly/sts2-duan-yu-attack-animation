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
    private const int FrameWidth = 240;
    private const int FrameHeight = 320;
    private const int Columns = 4;
    private const int Rows = 3;
    private const int ReleaseFrame = 4;
    private const double WindupFrameTime = 0.024;
    private const double MinimumWindupTimeout = 0.40;
    private static readonly ConditionalWeakTable<Sprite2D, PoseState> BodyStates = new();
    private static AttackClip _palm = null!;
    private static AttackClip _finger = null!;

    internal static void LoadPoses()
    {
        using var idleImage = LoadPng("idle", FrameWidth, FrameHeight);
        using var restImage = LoadPng("rest", FrameWidth, FrameHeight);
        if (idleImage is not null && Engine.GetMainLoop() is SceneTree tree)
        {
            var idle = ImageTexture.CreateFromImage(idleImage);
            var rest = restImage is null ? null : ImageTexture.CreateFromImage(restImage);
            tree.NodeAdded += node => ReplaceCharacterTexture(node, idle, rest);
        }

        var palm = LoadClip("palm", new Vector2(211, 98));
        var finger = LoadClip("finger", new Vector2(226, 93));
        if (idleImage is not null && palm is not null && finger is not null)
        {
            _palm = palm;
            _finger = finger;
            return;
        }

        GD.PushWarning("Duan Yu combat sprites are incomplete. Using the original attack poses.");
        var poses = new Texture2D[5];
        for (int frame = 0; frame < poses.Length; frame++)
        {
            using var image = new Image();
            Error result = image.LoadSvgFromString(AttackPoseArt.Render(frame));
            if (result != Error.Ok) throw new InvalidOperationException($"Attack pose {frame}: {result}");
            poses[frame] = ImageTexture.CreateFromImage(image);
        }
        _palm = new AttackClip([poses[0], poses[1], poses[4]], 1, new Vector2(228, 147), 0.10, 0.10);
        _finger = new AttackClip([poses[2], poses[3], poses[4]], 1, new Vector2(232, 145), 0.10, 0.10);
    }

    private static AttackClip? LoadClip(string name, Vector2 launchPoint)
    {
        using var atlas = LoadPng(name, FrameWidth * Columns, FrameHeight * Rows);
        if (atlas is null) return null;

        var frames = new Texture2D[Columns * Rows];
        for (int frame = 0; frame < frames.Length; frame++)
        {
            var region = new Rect2I(frame % Columns * FrameWidth, frame / Columns * FrameHeight, FrameWidth, FrameHeight);
            using var image = atlas.GetRegion(region);
            frames[frame] = ImageTexture.CreateFromImage(image);
        }
        return new AttackClip(frames, ReleaseFrame, launchPoint, 0.070, 0.032);
    }

    private static Image? LoadPng(string name, int width, int height)
    {
        using var stream = typeof(AttackPresentation).Assembly
            .GetManifestResourceStream($"DuanYuAttackAnimation.combat-{name}.png");
        if (stream is null) return null;

        var bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);
        var image = new Image();
        if (image.LoadPngFromBuffer(bytes) == Error.Ok && image.GetWidth() == width && image.GetHeight() == height)
            return image;

        image.Dispose();
        return null;
    }

    private static void ReplaceCharacterTexture(Node node, Texture2D idle, Texture2D? rest)
    {
        // BaseLib can replace the scene root. The sprite keeps the main mod's original texture path.
        if (node is Sprite2D { Texture: { } texture } sprite)
        {
            if (sprite.Name == "Visuals" && IsDuanYuTexture(texture, "character-"))
                sprite.Texture = idle;
            else if (rest is not null && IsDuanYuTexture(texture, "rest-"))
                sprite.Texture = rest;
        }
        else if (node is TextureRect { Texture: { } portrait } rect &&
            rect.Name == "Portrait" &&
            IsDuanYuTexture(portrait, "character-"))
        {
            rect.Texture = idle;
        }
    }

    private static bool IsDuanYuTexture(Texture2D texture, string prefix) =>
        texture.ResourcePath.StartsWith("user://cqbzly.duanyu/assets-", StringComparison.Ordinal) &&
        Path.GetFileName(texture.ResourcePath).StartsWith(prefix, StringComparison.Ordinal);

    internal static void WindUp(Creature attacker, AttackStyle style)
    {
        if (NCombatRoom.Instance?.GetCreatureNode(attacker)?.Body is not Sprite2D body) return;
        PoseState state = BodyStates.GetValue(body, sprite => new PoseState(sprite.Texture));
        state.ResetTimer?.Kill();
        if (state.Current is null || state.Current != body.Texture) state.Original = body.Texture;
        state.AwaitingRelease = true;

        AttackClip clip = style.Finger ? _finger : _palm;
        body.Texture = state.Current = clip.Frames[0];
        Tween timer = state.ResetTimer = body.CreateTween();
        for (int frame = 1; frame < clip.ReleaseIndex; frame++)
        {
            Texture2D texture = clip.Frames[frame];
            timer.TweenInterval(WindupFrameTime);
            timer.TweenCallback(Callable.From(() => Present(body, state, texture)));
        }

        // The game controls when damage happens. Hold the wind-up pose if its animation takes longer.
        double nativeDelay = attacker.Player?.Character.AttackAnimDelay ?? 0;
        double timeout = Math.Max(MinimumWindupTimeout, nativeDelay + 0.10);
        timer.TweenInterval(timeout - (clip.ReleaseIndex - 1) * WindupFrameTime);
        timer.TweenCallback(Callable.From(() => Restore(body, state)));
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

        AttackClip clip = style.Finger ? _finger : _palm;
        if (state.AwaitingRelease)
        {
            state.AwaitingRelease = false;
            state.ResetTimer?.Kill();
            Present(body, state, clip.Frames[clip.ReleaseIndex]);
            Tween timer = state.ResetTimer = body.CreateTween();
            timer.TweenInterval(clip.ReleaseHold);
            for (int frame = clip.ReleaseIndex + 1; frame < clip.Frames.Length; frame++)
            {
                Texture2D texture = clip.Frames[frame];
                timer.TweenCallback(Callable.From(() => Present(body, state, texture)));
                timer.TweenInterval(clip.RecoveryFrameTime);
            }
            timer.TweenCallback(Callable.From(() => Restore(body, state)));
        }

        Vector2 hand = clip.LaunchPoint;
        if (body.FlipH) hand.X = FrameWidth - hand.X;
        if (body.FlipV) hand.Y = FrameHeight - hand.Y;
        if (body.Centered) hand -= new Vector2(FrameWidth, FrameHeight) * 0.5f;
        return AttackTrail.Create(body.ToGlobal(body.Offset + hand), targetNode.VfxSpawnPosition, style);
    }

    private static void Present(Sprite2D body, PoseState state, Texture2D frame)
    {
        if (body.Texture == state.Current) body.Texture = state.Current = frame;
    }

    private static void Restore(Sprite2D body, PoseState state)
    {
        if (body.Texture == state.Current) body.Texture = state.Original;
        state.Current = null;
        state.AwaitingRelease = false;
        state.ResetTimer = null;
    }

    private sealed record AttackClip(Texture2D[] Frames, int ReleaseIndex, Vector2 LaunchPoint,
        double ReleaseHold, double RecoveryFrameTime);

    private sealed class PoseState(Texture2D? original)
    {
        public Texture2D? Original = original;
        public Texture2D? Current;
        public Tween? ResetTimer;
        public bool AwaitingRelease;
    }
}
