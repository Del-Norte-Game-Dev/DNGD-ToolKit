using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private ICostGrid costGrid;
    private Grid<FlowFieldCell> flowGrid;

    private FlowFieldCell destination;

    public FlowField(ICostGrid costGrid)
    {
        this.costGrid = costGrid;

        flowGrid = new Grid<FlowFieldCell>(
            costGrid.Width,
            costGrid.Height,
            costGrid.CellSize,
            costGrid.Origin,
            (g, x, y) => new FlowFieldCell(x, y)
        );
    }

    public void Regenerate()
    {
        GenerateIntegrationField(destination);
        GenerateFlowField();
    }

    public bool TryGenerate(Vector3 worldPosition)
    {
        if (!flowGrid.TryGetGridObject(worldPosition, out FlowFieldCell dest))
            return false;

        GenerateIntegrationField(dest);
        GenerateFlowField();

        return true;
    }

    private void ResetGrid()
    {
        for (int x = 0; x < flowGrid.GetWidth(); x++)
        {
            for (int y = 0; y < flowGrid.GetHeight(); y++)
            {
                var cell = flowGrid.GetGridObject(x, y);
                cell.integrationCost = int.MaxValue;
                cell.flowDirection = Vector2.zero;
            }
        }
    }

    private bool TryAddCosts(int currentCost, int tileCost, out int result)
    {
        if (currentCost == int.MaxValue || currentCost > int.MaxValue - tileCost){
            result = int.MaxValue;
            return false;
        }

        result = currentCost + tileCost;
        return true;
    }

    private void GenerateIntegrationField(FlowFieldCell dest)
    {
        ResetGrid();
        destination = dest;
        destination.integrationCost = 0;

        PriorityQueue<FlowFieldCell, int> queue = new PriorityQueue<FlowFieldCell, int>();
        queue.Enqueue(destination, destination.integrationCost);

        while (queue.Count > 0)
        {
            FlowFieldCell cur = queue.Dequeue();

            foreach (Vector2Int direction in Grid<FlowFieldCell>.DIR8_ManhattanBias)
            {
                int nx = cur.x + direction.x;
                int ny = cur.y + direction.y;

                if (!flowGrid.TryGetGridObject(nx, ny, out FlowFieldCell neighbor))
                    continue;

                if (!costGrid.TryGetCell(nx, ny, out ICostCell costCell) || !costCell.IsWalkable())
                    continue;

                // Prevent diagonal corner cutting
                if (direction.x != 0 && direction.y != 0)
                {
                    if (!costGrid.TryGetCell(cur.x + direction.x, cur.y, out ICostCell side1) ||
                        !costGrid.TryGetCell(cur.x, cur.y + direction.y, out ICostCell side2) ||
                        !side1.IsWalkable() ||
                        !side2.IsWalkable())
                    {
                        continue;
                    }
                }

                if (!TryAddCosts(cur.integrationCost, costCell.GetCost(), out int newCost)){
                    Debug.LogWarning("Integration cost overflowed");
                    continue;
                }

                if (newCost < neighbor.integrationCost)
                {
                    neighbor.integrationCost = newCost;
                    queue.Enqueue(neighbor, neighbor.integrationCost);
                }
            }
        }
    }

    private void GenerateFlowField()
    {
        for (int x = 0; x < flowGrid.GetWidth(); x++)
        {
            for (int y = 0; y < flowGrid.GetHeight(); y++)
            {
                var cell = flowGrid.GetGridObject(x, y);

                FlowFieldCell best = null;
                int bestCost = cell.integrationCost;

                foreach (Vector2Int direction in Grid<FlowFieldCell>.DIR8_ManhattanBias)
                {
                    int nx = x + direction.x;
                    int ny = y + direction.y;

                    if (!flowGrid.TryGetGridObject(nx, ny, out FlowFieldCell neighbor))
                        continue;

                    // Prevent diagonal corner cutting
                    if (direction.x != 0 && direction.y != 0)
                    {
                        if (!costGrid.TryGetCell(x + direction.x, y, out ICostCell side1) ||
                            !costGrid.TryGetCell(x, y + direction.y, out ICostCell side2) ||
                            !side1.IsWalkable() ||
                            !side2.IsWalkable())
                        {
                            continue;
                        }
                    }

                    if (!costGrid.TryGetCell(nx, ny, out ICostCell costCell) || !costCell.IsWalkable())
                        continue;

                    if (neighbor.integrationCost < bestCost)
                    {
                        bestCost = neighbor.integrationCost;
                        best = neighbor;
                    }
                }

                if (best != null)
                {
                    Vector2 dir = new Vector2(best.x - cell.x, best.y - cell.y).normalized;
                    cell.flowDirection = dir;
                }
                else
                {
                    cell.flowDirection = Vector2.zero;
                }
            }
        }

        flowGrid.TriggerDebugFullRefresh();
    }

    public bool TryGetFlowDirection(Vector3 worldPosition, out Vector2 dir)
    {
        if (flowGrid.TryGetGridObject(worldPosition, out FlowFieldCell cell))
        {
            dir = cell.flowDirection;
            return true;
        }

        dir = Vector2.zero;
        return false;
    }

    public Vector2 GetFlowDirection(int x, int y)
    {
        return flowGrid.GetGridObject(x ,y).flowDirection;
    }

    public Vector3 GetDestinationWorld()
    {
        return flowGrid.GetWorldPositionCenter(destination.x, destination.y);
    }

    public Grid<FlowFieldCell> GetFlowGrid()
    {
        return flowGrid;
    }

    public ICostGrid GetCostGrid()
    {
        return costGrid;
    }
}