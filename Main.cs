using Godot;
using System;

public class Main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public static readonly float AUDIO_DELAY = 0.18f;

    private TreeLogic treeLogic;
    private SceneDraw sceneDraw;
    private AudioStreamPlayer audioChannel1;
    private AudioStreamPlayer audioChannel2;
    private AudioStreamPlayer audioChannel3;
    private AudioStreamPlayer clickPlayer;
    private BeatsDraw beatsDraw;
    private AudioStats audioStats;
    private CanvasLayer titleLayer;
    private CanvasLayer readyLayer;
    private CanvasLayer gameLayer;
    private Node2D beatResultIndicator;
    private CanvasLayer overLayer;
    private GetReady getReadyIndicator;

    private Timer readyTimer;
    private Timer overTimer;

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

        beatsDraw = GetNode<BeatsDraw>("Game/BeatsDraw");
        beatsDraw.AudioStats = audioStats;

        titleLayer = GetNode<CanvasLayer>("Title");
        readyLayer = GetNode<CanvasLayer>("GetReady");
        readyLayer.Visible = false;
        gameLayer = GetNode<CanvasLayer>("Game");
        gameLayer.Visible = false;
        beatResultIndicator = GetNode<Node2D>("Game/BeatResultIndicator");
        getReadyIndicator = GetNode<GetReady>("GetReady/GetReady");
        overLayer = GetNode<CanvasLayer>("Over");
        overLayer.Visible = false;

        treeLogic.Connect(nameof(TreeLogic.StateChange), this, nameof(HandleSwitchState));

        readyTimer = GetNode<Timer>("ReadyTimer");
        readyTimer.OneShot = true;
        readyTimer.WaitTime = 60.0f / BeatsPerMinute  * 16.0f;
        readyTimer.Connect("timeout", this, "HandleReadyTimer");

        overTimer = GetNode<Timer>("OverTimer");
        overTimer.OneShot = true;
        overTimer.WaitTime = 5.0f;
        overTimer.Connect("timeout", this, "HandleOverTimer");

        clickPlayer = GetNode<AudioStreamPlayer>("ClickPlayer");
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var currentZoom = camera.Zoom;
        var desiredZoom = new Vector2(desiredZoomV, desiredZoomV);
        camera.Zoom = currentZoom.MoveToward(desiredZoom, delta);
        debugLabel.Text = $"Current beat: {audioStats.CurrentBeat}, t: {readyTimer.TimeLeft}, st: {treeLogic.State}";

        double playerTime = 0;
        if (treeLogic.State == TreeLogicState.READY)
        {
            playerTime = clickPlayer.GetPlaybackPosition();
        }
        else
        {
            playerTime = audioChannel1.GetPlaybackPosition();
        }
        double time = playerTime + AudioServer.GetTimeSinceLastMix();
        // Compensate for output latency.
        time -= AudioServer.GetOutputLatency();
        audioStats.Set((float)time, _streamDuration, BeatsPerMinute);

        switch (treeLogic.State)
        {
            case TreeLogicState.READY:
            {
                getReadyIndicator.BeatTimer = audioStats.CurrentBeat;
                break;
            }
            case TreeLogicState.GAME:
            {
                treeLogic.GrowShoot(delta);
                treeLogic.UpdatePulse(delta);
                break;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        switch (treeLogic.State)
        {
            case TreeLogicState.TITLE:
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    treeLogic.SwitchState(TreeLogicState.READY);
                }
                break;
            case TreeLogicState.READY:
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    getReadyIndicator.Action();
                    var clickResult = getReadyIndicator.LastClickPlayerGood;
                    GD.Print(clickResult? "Okay!": "Meh");
                }
                break;
            case TreeLogicState.GAME:
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    beatsDraw.Action();
                    var clickResult = beatsDraw.LastClickPlayerGood;
                    GD.Print(clickResult? "Okay!": "Meh");
                    var resultLabel = new ResultLabel(clickResult? "Okay!": "Meh");
                    beatResultIndicator.AddChild(resultLabel);
                    resultLabel.RectPosition =  new Vector2(90, 80);
                }
                break;
            case TreeLogicState.OVER:
                // none
                break;
        }
    }

    private void HandleSwitchState(TreeLogicState oldState, TreeLogicState newState)
    {
        switch (oldState)
        {
            case TreeLogicState.TITLE:
                titleLayer.Visible = false;
                break;
            case TreeLogicState.READY:
                readyLayer.Visible = false;
                clickPlayer.Stop();
                break;
            case TreeLogicState.GAME:
                gameLayer.Visible = false;
                audioChannel1.Stop();
                audioChannel2.Stop();
                audioChannel3.Stop();
                break;
            case TreeLogicState.OVER:
                overLayer.Visible = false;
                break;            
        }

        switch (newState)
        {
            case TreeLogicState.TITLE:
                titleLayer.Visible = true;
                break;
            case TreeLogicState.READY:
                readyLayer.Visible = true;
                _streamDuration = clickPlayer.Stream.GetLength();
                audioChannel1.Stop();
                audioChannel2.Stop();
                audioChannel3.Stop();
                clickPlayer.Play();
                readyTimer.Start();
                break;
            case TreeLogicState.GAME:
                gameLayer.Visible = true;
                _streamDuration = audioChannel1.Stream.GetLength();
                audioChannel1.Play();
                audioChannel2.Play();
                audioChannel3.Play();
                break;
            case TreeLogicState.OVER:
                overLayer.Visible = true;
                overTimer.Start();
                break;
        }
    }

    private void HandleReadyTimer()
    {
        treeLogic.SwitchState(TreeLogicState.GAME);
    }

    private void HandleOverTimer()
    {
        treeLogic.SwitchState(TreeLogicState.TITLE);
    }
}
