using UnityEngine;
using System;

public struct HealthInfo
{
    public float damage, currentHealth, percentHealth;

    public HealthInfo(float damage, float currentHealth, float percentHealth)
    {
        this.damage = damage;
        this.currentHealth = currentHealth;
        this.percentHealth = percentHealth;
    }
}

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float CurrentHealth = 10f;
    [SerializeField] private float MaxHealth = 10f;

    public float HealthPercent => MaxHealth <= 0 ? 0 : CurrentHealth / MaxHealth;
    public event Action<HealthInfo> OnHealthChanged; // change to a struct
    public event Action OnDead;

    void Awake()
    {
        OnHealthChanged?.Invoke(new HealthInfo(0f, CurrentHealth, HealthPercent));
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || CurrentHealth <= 0f) return;

        CurrentHealth -= damage;
        CurrentHealth = Math.Max(CurrentHealth, 0);

        OnHealthChanged?.Invoke(new HealthInfo(0f, CurrentHealth, HealthPercent));

        if (CurrentHealth == 0)
            OnDead?.Invoke();
    }
}
