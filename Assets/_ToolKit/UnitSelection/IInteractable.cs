using UnityEngine;

public interface IInteractable : ISelectable
{
    public void Interact(Vector3 target);
}
