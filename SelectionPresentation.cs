using Godot;

namespace DuanYuAttackAnimation;

internal static class SelectionPresentation
{
    private const int FrameWidth = 768;
    private const int FrameHeight = 416;
    private const int AtlasColumns = 4;
    private const int AtlasRows = 2;
    private const int FramesPerAtlas = AtlasColumns * AtlasRows;
    private const int FrameCount = 46;
    private const int FramesPerSecond = 12;
    private static readonly Lazy<Texture2D[]?> Atlases = new(LoadAtlases);

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
            scene.GetNodeOrNull<TextureRect>("Portrait") is not { } portrait ||
            Atlases.Value is not { } atlases) return;

        var frameTexture = new AtlasTexture { FilterClip = true };
        var animation = new TextureRect
        {
            Name = "DuanYuSelectionAnimation",
            Texture = frameTexture,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        scene.AddChild(animation);
        var viewport = scene.GetViewport();
        int currentFrame = -1;
        void ShowFrame(float frameIndex)
        {
            // The game moves and scales its background container during entry.
            Rect2 bounds = viewport.GetVisibleRect();
            Transform2D toLocal = scene.GetGlobalTransformWithCanvas().AffineInverse();
            animation.Position = toLocal * bounds.Position;
            animation.Size = toLocal * bounds.End - animation.Position;

            int frame = (int)frameIndex % FrameCount;
            if (currentFrame == frame) return;

            int tile = frame % FramesPerAtlas;
            frameTexture.Atlas = atlases[frame / FramesPerAtlas];
            frameTexture.Region = new Rect2((tile % AtlasColumns) * FrameWidth,
                (tile / AtlasColumns) * FrameHeight, FrameWidth, FrameHeight);
            currentFrame = frame;
        }

        ShowFrame(0);
        landscape.Hide();
        portrait.Hide();
        var tween = scene.CreateTween().SetLoops();
        tween.TweenMethod(Callable.From<float>(ShowFrame), 0f, (float)FrameCount,
            (double)FrameCount / FramesPerSecond);
    }

    private static Texture2D[]? LoadAtlases()
    {
        int count = (FrameCount + FramesPerAtlas - 1) / FramesPerAtlas;
        var textures = new Texture2D[count];
        for (int index = 0; index < count; index++)
        {
            using var stream = typeof(SelectionPresentation).Assembly
                .GetManifestResourceStream($"DuanYuAttackAnimation.selection-atlas-{index}.jpg");
            if (stream is null) return null;

            var data = new byte[stream.Length];
            stream.ReadExactly(data);
            using var image = new Image();
            if (image.LoadJpgFromBuffer(data) != Error.Ok ||
                image.GetWidth() != FrameWidth * AtlasColumns ||
                image.GetHeight() != FrameHeight * AtlasRows) return null;

            textures[index] = ImageTexture.CreateFromImage(image);
        }
        return textures;
    }
}
