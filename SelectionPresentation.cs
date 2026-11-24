using Godot;

namespace DuanYuAttackAnimation;

internal static class SelectionPresentation
{
    private static readonly Color Jade = new("83edce");
    private static readonly Color Gold = new("e6c47b");
    private static readonly Color Violet = new("cbb1ff");
    private const float SwordCycle = 6.4f;
    private const float WindupStart = 0.48f;
    private const float ReleaseStart = 0.81f;
    private const float ReleaseEnd = 1.37f;
    private const float RecoveryEnd = 1.62f;

    internal static void Initialize()
    {
        if (Engine.GetMainLoop() is SceneTree tree) tree.NodeAdded += OnNodeAdded;
    }

    private static void OnNodeAdded(Node node)
    {
        if (node is not Control scene || scene.GetParent()?.Name != "AnimatedBg") return;
        string path = scene.SceneFilePath;
        if (!path.StartsWith("user://cqbzly.duanyu/assets-", StringComparison.Ordinal) ||
            !Path.GetFileName(path).StartsWith("select-", StringComparison.Ordinal)) return;

        scene.Ready += () => Attach(scene);
    }

    private static void Attach(Control scene)
    {
        if (scene.GetNodeOrNull<TextureRect>("Landscape") is not { } landscape ||
            scene.GetNodeOrNull<TextureRect>("Portrait") is not { Texture: { } idle } portrait) return;

        var ambient = new Node2D { Name = "DuanYuSelectionAmbient" };
        var qi = new Node2D { Name = "DuanYuSelectionQi" };
        scene.AddChild(ambient);
        scene.MoveChild(ambient, portrait.GetIndex());
        scene.AddChild(qi);

        float time = 0;
        ambient.Draw += () => DrawAmbient(ambient, scene.Size, time);
        qi.Draw += () => DrawQi(qi, portrait, time);

        void Animate(float seconds)
        {
            if (!scene.IsVisibleInTree()) return;

            time = seconds;
            float phase = seconds % SwordCycle;
            float breath = Mathf.Sin(seconds * (Mathf.Tau / 4f));
            bool windup = phase is >= WindupStart and < ReleaseStart;
            bool release = phase is >= ReleaseStart and < ReleaseEnd;
            bool recovery = phase is >= ReleaseEnd and < RecoveryEnd;
            var pose = windup ? AttackPresentation.PoseTexture(2)
                : release ? AttackPresentation.PoseTexture(3)
                : recovery ? AttackPresentation.PoseTexture(4)
                : idle;

            if (portrait.Texture != pose) portrait.Texture = pose;
            portrait.PivotOffset = new Vector2(portrait.Size.X * 0.5f, portrait.Size.Y * 0.95f);
            float flourish = Mathf.Sin(Mathf.Pi * Mathf.Clamp((phase - WindupStart) / (RecoveryEnd - WindupStart), 0f, 1f));
            portrait.Scale = new Vector2(1f + breath * 0.009f, 1f + breath * 0.019f + flourish * 0.017f);
            portrait.Rotation = breath * 0.009f + flourish * 0.017f;
            landscape.SelfModulate = Colors.White.Lerp(new Color("d5e9e8"),
                0.23f + 0.16f * Mathf.Sin(seconds * (Mathf.Tau / 8f)));
            ambient.QueueRedraw();
            qi.QueueRedraw();
        }

        Animate(0);
        var tween = scene.CreateTween().SetLoops();
        tween.TweenMethod(Callable.From<float>(Animate), 0f, 32f, 32);
    }

