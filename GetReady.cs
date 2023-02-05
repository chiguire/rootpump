using Godot;
using System;

public class GetReady : Node2D
{
    // Display 6 beats: (_) (1) (2) (3) (4) (1)
    // Display an indicator to alert player that beat is about to begin

    public float BeatTimer { get; set; } = 0.0f; // In beat-space time
    public bool LastClickPlayerGood { get; set; }

    private Color beatColor = Colors.Black;
    private Color indicatorColor = Colors.WhiteSmoke;
    private Color pressColor = Colors.Red;
    
    private float currentAction = 0.0f;
    private float actionDuration = 0.2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Action()
    {
        currentAction = actionDuration;
    }

    public override void _Draw()
    {
        var numBeats = 18;
        var screenWidth = GetViewport().GetVisibleRect().Size.x;
        var screenHeight = GetViewport().GetVisibleRect().Size.y;
        var beatOffset = screenWidth * 0.2f - screenWidth / 2.0f;
        var beatLength = screenWidth * 0.8f /* margin */ / numBeats;
        var beatRadius = 10.0f;
        var beatCenterY = 700 - screenHeight / 2.0f;

        for (int i = 0; i != numBeats - 1; i++)
        {
            var center = new Vector2(beatOffset + beatLength * i, beatCenterY);
            if (i == 0 || i == numBeats - 1)
            {
                DrawArc(center, beatRadius, 0, Mathf.Tau, 20, beatColor, 10);
            }
            else
            {
                DrawCircle(center, beatRadius, beatColor);
            }
        }

        var beatIndicatorPos = new Vector2(beatOffset + beatLength * BeatTimer, beatCenterY);
        var finalColor = indicatorColor;
        var timer = BeatTimer + Main.AUDIO_DELAY;
        if (BeatsDraw.BeatInArea(timer, Mathf.Round(timer)))
        {
            finalColor = pressColor;
        }
        DrawCircle(beatIndicatorPos, beatRadius * 1.1f, finalColor);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (currentAction > 0.0f)
        {
            currentAction = Mathf.Max(0, currentAction - delta);
            LastClickPlayerGood = BeatsDraw.BeatInArea(BeatTimer, Mathf.Round(BeatTimer));
        }
        Update();
    }
}
