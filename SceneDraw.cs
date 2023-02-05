using Godot;
using System;
using System.Collections.Generic;

public class SceneDraw : Node2D
{
    public TreeLogic TreeLogic { get; set; }

    public Camera2D Camera {
        get { return _camera; }
        set {
            value.LimitLeft = 0;
            value.LimitRight = WORLD_WIDTH;
            value.LimitTop = 0;
            value.LimitBottom = WORLD_HEIGHT;
            value.MakeCurrent();
            value.Position = WORLD_CENTER;
            _camera = value;
        }
    }

    private Camera2D _camera;

    public static readonly int WORLD_WIDTH = 3000;
    public static readonly int WORLD_HEIGHT = 3000;

    public static readonly int SURFACE_HEIGHT = WORLD_HEIGHT/2;
    public static readonly int SURFACE_DEPTH = WORLD_HEIGHT - SURFACE_HEIGHT;
    public static readonly int GRASS_THICKNESS = 20;

    public static readonly Color SkyColor = Colors.SkyBlue;
    public static readonly Color SoilColor = new Color(0x380F1CFF);

    public static readonly Color GrassColor = new Color(0x126201FF);
    public static readonly Color ShootColor = new Color(0x592328FF);
    public static readonly Color RootColor = new Color(0x170014FF);

    public static readonly Rect2 SkyRect = new Rect2(0, 0, WORLD_WIDTH, SURFACE_HEIGHT);
    public static readonly Rect2 SoilRect = new Rect2(0, SURFACE_HEIGHT, WORLD_WIDTH, SURFACE_HEIGHT);
    public static readonly Rect2 GrassRect = new Rect2(0, SURFACE_HEIGHT - GRASS_THICKNESS, WORLD_WIDTH, GRASS_THICKNESS);

    public static readonly Vector2 WORLD_CENTER = new Vector2(WORLD_WIDTH/2, WORLD_HEIGHT/2);

    public static readonly float STARTING_ZOOM = 1.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Draw()
    {
        if (TreeLogic is null)
        {
            return;
        }

        DrawRect(SkyRect, SkyColor);
        DrawRect(SoilRect, SoilColor);

        DrawTree(TreeLogic.LeTree);

        DrawRect(GrassRect, GrassColor);
    }

    private void DrawTree(LeTree leTree)
    {
        DrawShoots(leTree.ShootBranch);
        DrawRoots(leTree.RootBranch);
    }

    private void DrawShoots(TBranch branch)
    {
        DrawShoot(branch);
        foreach (var c in branch.Children)
        {
            DrawShoots(c);
        }
    }

    private void DrawShoot(TBranch branch)
    {
        // SHOOT
        if (branch.BranchType == TBranchType.SHOOT)
        {
            var leftPoints = new List<Vector2>();
            var rightPoints = new List<Vector2>();

            // Generate pairs of vectors
            for (var i = 0; i != ROOT_SEGMENTS + 1; i++)
            {
                float segmentLength = (float)i / ROOT_SEGMENTS;
                var halfWidth = (branch.Thickness - (branch.Thickness * 0.5f * segmentLength));
                var centerPoint = WORLD_CENTER + branch.Position + Mathf.Polar2Cartesian(branch.Length * segmentLength, branch.Direction);
                var leftPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction - Mathf.Pi * 0.5f);
                var rightPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction + Mathf.Pi * 0.5f);
                
                leftPoints.Add(leftPoint);
                rightPoints.Insert(0, rightPoint);
            }
            leftPoints.AddRange(rightPoints);
            var points = leftPoints.ToArray();
            var colors = new List<Color>();
            for (var i = 0; i != points.Length; i++)
            {
                colors.Add(ShootColor);
            }

            DrawPolygon(points, colors.ToArray());
        }
        // LEAF
        else if (branch.BranchType == TBranchType.LEAF)
        {
            var leftPoints = new List<Vector2>();
            var rightPoints = new List<Vector2>();

            // Generate pairs of vectors
            for (var i = 0; i != ROOT_SEGMENTS + 1; i++)
            {
                float segmentLength = (float)i / ROOT_SEGMENTS;
                if (segmentLength < 0.5f)
                {
                    var halfWidth = (segmentLength / 0.5f) * branch.Thickness;
                    var centerPoint = WORLD_CENTER + branch.Position + Mathf.Polar2Cartesian(branch.Length * segmentLength, branch.Direction);
                    var leftPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction - Mathf.Pi * 0.5f);
                    var rightPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction + Mathf.Pi * 0.5f);
                    
                    leftPoints.Add(leftPoint);
                    rightPoints.Insert(0, rightPoint);
                } else {
                    var halfWidth = (1.0f - ((segmentLength - 0.5f) / 0.5f)) * branch.Thickness;
                    var centerPoint = WORLD_CENTER + branch.Position + Mathf.Polar2Cartesian(branch.Length * segmentLength, branch.Direction);
                    var leftPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction - Mathf.Pi * 0.5f);
                    var rightPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, branch.Direction + Mathf.Pi * 0.5f);
                    
                    leftPoints.Add(leftPoint);
                    rightPoints.Insert(0, rightPoint);
                }
                
            }
            leftPoints.AddRange(rightPoints);
            var points = leftPoints.ToArray();
            var colors = new List<Color>();
            for (var i = 0; i != points.Length; i++)
            {
                colors.Add(GrassColor);
            }

            DrawPolygon(points, colors.ToArray());
        }
    }

    private void DrawRoots(TBranch root)
    {
        DrawRoot(root);
        foreach (var c in root.Children)
        {
            DrawRoots(c);
        }
    }

    private static readonly int ROOT_SEGMENTS = 8;

    private void DrawRoot(TBranch root)
    {
        
        var leftPoints = new List<Vector2>();
        var rightPoints = new List<Vector2>();

        // Generate pairs of vectors
        for (var i = 0; i != ROOT_SEGMENTS + 1; i++)
        {
            float segmentLength = (float)i / ROOT_SEGMENTS;
            var halfWidth = (root.Thickness - (root.Thickness * 0.5f * segmentLength));
            var centerPoint = WORLD_CENTER + root.Position + Mathf.Polar2Cartesian(root.Length * segmentLength, root.Direction);
            var leftPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, root.Direction - Mathf.Pi * 0.5f);
            var rightPoint = centerPoint + Mathf.Polar2Cartesian(halfWidth, root.Direction + Mathf.Pi * 0.5f);
            
            leftPoints.Add(leftPoint);
            rightPoints.Insert(0, rightPoint);
        }
        leftPoints.AddRange(rightPoints);
        var points = leftPoints.ToArray();
        var colors = new List<Color>();
        for (var i = 0; i != points.Length; i++)
        {
            colors.Add(RootColor);
        }

        DrawPolygon(points, colors.ToArray());
        if (root.Pulse >= 0.0f && root.Pulse <= 1.0f)
        {
            DrawCircle(WORLD_CENTER + root.Position + Mathf.Polar2Cartesian(root.Length * root.Pulse, root.Direction), root.Thickness*2.0f, RootColor);
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Update();
    }
}
