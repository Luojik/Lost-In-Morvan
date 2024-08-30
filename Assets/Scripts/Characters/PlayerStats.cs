using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Parameters")]
    public int maxHealth;
    public HealthBar healthBar;
    private bool isDead;

    private int currentHealth;
    private int healthLevel;

    [Header("Stats Parameters")]
    public int maxDamage;
    public float crits;

    private int damageLevel;
    private int critsLevel;

    [Header("Skill Points Parameters")]
    public int skillPoints;

    [Header("Name Parameters")]
    public string characterName;

    [Header("Text Parameters")]
    public Text healthLevelText;
    public Text damageLevelText;
    public Text critsLevelText;
    public Text skillPointsText;
    public Text characterNameText;

    public int maxHealthAdded;
    public int maxDamageAdded;
    public float critsAdded;

    private static string currentSceneName;

    void Start()
    {
        GameObject healthBarObject = GameObject.FindWithTag("HealthBar");
        GameObject skillPointsTextObject = GameObject.FindWithTag("skillPointsText");
        GameObject characterNameTextObject = GameObject.FindWithTag("characterNameText");

        healthBar = healthBarObject.GetComponent<HealthBar>();
        skillPointsText = skillPointsTextObject.GetComponent<Text>();
        characterNameText = characterNameTextObject.GetComponent<Text>();

        skillPointsText.text = "Skill Points: " + skillPoints;
        characterNameText.text = characterName;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        isDead = false;  

        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public bool HasSceneChanged()
    {
        // Get the name of the currently active scene
        string newSceneName = SceneManager.GetActiveScene().name;

        // Check if the scene has changed
        if (currentSceneName != newSceneName)
        {
            // Update the currentSceneName to the new scene
            currentSceneName = newSceneName;
            return true;
        }
        
        return false;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public int GetSkillPoints()
    {
        return skillPoints;
    }

    void Update()
    {
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
        }

        skillPointsText.text = "Skill Points: " + skillPoints;

        if(HasSceneChanged())
        {
            GameObject healthBarObject = GameObject.FindWithTag("HealthBar");
            GameObject skillPointsTextObject = GameObject.FindWithTag("skillPointsText");
            GameObject characterNameTextObject = GameObject.FindWithTag("characterNameText");

            healthBar = healthBarObject.GetComponent<HealthBar>();
            skillPointsText = skillPointsTextObject.GetComponent<Text>();
            characterNameText = characterNameTextObject.GetComponent<Text>();

            skillPointsText.text = "Skill Points: " + skillPoints;
            characterNameText.text = characterName;

            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            isDead = false;  

            currentSceneName = SceneManager.GetActiveScene().name;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void AddSkillPoint(int nbPoints)
    {
        skillPoints+= nbPoints;
    }

    public void RemoveSkillPoint(int nbPoints)
    {
        skillPoints-= nbPoints;
    }

    public void AddHealth()
    {
        if(skillPoints > 0)
        {
            maxHealth += maxHealthAdded;
            Debug.Log("Health added. New Health: " + maxHealth);
            RemoveSkillPoint(1);
            healthLevel++;
            healthLevelText.text = "Health: " + maxHealth;
        }
    }

    public void AddDamage()
    {
        if(skillPoints > 0)
        {
            maxDamage += maxDamageAdded;
            Debug.Log("Damage added. New Damage: " + maxDamage);
            RemoveSkillPoint(1);
            damageLevel++;
            damageLevelText.text = "Dmg: " + maxDamage;
        }
    }

    public void AddCrits()
    {
        if(skillPoints > 0)
        {
            crits += critsAdded;
            Debug.Log("Crits added. New Crits: " + crits);
            RemoveSkillPoint(1);
            critsLevel++;
            critsLevelText.text = "Crits: " + crits + "%";
        }
    }

    public void RemoveHealth()
    {
        if(maxHealth > 100)
        {
            maxHealth -= maxHealthAdded;
            Debug.Log("Health added. New Health: " + maxHealth);
            AddSkillPoint(1);
            healthLevel--;
            healthLevelText.text = "Health: " + maxHealth;
        }
    }

    public void RemoveDamage()
    {
        if(maxDamage > 15)
        {
            maxDamage -= maxDamageAdded;
            Debug.Log("Damage added. New Damage: " + maxDamage);
            AddSkillPoint(1);
            damageLevel--;
            damageLevelText.text = "Dmg: " + maxDamage;
        }
    }

    public void RemoveCrits()
    {
        if(crits > 0.01f)
        {
            crits -= critsAdded;
            Debug.Log("CritS added. New CritS: " + crits);
            AddSkillPoint(1);
            critsLevel--;
            critsLevelText.text = "Crits: " + crits + "%";
        }
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
