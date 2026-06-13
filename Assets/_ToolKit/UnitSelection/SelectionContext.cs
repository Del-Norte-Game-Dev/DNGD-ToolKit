using UnityEngine;
using System.Collections.Generic;

public struct SelectionContext
{
    public readonly List<ISelectable> selected;
    public readonly Vector3 target;

    public SelectionContext(List<ISelectable> selected, Vector3 target)
    {
        this.selected = selected;
        this.target = target;
    }
}