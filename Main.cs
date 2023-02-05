using Godot;
using System;

public class Main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private TreeLogic treeLogic;
    private SceneDraw sceneDraw;
    private AudioStreamPlayer audioChannel1;
    private AudioStreamPlayer audioChannel2;
    private AudioStreamPlayer audioChannel3;
    private BeatsDraw beatsDraw;
    private AudioStats audioStats;
    private CanvasLayer titleLayer;



    private Camera2D camera;

    private Label debugLabel;

    private float desiredZoomV = SceneDraw.STARTING_ZOOM;

    private double _timeBegin;
    private double _timeDelay;
    private float _streamDuration;
    
    public float BeatsPerMinute { get; set; } = 120.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        audioStats = new AudioStats();

        treeLogic = GetNode<TreeLogic>("TreeLogic");
        treeLogic.AudioStats = audioStats;

        camera = GetNode<Camera2D>("Camera");
        camera.Zoom = new Vector2(desiredZoomV, desiredZoomV);

        sceneDraw = GetNode<SceneDraw>("SceneDraw");
        sceneDraw.TreeLogic = treeLogic;
        sceneDraw.Camera = camera;
        
        audioChannel1 = GetNode<AudioStreamPlayer>("Song/Channel1");
        audioChannel1.Bus = "Channel 1";
        audioChannel2 = GetNode<AudioStreamPlayer>("Song/Channel2");
        audioChannel2.Bus = "Channel 2";
        audioChannel3 = GetNode<AudioStreamPlayer>("Song/Channel3");
        audioChannel3.Bus = "Channel 3";

        debugLabel = GetNode<Label>("UI/DebugLabel");

        beatsDraw = GetNode<BeatsDraw>("UI/BeatsDraw");
        beatsDraw.AudioStats = audioStats;

        titleLayer = GetNode<CanvasLayer>("Title");

        treeLogic.Connect(nameof(TreeLogic.StateChange), this, nameof(HandleSwitchState));
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var currentZoom = camera.Zoom;
        var desiredZoom = new Vector2(desiredZoomV, desiredZoomV);
        camera.Zoom = currentZoom.MoveToward(desiredZoom, delta);
        debugLabel.Text = $"Current beat: {audioStats.CurrentBeat}, t: {audioStats.StreamTime}, st: {treeLogic.State}";

        var time = (OS.GetTicksUsec() - _timeBegin) / 1000000.0d;
        time = Math.Max(0.0d, time - _timeDelay);
        audioStats.Set((float)time, _streamDuration, BeatsPerMinute);
    }

    public override void _Input(InputEvent @event)
    {
        switch (treeLogic.State)
        {
            case TreeLogicState.TITLE:
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    treeLogic.SwitchState(TreeLogicState.GAME);
                }
                break;
            case TreeLogicState.GAME:
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    if (!audioChannel1.Playing)
                    {
                        _timeBegin = OS.GetTicksUsec();
                        _timeDelay = AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency() + 0.15f;
                        _streamDuration = audioChannel1.Stream.GetLength();
                        audioChannel1.Play();
                        audioChannel2.Play();
                        audioChannel3.Play();
                    }
                    beatsDraw.Action();
                }
                break;
        }
        
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

    private void HandleSwitchState(TreeLogicState oldState, TreeLogicState newState)
    {
        switch (oldState)
        {
            case TreeLogicState.TITLE:
                titleLayer.Visible = false;
                break;
        }

        switch (newState)
        {
            case TreeLogicState.TITLE:
                titleLayer.Visible = true;
                break;
        }
    }
}
