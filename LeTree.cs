using System;
using System.Text;
using System.Collections.Generic;
using Godot;

public enum TBranchType
{
    SHOOT,
    ROOT,
    LEAF
}

public class TBranch  { // Can be used for shoot or root
    public TBranchType BranchType { get; set; }
    public Vector2 Position { get; set; } // Only given for "root" parent (hehe), children are calculated
    public float StartingPosition { get; set; } // w.r.t. parent
    public float Direction { get; set; } // w.r.t. parent direction
    public float ArcRangeStart { get; set; } // CCW arc range start
    public float ArcRangeEnd { get; set; } // CW arc range end
    public float Length { get; set; }
    public float Thickness { get; set; }
    public float Pulse { get; set; } // Between 0 and 1.0, where the pulse is travelling, -10 for no pulse
    public List<TBranch> Children { get; set; }

    public static readonly float NO_PULSE = -10.0f;

    public void PopulateTestTree(int level) {
        if (level == 0) {
            return;
        }

        var numChildren = GD.Randi() % 4;

        for (var i = 0; i != numChildren; i++)
        {
            var startingPosition = (float)GD.RandRange(0.25, 1.0);
            var direction = (float) GD.RandRange(Direction - ArcRangeStart, Direction + ArcRangeEnd);

            var newChild = new TBranch {
                BranchType = TBranchType.ROOT,
                Position = Position + Mathf.Polar2Cartesian(startingPosition * Length, Direction),
                StartingPosition = startingPosition,
                Direction = direction,
                ArcRangeStart = ArcRangeStart,
                ArcRangeEnd = ArcRangeEnd,
                Length = Length * 0.8f,
                Thickness = Thickness * 0.8f,
                Pulse = TBranch.NO_PULSE,
                Children = new List<TBranch>(),
            };

            Children.Add(newChild);
        }

        foreach (var c in Children) {
            c.PopulateTestTree(level - 1);
        }
    }

    public void ToString_(int indent, StringBuilder builder)
    {
        for (int i = 0; i != indent; i++) {
            builder.Append("> ");
        }
        builder.AppendLine($"Tree [ Position = {Position}, StartingPosition = {StartingPosition}, Direction = {Direction}, Length = {Length}, Thickness = {Thickness}, ChildrenNum = {Children.Count} ]");
        foreach (var c in Children) {
            c.ToString_(indent + 1, builder);
        }
    }

    public override String ToString() {
        StringBuilder builder = new StringBuilder();
        ToString_(0, builder);
        return builder.ToString();
    }
}

public class LeTree
{
    public TBranch ShootBranch { get; set; }
    public TBranch RootBranch { get; set; }
    public LeTree(Vector2 position)
    {
        ShootBranch = CreateShoot(position);
        RootBranch = new TBranch() {
            Position = position,
            StartingPosition = 0,
            Direction = Mathf.Pi * 0.5f,
            ArcRangeStart = Mathf.Pi/7.0f,
            ArcRangeEnd = Mathf.Pi/7.0f,
            Length = 100.0f,
            Thickness = 5.0f,
            Pulse = TBranch.NO_PULSE,
            Children = new List<TBranch>(),
        };
    }

    private TBranch CreateShoot(Vector2 position)
    {
        var length = 100.0f;
        var direction = -Mathf.Pi * 0.5f;
        return new TBranch(){
            BranchType = TBranchType.SHOOT,
            Position = position,
            StartingPosition = 0,
            Direction = direction,
            ArcRangeStart = 0,
            ArcRangeEnd = 0,
            Length = length,
            Thickness = 5.0f,
            Pulse = TBranch.NO_PULSE,
            Children = new List<TBranch>() {
                new TBranch() {
                    BranchType = TBranchType.LEAF,
                    Position = position + Mathf.Polar2Cartesian(length, direction),
                    StartingPosition = 1.0f,
                    Direction = -Mathf.Pi * 0.25f,
                    ArcRangeStart = 0,
                    ArcRangeEnd = 0,
                    Length = length*0.5f,
                    Thickness = 15.0f,
                    Pulse = TBranch.NO_PULSE,
                    Children = new List<TBranch>(),
                },
                new TBranch() {
                    BranchType = TBranchType.LEAF,
                    Position = position + Mathf.Polar2Cartesian(length, direction),
                    StartingPosition = 1.0f,
                    Direction = -Mathf.Pi * 0.8f,
                    ArcRangeStart = 0,
                    ArcRangeEnd = 0,
                    Length = length*0.6f,
                    Thickness = 12.0f,
                    Pulse = TBranch.NO_PULSE,
                    Children = new List<TBranch>(),
                }
            },
        };
    }
}
