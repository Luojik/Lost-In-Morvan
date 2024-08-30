using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrcStats : MonoBehaviour
{
    [Header("Health Parameters")]
    public int maxHealth;
    public HealthBar healthBar;
    private int currentHealth;
    private bool isDead;

    [Header("Stats Parameters")]
    public int maxDamage;
    public float crits;

    [Header("Name Parameters")]
    public string orcName;

    [Header("Text Parameters")]
    public Text orcNameText;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isDead = false;

        orcNameText.text = orcName;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    void Update()
    {
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public int GetDamage()
    {
        int randomVariation = Random.Range(-5, 6);
        int damageWithVariation = maxDamage + randomVariation;

        bool isCriticalHit = Random.value < crits;
        if (isCriticalHit)
        {
            damageWithVariation *= 2;
            Debug.Log("Critical hit! Damage doubled.");
        }

        int finalDamage = Mathf.CeilToInt(damageWithVariation);

        Debug.Log("Final Damage: " + finalDamage);

        return finalDamage;
    }
}

