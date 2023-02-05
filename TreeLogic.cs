using Godot;
using System;

public class TreeLogic : Node
{
    public TreeLogicState State { get; set; }

    public LeTree LeTree { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LeTree = new LeTree(Vector2.Zero);
        LeTree.RootBranch.PopulateTestTree(3);
        GD.Print(LeTree.RootBranch.ToString());
    }

    public void Reset()
    {

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
