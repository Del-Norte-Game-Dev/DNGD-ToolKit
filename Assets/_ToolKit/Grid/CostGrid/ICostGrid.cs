using System;
using UnityEngine;

public interface ICostCell
{
    public byte GetCost();
    public bool IsWalkable();
}


public interface ICostGrid
{
    event Action CostGridChanged;

    int Width { get; }
    int Height { get; }
    float CellSize { get; }
    Vector3 Origin { get; }

    ICostCell GetCell(int x, int y);
    bool TryGetCell(int x, int y, out ICostCell cell);
}
