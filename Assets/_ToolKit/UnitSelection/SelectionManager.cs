using UnityEngine;
using System.Collections.Generic;
using System;

public class SelectionManager : GenericSingleton<SelectionManager>
{
    [SerializeField] private int mouseButton = 0;
    [SerializeField] private KeyCode additiveKey = KeyCode.LeftShift;

    private bool isDragging;
    private Vector3 dragStart;
    private Vector3 dragEnd;

    public Action<SelectionContext> OnNewSelection;

    private List<ISelectable> selected = new List<ISelectable>();

    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            isDragging = true;
            dragStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(mouseButton) && isDragging)
        {
            dragEnd = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(mouseButton))
        {
            isDragging = false;
            dragEnd = Input.mousePosition;

            if (Vector2.Distance(dragStart, dragEnd) < 10f)
                HandleClick(ScreenToWorld(Input.mousePosition));
            else
                SelectUnitsInBox(dragStart, dragEnd);
        }
    }

    void HandleClick(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null)
        {
            ISelectable unit = hit.GetComponent<ISelectable>();

            if (unit != null)
            {
                if (!Input.GetKey(additiveKey))
                    ClearSelection();

                SelectUnit(unit);
                return;
            }
        }
        
        //create a move order
        if (selected.Count > 0)
            CreateSelection(worldPos);
    }

    void SelectUnitsInBox(Vector2 dragStart, Vector2 dragEnd)
    {
        bool additive = Input.GetKey(additiveKey);
        if (!additive)
            ClearSelection();

        Vector2 p1 = ScreenToWorld(dragStart);
        Vector2 p2 = ScreenToWorld(dragEnd);

        Vector2 min = Vector2.Min(p1, p2);
        Vector2 max = Vector2.Max(p1, p2);

        Collider2D[] hits = Physics2D.OverlapAreaAll(min, max);

        foreach (var col in hits)
        {
            ISelectable unit = col.GetComponent<ISelectable>();
            if (unit != null)
                SelectUnit(unit);
        }
    }

    void SelectUnit(ISelectable unit)
    {
        if (selected.Contains(unit)) return;

        selected.Add(unit);
        unit.SetSelected(true);
    }

    void ClearSelection()
    {
        foreach (var unit in selected)
        {
            if (unit != null)
                unit.SetSelected(false);
        }

        selected.Clear();
    }

    void CreateSelection(Vector2 target)
    {
        SelectionContext order = new SelectionContext(selected, target);
        OnNewSelection?.Invoke(order);
    }

    Vector2 ScreenToWorld(Vector3 screen)
    {
        float z = -Camera.main.transform.position.z;
        screen.z = z;
        return Camera.main.ScreenToWorldPoint(screen);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isDragging) return;

        Vector2 p1 = ScreenToWorld(dragStart);
        Vector2 p2 = ScreenToWorld(dragEnd);

        Vector2 min = Vector2.Min(p1, p2);
        Vector2 max = Vector2.Max(p1, p2);

        Vector3 bl = new Vector3(min.x, min.y, 0);
        Vector3 br = new Vector3(max.x, min.y, 0);
        Vector3 tr = new Vector3(max.x, max.y, 0);
        Vector3 tl = new Vector3(min.x, max.y, 0);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);
    }
}