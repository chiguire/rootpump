using Godot;
using System;

public class BeatsDraw : Node2D
{

    public AudioStats AudioStats { get; set; }

    private float actionDuration = 0.3f;
    private float currentAction = 0.0f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Draw()
    {
        DrawAudioBeats();
    }

    public void Action()
    {
        currentAction = actionDuration;
    }
    
    private void DrawAudioBeats()
    {
        var beatRadius = 10.0f;
        var xOffset = 100.0f - AudioStats.StreamTime * AudioStats.PixelsBetweenSeconds;
        var currentBeat = AudioStats.CurrentBeat;

        if (currentAction > 0.0f)
        {
            DrawCircle(new Vector2(100.0f, 100), 20.0f, new Color(0x00000088));
        }

        DrawCircle(new Vector2(100.0f, 100), 15.0f, new Color(0x380F1C88));
        
        for (int i = 0; i != (int)AudioStats.NumBeats + 1; i++)
        {
            var p = (float)AudioStats.PixelsBetweenBeats * i;
            var pos = new Vector2(xOffset + p, 100);
            var color = Colors.Bisque;
            if (currentBeat >= (i-0.1f) && currentBeat <= (i+0.1f))
            {
                color = Colors.Red;
            }
            DrawCircle(pos, beatRadius, color);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (currentAction > 0.0f)
        {
            currentAction = Mathf.Max(0, currentAction - delta);
        }
        Update();
    }
}
