using UnityEngine;

[CreateAssetMenu(fileName = "CostGridVisualConfig", menuName = "GridDebug/CostGridVisualConfig")]
public class CostGridVisualConfig : ScriptableObject
{
    public Color minCostColor;
    public Color maxCostColor;
    public Color impassibleColor;
}
