using UnityEngine;

public class FlowFieldManager : GenericSingleton<FlowFieldManager>
{
    private FlowField flowField;
    private FlowFieldVisual visual;
    private ICostGrid costGrid;
    [SerializeField] private FlowFieldVisualConfig config;
    [SerializeField] bool enableDebug = false;

    private Vector3 currentDestination;
    private bool costGridDirty;
    private float costGridDirtyTimer;
    [SerializeField] private float regenerateDelay = 0.05f;

    public void Initialize(ICostGrid costGrid, Vector3 worldDestination)
    {
        this.costGrid = costGrid;
        flowField = new FlowField(costGrid);
        SetDestination(worldDestination);
        costGrid.CostGridChanged += HandleCostGridChanged;

        if (!enableDebug) return;
        visual = new GameObject("Flow Visual")
            .AddComponent<FlowFieldVisual>();
        visual.transform.SetParent(transform, false);
        visual.Initialize(config);
        visual.SetFlowField(flowField);
    }

    private void OnDisable()
    {
        if (costGrid != null)
        {
            costGrid.CostGridChanged -= HandleCostGridChanged;
        }
    }

    private void HandleCostGridChanged()
    {
        costGridDirty = true;
        costGridDirtyTimer = 0f;
    }

    private void Update()
    {
        if (!costGridDirty || flowField == null) return;

        costGridDirtyTimer += Time.deltaTime;
        if (costGridDirtyTimer < regenerateDelay) return;

        costGridDirty = false;
        flowField.Regenerate();
    }

    private void SetDestination(Vector3 worldDestination)
    {
        currentDestination = worldDestination;
        flowField.TryGenerate(currentDestination);
    }

    public FlowField GetFlowField(){
        return flowField;
    }
}