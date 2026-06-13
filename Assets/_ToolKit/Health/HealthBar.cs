using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private HealthComponent healthComponent;
    MeshRenderer meshRenderer;
    MaterialPropertyBlock block;

    void Awake()
    {
        healthComponent = GetComponentInParent<HealthComponent>();
        meshRenderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();

        healthComponent.OnHealthChanged += UpdateHealthBar;
    }

    void UpdateHealthBar(HealthInfo info)
    {
        meshRenderer.GetPropertyBlock(block);
        block.SetFloat("_PercentHealth", info.percentHealth);
        meshRenderer.SetPropertyBlock(block);
    }
}
