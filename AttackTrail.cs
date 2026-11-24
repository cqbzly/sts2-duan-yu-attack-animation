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
        float head = Mathf.Clamp(age * 6f, 0, 1f);
        float tail = Mathf.Clamp((age - 0.17f) * 1.75f, 0, 1f);
        float fade = (1 - age) * (1 - age);
        Color core = style.Tint.Lerp(Colors.White, 0.74f);

        for (int strand = 0; strand < style.Strands; strand++)
        {
            float offset = strand - (style.Strands - 1) * 0.5f;
            float arc = style.Arc + offset * 74;
            var points = new Vector2[17];
            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                float fraction = Mathf.Lerp(tail, head, pointIndex / (float)(points.Length - 1));
                points[pointIndex] = end * fraction + side * (Mathf.Sin(Mathf.Pi * fraction) * arc);
            }

            float width = style.Width * (1 - Mathf.Abs(offset) * 0.32f);
            effect.DrawPolyline(points, WithAlpha(style.Tint, 0.19f * fade), width * 2.8f, true);
            effect.DrawPolyline(points, WithAlpha(style.Tint, 0.78f * fade), width, true);
            effect.DrawPolyline(points, WithAlpha(core, 0.86f * fade), Mathf.Max(1.4f, width * 0.30f), true);

            Vector2 tip = points[^1];
            Vector2 forward = (tip - points[^2]).Normalized();
            Vector2 flank = forward.Orthogonal() * width * 0.95f;
            Vector2 root = tip - forward * (18f + width);
            if (forward != Vector2.Zero)
            {
                effect.DrawColoredPolygon([tip + forward * 7, root + flank, tip - forward * 8, root - flank],
                    WithAlpha(core, 0.95f * fade));
            }
        }

        float launch = Mathf.Max(0, 1 - age * 3.8f);
        float radius = style.Width * (1.1f + age * 7f);
        effect.DrawArc(Vector2.Zero, radius, direction.Angle() - 1.2f, direction.Angle() + 1.2f, 21,
            WithAlpha(style.Tint, launch * 0.80f), style.Finger ? 2f : 5f, true);
        effect.DrawCircle(Vector2.Zero, style.Width * 0.38f, WithAlpha(core, launch), antialiased: true);

        float impactAge = Mathf.Clamp((age - 0.12f) / 0.88f, 0, 1);
        float sparkAlpha = Mathf.Min(1f, age * 9f) * (1 - impactAge) * (1 - impactAge);
        if (age < 0.12f) return;

        float spread = (style.Finger ? 28f : 43f) * (0.28f + impactAge);
        float angle = direction.Angle() - 0.6f;
        effect.DrawArc(end, spread, angle - 1.35f, angle + 1.85f, 24, WithAlpha(style.Tint, sparkAlpha * 0.65f), 2, true);
        effect.DrawCircle(end, (style.Finger ? 10f : 15f) * (1 - impactAge), WithAlpha(style.Tint, sparkAlpha * 0.38f),
            antialiased: true);
        for (int spark = 0; spark < 7; spark++)
        {
            float sparkAngle = angle + spark * (Mathf.Tau / 7) + style.Arc * 0.03f;
            Vector2 ray = Vector2.FromAngle(sparkAngle);
            float length = spread * (0.52f + (spark % 3) * 0.19f);
            Vector2 point = end + ray * length;
            effect.DrawLine(point, point + ray * (8 + spark % 3 * 5), WithAlpha(core, sparkAlpha), spark % 2 + 1, true);
        }
    }

    private static Color WithAlpha(Color color, float alpha) => new(color.R, color.G, color.B, alpha);
}
