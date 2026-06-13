using System;
using UnityEngine;

public class CostGrid : ICostGrid
{
    public event Action CostGridChanged;

    private Grid<CostCell> grid;

    public CostGrid(int width, int height, float cellSize, Vector3 origin)
    {
        grid = new Grid<CostCell>(
            width,
            height,
            cellSize,
            origin,
            (g, x, y) => new CostCell(x, y)
        );

        InitializePlainCosts();
    }

    public Grid<CostCell> GetGrid() => grid;

    public int Width => grid.GetWidth();
    public int Height => grid.GetHeight();
    public float CellSize => grid.GetCellSize();
    public Vector3 Origin => grid.GetOriginPosition();

    public CostCell GetCell(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public bool TryGetCell(int x, int y, out CostCell cell)
    {
        return grid.TryGetGridObject(x, y, out cell);
    }

    ICostCell ICostGrid.GetCell(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    bool ICostGrid.TryGetCell(int x, int y, out ICostCell cell)
    {
        if (grid.TryGetGridObject(x, y, out CostCell costCell))
        {
            cell = costCell;
            return true;
        }

        cell = null;
        return false;
    }

    public bool TryGetCell(Vector3 worldPos, out CostCell cell)
    {
        return grid.TryGetGridObject(worldPos, out cell);
    }

    private void InitializePlainCosts()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                grid.GetGridObject(x, y).SetCost(1);
            }
        }
    }

    public bool SetCost(int x, int y, byte cost)
    {
        if (!grid.TryGetGridObject(x, y, out CostCell cell))
            return false;

        bool result = cell.SetCost(cost);

        grid.TriggerDebugRefresh(x, y);
        CostGridChanged?.Invoke();

        return result;
    }

    public bool IncreaseCost(int x, int y, byte cost)
    {
        if (!grid.TryGetGridObject(x, y, out CostCell cell))
            return false;

        bool result = cell.IncreaseCost(cost);

        grid.TriggerDebugRefresh(x, y);
        CostGridChanged?.Invoke();

        return result;
    }

    public bool DecreaseCost(int x, int y, byte cost)
    {
        if (!grid.TryGetGridObject(x, y, out CostCell cell))
            return false;

        bool result = cell.DecreaseCost(cost);

        grid.TriggerDebugRefresh(x, y);
        CostGridChanged?.Invoke();

        return result;
    }

    public void SetWalkable(int x, int y, bool isWalkable)
    {
        if (!grid.TryGetGridObject(x, y, out CostCell cell))
            return;

        cell.SetWalkable(isWalkable);

        grid.TriggerDebugRefresh(x, y);
        CostGridChanged?.Invoke();
    }
}
