public class AudioStats
{
    public float PixelsBetweenSeconds { get; set; } = 200.0f; // Visual distance between seconds
    public float WholePixels { get; set; } // Full distance of the whole song
    public float NumBeats { get; set; } // How many beats there are in a song
    public float PixelsBetweenBeats { get; set; } // Visual distance between beats
    public float CurrentBeat { get; set; } // Beat-space position
    public float StreamTime { get; set; }
    public float StreamDuration { get; set; }

    public void Set(float streamTime, float streamDuration, float beatsPerMinute)
    {
        WholePixels = streamDuration * PixelsBetweenSeconds;
        NumBeats = streamDuration / 60.0f * beatsPerMinute;
        PixelsBetweenBeats = WholePixels / NumBeats;
        CurrentBeat = streamTime / 60.0f * beatsPerMinute;
        StreamTime = streamTime;
        StreamDuration = streamDuration;
    }
}