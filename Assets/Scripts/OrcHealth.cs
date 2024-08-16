using System.Collections;
using UnityEngine;

public class OrcHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    public bool die;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        die = false;
    }

    void Update()
    {
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            die = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}


