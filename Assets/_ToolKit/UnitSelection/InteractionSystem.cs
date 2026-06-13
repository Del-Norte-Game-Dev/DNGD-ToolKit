using System.Net.Sockets;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    private void OnEnable()
    {
        SelectionManager.Instance.OnNewSelection += HandleUnitOrder;
    }

    private void Start()
    {
        SelectionManager.Instance.OnNewSelection += HandleUnitOrder;
    }

    private void HandleUnitOrder(SelectionContext selectionOrder)
    {
        foreach (IInteractable interactable in selectionOrder.selected)
        {
            interactable.Interact(selectionOrder.target);
        }
    }

    void OnDisable()
    {
        if (SelectionManager.Instance == null) return;
        SelectionManager.Instance.OnNewSelection -= HandleUnitOrder;
    }
}