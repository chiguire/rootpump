using Godot;
using System;

public class SceneDraw : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public TreeLogic TreeLogic { get; set; }

    public Camera2D Camera {
        get { return _camera; }
        set {
            value.LimitLeft = 0;
            value.LimitRight = WORLD_WIDTH;
            value.LimitTop = 0;
            value.LimitBottom = WORLD_HEIGHT;
            value.AnchorMode = Camera2D.AnchorModeEnum.DragCenter;
            value.MakeCurrent();
            value.Position = WORLD_CENTER;
            _camera = value;
        }
    }

    private Camera2D _camera;

    public static readonly int WORLD_WIDTH = 3000;
    public static readonly int WORLD_HEIGHT = 3000;

    public static readonly int SURFACE_HEIGHT = WORLD_HEIGHT/2;
    public static readonly int SURFACE_DEPTH = WORLD_HEIGHT - SURFACE_HEIGHT;
    public static readonly int GRASS_THICKNESS = 20;

    public static readonly Color SkyColor = Colors.SkyBlue;
    public static readonly Color SoilColor = new Color(0x380F1CFF);

    public static readonly Color GrassColor = new Color(0x126201FF);
    public static readonly Color StemColor = new Color(0x592328FF);
    public static readonly Color RootColor = new Color(0x170014FF);

    public static readonly Rect2 SkyRect = new Rect2(0, 0, WORLD_WIDTH, SURFACE_HEIGHT);
    public static readonly Rect2 SoilRect = new Rect2(0, SURFACE_HEIGHT, WORLD_WIDTH, SURFACE_HEIGHT);
    public static readonly Rect2 GrassRect = new Rect2(0, SURFACE_HEIGHT - GRASS_THICKNESS/2, WORLD_WIDTH, GRASS_THICKNESS);

    public static readonly Vector2 WORLD_CENTER = new Vector2(WORLD_WIDTH/2, WORLD_HEIGHT/2);

    public static readonly float STARTING_ZOOM = 4.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Draw()
    {
        if (TreeLogic is null)
        {
            return;
        }

        DrawRect(SkyRect, SkyColor);
        DrawRect(SoilRect, SoilColor);
        DrawRect(GrassRect, GrassColor);

        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Update();
    }
}