    private static void DrawAmbient(Node2D canvas, Vector2 size, float time)
    {
        float lift = Mathf.Sin(time * (Mathf.Tau / 8f));
        for (int band = 0; band < 3; band++)
        {
            float baseline = size.Y * (0.73f + band * 0.084f);
            float thickness = size.Y * (0.014f + band * 0.006f);
            Vector2[] points = new Vector2[33];
            for (int i = 0; i < points.Length; i++)
            {
                float x = i / (float)(points.Length - 1);
                float ripple = Mathf.Sin(x * 8f + time * (Mathf.Tau / 8f) + band * 1.6f);
                points[i] = new Vector2(size.X * x, baseline + size.Y * (ripple * 0.024f + lift * 0.012f));
            }
            canvas.DrawPolyline(points, WithAlpha(Jade, 0.035f + band * 0.008f), thickness, true);
            canvas.DrawPolyline(points, WithAlpha(Jade, 0.12f + band * 0.026f), thickness * 0.12f, true);
        }

        for (int i = 0; i < 27; i++)
        {
            float phase = (time / 8f + i * 0.193f) % 1f;
            float opacity = Mathf.Sin(phase * Mathf.Pi);
            float spread = (i * 0.618f) % 1f;
            float x = 0.49f + spread * 0.48f + Mathf.Sin(time * (Mathf.Tau / 8f) + i) * 0.023f;
            float y = 0.90f - phase * (0.58f + spread * 0.23f);
            Vector2 point = size * new Vector2(x, y);
            Color color = i % 4 == 0 ? Gold : Jade;
            float radius = size.Y * (0.0012f + 0.0005f * (i % 3));
            canvas.DrawCircle(point, radius * 3.0f, WithAlpha(color, opacity * 0.09f), antialiased: true);
            canvas.DrawCircle(point, radius, WithAlpha(color, opacity * 0.66f), antialiased: true);
        }
    }

    private static void DrawQi(Node2D canvas, TextureRect portrait, float time)
    {
        if (portrait.Texture is not { } texture) return;
        Vector2 textureSize = texture.GetSize();
        float scale = Mathf.Min(portrait.Size.X / textureSize.X, portrait.Size.Y / textureSize.Y);
        if (scale <= 0) return;

        float phase = time % SwordCycle;
        bool windup = phase is >= WindupStart and < ReleaseStart;
        bool release = phase is >= ReleaseStart and < ReleaseEnd;
        bool recovery = phase is >= ReleaseEnd and < RecoveryEnd;
        Vector2 fingertip = windup ? new(183, 148) : release ? new(232, 145) : recovery ? new(228, 173) : new(221, 156);
        Vector2 centered = (portrait.Size - textureSize * scale) * 0.5f;
        Transform2D transform = canvas.GetGlobalTransform().AffineInverse() * portrait.GetGlobalTransform();
        Vector2 hand = transform * (centered + fingertip * scale);
        float pulse = 0.52f + Mathf.Sin(time * (Mathf.Tau / 3.2f)) * 0.23f;

        canvas.DrawCircle(hand, 13f * scale, WithAlpha(Jade, pulse * 0.14f), antialiased: true);
        canvas.DrawCircle(hand, 3f * scale, WithAlpha(Jade.Lerp(Colors.White, 0.5f), pulse), antialiased: true);

        for (int i = 0; i < 5; i++)
        {
            float travel = (time / 1.28f + i * 0.2f) % 1f;
            Vector2 point = hand + new Vector2((18f + i * 7f) * travel, -(18f + i * 9f) * travel) * scale;
            canvas.DrawCircle(point, scale * (2f - travel), WithAlpha(Jade, (1f - travel) * 0.65f), antialiased: true);
        }

        if (!release) return;
        float progress = (phase - ReleaseStart) / (ReleaseEnd - ReleaseStart);
        float rise = 35f + 52f * Mathf.Sin(progress * Mathf.Pi);
        float opacity = Mathf.Sin(progress * Mathf.Pi);
        Color core = Violet.Lerp(Colors.White, 0.73f);
        for (int strand = -1; strand <= 1; strand++)
        {
            Vector2[] points = new Vector2[19];
            for (int i = 0; i < points.Length; i++)
            {
                float t = i / (float)(points.Length - 1);
                // The hand sits near the right edge. Bend the trails back into the moonlit sky.
                float sweep = t * (37f + strand * 9f) - t * t * (62f - strand * 13f);
                points[i] = hand + new Vector2(sweep, -t * (rise + strand * 12f)) * scale;
            }
            canvas.DrawPolyline(points, WithAlpha(Violet, opacity * 0.25f), scale * 8f, true);
            canvas.DrawPolyline(points, WithAlpha(core, opacity * 0.91f), scale * 2f, true);
        }
    }

    private static Color WithAlpha(Color color, float alpha) => new(color.R, color.G, color.B, alpha);
}
