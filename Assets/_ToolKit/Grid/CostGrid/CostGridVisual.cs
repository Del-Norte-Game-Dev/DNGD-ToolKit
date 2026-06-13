using UnityEngine;

public class CostGridVisual : MonoBehaviour
{
    private CostGridVisualConfig config;

    private CostGrid costGrid;
    private GridDebugDrawer debugDrawer;

    public void Initialize(CostGridVisualConfig config, CostGrid costGrid)
    {
        this.config = config;
        this.costGrid = costGrid;

        GameObject go = new GameObject("CostGridDebugDrawer");
        go.transform.SetParent(transform, false);

        debugDrawer = go.AddComponent<GridDebugDrawer>();
        debugDrawer.Initialize(costGrid.GetGrid());
        UpdateCostDebug();
    }
    
    private void UpdateCostDebug()
    {
        if (costGrid == null) return;

        debugDrawer.SetColor(true, (x, y) =>
        {
            costGrid.GetGrid().TryGetGridObject(x, y, out CostCell cell);

            if (cell == null || !cell.IsWalkable())
                return config.impassibleColor;

            float t = cell.cost / 5f;
            return Color.Lerp(
                config.minCostColor,
                config.maxCostColor,
                Mathf.Clamp01(t)
            );
        });
    }
}
