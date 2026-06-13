using System;
using UnityEngine;

public class CostGridManager : GenericSingleton<CostGridManager>
{
    public event Action CostGridChanged;

    [SerializeField] private bool showDebug = true;
    [SerializeField] private CostGridVisualConfig config;

    private CostGrid costGrid;
    private CostGridVisual visual;
    private bool isInitialized;

    public void Initialize(int width, int height, float cellSize, Vector3 origin)
    {
        if (isInitialized)
            return;

        costGrid = new CostGrid(width, height, cellSize, origin);
        costGrid.CostGridChanged += HandleCostGridChanged;

        if (showDebug)
        {
            visual = new GameObject("CostGridVisual")
                .AddComponent<CostGridVisual>();

            visual.transform.SetParent(transform, false);
            visual.Initialize(config, costGrid);
        }

        isInitialized = true;
    }

    private void OnDisable()
    {
        if (costGrid != null)
            costGrid.CostGridChanged -= HandleCostGridChanged;
    }

    private void HandleCostGridChanged()
    {
        CostGridChanged?.Invoke();
    }
    
    public CostGrid GetCostGrid() => costGrid;
}
