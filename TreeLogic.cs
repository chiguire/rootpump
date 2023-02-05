using Godot;
using System;

public class TreeLogic : Node
{
    public TreeLogicState State { get; set; } = TreeLogicState.TITLE;

    public LeTree LeTree { get; set; }

    public AudioStats AudioStats { get; set; }

    [Signal]
    public delegate void StateChange(TreeLogicState OldState, TreeLogicState NewState);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LeTree = new LeTree(Vector2.Zero);
        //LeTree.RootBranch.PopulateTestTree(3);
        //GD.Print(LeTree.RootBranch.ToString());
        State = TreeLogicState.TITLE;
    }

    public void Reset()
    {
        State = TreeLogicState.TITLE;
    }

    public void Action()
    {

    }

    public void SwitchState(TreeLogicState NewState)
    {
        var oldState = State;
        State = NewState;
        EmitSignal(nameof(StateChange), oldState, NewState);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        LeTree.RootBranch.Length += delta;
        LeTree.RootBranch.Thickness += delta;

        LeTree.RootBranch.Update();
        LeTree.ShootBranch.Update();
    }
}
