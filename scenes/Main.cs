using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
    private GridManager gridManager;
    private Sprite2D cursor;
    private PackedScene buildingScene;
    private Button placeBuildingButton;
    private Vector2? hoveredGridCell;

    public override void _Ready()
    {
        buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
        gridManager = GetNode<GridManager>("GridManager");
        cursor = GetNode<Sprite2D>("Cursor");
        placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

        cursor.Visible = false;

        placeBuildingButton.Pressed += OnButtonPressed;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionValid(hoveredGridCell.Value))
        {
            PlaceBuildingAtHoverecCellPosition();
            cursor.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        var gridPosition = gridManager.GetMouseGridCellPosition();
        cursor.GlobalPosition = gridPosition * 64;
        if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
        {
            hoveredGridCell = gridPosition;
            gridManager.HighLightValidTilesInRadius(hoveredGridCell.Value, 3);
        }
    }

    private void PlaceBuildingAtHoverecCellPosition()
    {
        if (!hoveredGridCell.HasValue)
        {
            return;
        }
        var building = buildingScene.Instantiate<Node2D>();
        AddChild(building);


        building.GlobalPosition = hoveredGridCell.Value * 64;
        gridManager.MarkTileAsOccupied(hoveredGridCell.Value);

        hoveredGridCell = null;
        gridManager.ClearHighlightedTiles();
    }

    private void OnButtonPressed()
    {
        cursor.Visible = true;
    }

}
