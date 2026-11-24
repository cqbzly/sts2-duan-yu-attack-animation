using Godot;

namespace DuanYuAttackAnimation;

internal static class AttackTrail
{
    internal static Node2D Create(Vector2 origin, Vector2 impact, AttackStyle style)
    {
        var effect = new Node2D { Name = "DuanYuAttackTrail", TopLevel = true, Position = origin, ZIndex = 10 };
        Vector2 end = impact - origin;
        float age = 0;
        effect.Draw += () => Draw(effect, end, style, age);
        effect.Ready += () =>
        {
            var tween = effect.CreateTween();
            tween.TweenMethod(Callable.From<float>(value =>
            {
                age = value;
                effect.QueueRedraw();
            }), 0f, 1f, style.Finger ? 0.30 : 0.26);
            tween.TweenCallback(Callable.From(effect.QueueFree));
        };
        return effect;
    }

    private static void Draw(Node2D effect, Vector2 end, AttackStyle style, float age)
    {
        Vector2 direction = end.LengthSquared() > 1 ? end.Normalized() : Vector2.Right;
        Vector2 side = direction.Orthogonal();
        const float arrival = 0.28f;
        float head = Mathf.Clamp(age / arrival, 0f, 1f);
        float impactAge = Mathf.Clamp((age - arrival) / (1f - arrival), 0f, 1f);
        float length = Mathf.Min(style.Finger ? 110f : 86f, end.Length() * 0.54f);
        float fraction = length / Mathf.Max(1f, end.Length());
        float tail = Mathf.Clamp(head - fraction * (1f - impactAge * 2.1f), 0f, 1f);
        float fade = 1f - impactAge;
        Color core = style.Tint.Lerp(new Color("fff3d6"), 0.62f);

        if (head - tail > 0.008f && end.LengthSquared() > 1f)
        {
            DrawCrescent(effect, end, side, style.Arc, tail, head, style.Width * 1.4f,
                WithAlpha(style.Tint, 0.30f * fade));
            DrawCrescent(effect, end, side, style.Arc, tail, head, style.Width * 0.83f,
                WithAlpha(style.Tint, 0.91f * fade));
            DrawCrescent(effect, end, side, style.Arc, Mathf.Lerp(tail, head, 0.25f), head, style.Width * 0.35f,
                WithAlpha(core, 0.91f * fade));
        }

        if (age < arrival) return;
        float spread = (style.Finger ? 25f : 39f) * (0.34f + impactAge);
        int shardCount = 4 + style.Strands;
        for (int shard = 0; shard < shardCount; shard++)
        {
            float angle = direction.Angle() - 1.48f + 2.96f * shard / (shardCount - 1);
            Vector2 ray = Vector2.FromAngle(angle);
            Vector2 point = end + ray * spread * (0.75f + 0.12f * (shard % 3));
            Vector2 flank = ray.Orthogonal() * (1.3f + shard % 2);
            effect.DrawColoredPolygon([point - flank, point + ray * (5f + 9f * fade), point + flank],
                WithAlpha(shard % 2 == 0 ? core : style.Tint, fade * fade));
        }
    }

    private static void DrawCrescent(Node2D effect, Vector2 end, Vector2 side, float arc,
        float tail, float head, float width, Color color)
    {
        const int samples = 11;
        var shape = new Vector2[samples * 2];
        for (int index = 0; index < samples; index++)
        {
            float time = index / (float)(samples - 1);
            float fraction = Mathf.Lerp(tail, head, time);
            Vector2 center = end * fraction + side * (Mathf.Sin(Mathf.Pi * fraction) * arc);
            float span = width * (0.04f + 0.46f * Mathf.Sin(Mathf.Pi * time));
            shape[index] = center + side * span;
            shape[shape.Length - 1 - index] = center - side * span * 0.42f;
        }
        effect.DrawColoredPolygon(shape, color);
    }

    private static Color WithAlpha(Color color, float alpha) => new(color.R, color.G, color.B, alpha);
}
