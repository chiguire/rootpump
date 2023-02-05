using Godot;
using System;

public class BeatsDraw : Node2D
{

    public AudioStats AudioStats { get; set; }
    public bool LastClickPlayerGood { get; set; }

    private float actionDuration = 0.2f;
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
        var xMargin = 100.0f;
        var timer = AudioStats.StreamTime + Main.AUDIO_DELAY;
        var xOffset = xMargin - timer * AudioStats.PixelsBetweenSeconds;
        var yOffset = 100.0f;
        var currentBeat = AudioStats.CurrentBeat;

        if (currentAction > 0.0f)
        {
            DrawCircle(new Vector2(xMargin, yOffset), 20.0f, new Color(0x00000088));
        }

        // Good beat indicator
        DrawArc(new Vector2(xMargin, yOffset), 15.0f, 0, Mathf.Tau, 20, new Color(0x180F1C88), 5); 
        
        for (int i = 0; i != (int)AudioStats.NumBeats + 1; i++)
        {
            var p = (float)AudioStats.PixelsBetweenBeats * i;
            var pos = new Vector2(xOffset + p, yOffset);
            var color = Colors.Bisque;
            DrawCircle(pos, beatRadius, color);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (currentAction > 0.0f)
        {
            currentAction = Mathf.Max(0, currentAction - delta);
            LastClickPlayerGood = BeatsDraw.BeatInArea(AudioStats.CurrentBeat, Mathf.Round(AudioStats.CurrentBeat));
        }
        Update();
    }

    public static bool BeatInArea(float beat, float target) {
        return (beat >= (target-0.15f) && beat <= (target+0.15f));
    }
}
