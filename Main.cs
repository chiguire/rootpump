using Godot;
using System;

public class Main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private TreeLogic treeLogic;
    private SceneDraw sceneDraw;

    private Camera2D camera;

    private Label debugLabel;

    private float desiredZoomV = SceneDraw.STARTING_ZOOM;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        treeLogic = (TreeLogic)GetNode("TreeLogic");
        sceneDraw = (SceneDraw)GetNode("SceneDraw");
        sceneDraw.TreeLogic = treeLogic;
        camera = (Camera2D)GetNode("Camera");
        sceneDraw.Camera = camera;
        camera.Zoom = new Vector2(desiredZoomV, desiredZoomV);
        

        debugLabel = (Label)GetNode("UI/DebugLabel");
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var currentZoom = camera.Zoom;
        var desiredZoom = new Vector2(desiredZoomV, desiredZoomV);
        camera.Zoom = currentZoom.MoveToward(desiredZoom, delta);
        debugLabel.Text = $"Zoom: {camera.Zoom.x} // Desired: {desiredZoomV}";

        Update();
    }

    public override void _Input(InputEvent @event)
    {
        /*
        if (Input.IsActionPressed("zoom_out"))
        {
            desiredZoomV = Math.Min(desiredZoomV + 0.2f, 10.0f);
        }
        if (Input.IsActionPressed("zoom_in"))
        {
            desiredZoomV = Math.Max(desiredZoomV - 0.2f, 1.0f);
        }
        */
    }
}
