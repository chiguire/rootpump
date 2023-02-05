using Godot;

public class ResultLabel : Label
{
    public Timer timer;
    public string TheText { get; set; }

    public ResultLabel(string text)
    {
        TheText = text;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        timer = new Timer();
        timer.WaitTime = 0.5f;
        timer.Connect("timeout", this, "HandleTimeout");
        AddChild(timer);
    }

    public override void _Ready()
    {
        base._Ready();
        timer.Start();
        Text = TheText;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        RectPosition += new Vector2(0, -delta*30.0f);
    }

    public void HandleTimeout()
    {
        QueueFree();
    }
}