using Godot;
using System;

public class TreeLogic : Node
{
    public TreeLogicState State { get; set; } = TreeLogicState.TITLE;

    public LeTree LeTree { get; set; }

    public AudioStats AudioStats { get; set; }

    public Vector2 WorldCameraPos {
        get {
            if (LeTree.CurrentBranch == null)
            {
                return Vector2.Zero;
            }
            else
            {
                return LeTree.CurrentBranch.WorldCameraPos;
            }
        }
    }

    [Signal]
    public delegate void StateChange(TreeLogicState OldState, TreeLogicState NewState);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Reset();
        
        //LeTree.RootBranch.PopulateTestTree(3);
        //GD.Print(LeTree.RootBranch.ToString());
    }

    public void Reset()
    {
        LeTree = new LeTree(Vector2.Zero);
        LeTree.RootBranch.PopulateTestTree(3);
        State = TreeLogicState.TITLE;
    }

    public void Action()
    {
        if (LeTree.CurrentBranch == null)
        {
            // do nothing
        }
        LeTree.CurrentBranch.Action();
        if (!(LeTree.CurrentBranch.ChangeToThisBranch is null))
        {
            var cb = LeTree.CurrentBranch;
            LeTree.CurrentBranch = LeTree.CurrentBranch.ChangeToThisBranch;
            LeTree.CurrentBranch.Pulse = 0;
            cb.ChangeToThisBranch = null;
        }
        else
        {
            // Wait for a new pulse on next update
            LeTree.CurrentBranch = null;
        }
    }

    public void SwitchState(TreeLogicState NewState)
    {
        var oldState = State;
        State = NewState;
        GD.Print($"Switch State from {oldState} to {NewState}");
        EmitSignal(nameof(StateChange), oldState, NewState);
    }
    
    public void GrowShoot(float delta)
    {
        LeTree.ShootBranch.Length += delta;
        LeTree.ShootBranch.Thickness += delta;
    }

    public void GrowRoot(float delta)
    {
        LeTree.RootBranch.GrowLengthThickness(delta);
    }

    public void UpdatePulse(float delta)
    {
        if (LeTree.CurrentBranch is null)
        {
            // Create Pulse
            LeTree.CurrentBranch = LeTree.RootBranch;
            LeTree.CurrentBranch.Pulse = 0;
        }
        else
        {
            LeTree.CurrentBranch.UpdatePulse(delta);
            if (!(LeTree.CurrentBranch.ChangeToThisBranch is null))
            {
                var cb = LeTree.CurrentBranch;
                LeTree.CurrentBranch = LeTree.CurrentBranch.ChangeToThisBranch;
                LeTree.CurrentBranch.Pulse = 0;
                cb.ChangeToThisBranch = null;
            }
            else if (LeTree.CurrentBranch.Pulse == TBranch.NO_PULSE)
            {
                LeTree.CurrentBranch = null;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        LeTree.RootBranch.Update();
        LeTree.ShootBranch.Update();
    }
}
