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

    private PauseMenu pauseMenu;

    void Start()
    {
        Debug.Log("Start method called.");

        GameObject healthBarObject = GameObject.FindWithTag("HealthBar");
        GameObject skillPointsTextObject = GameObject.FindWithTag("skillPointsText");
        GameObject characterNameTextObject = GameObject.FindWithTag("characterNameText");

        GameObject pauseMenu = GameObject.FindWithTag("PauseMenu");

        if (healthBarObject != null)
        {
            healthBar = healthBarObject.GetComponent<HealthBar>();
        }
        else
        {
            Debug.LogError("HealthBar object not found.");
        }

        if (skillPointsTextObject != null)
        {
            skillPointsText = skillPointsTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("SkillPointsText object not found.");
        }

        if (characterNameTextObject != null)
        {
            characterNameText = characterNameTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("CharacterNameText object not found.");
        }

        if (healthBar != null && skillPointsText != null && characterNameText != null)
        {
            skillPointsText.text = skillPoints.ToString();
            characterNameText.text = characterName;

            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            isDead = false;
        }
        else
        {
            Debug.LogError("One or more Text components are missing.");
        }

        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public bool HasSceneChanged()
    {
        string newSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName != newSceneName)
        {
            currentSceneName = newSceneName;
            Debug.Log($"Scene changed to: {newSceneName}");
            return true;
        }

        return false;
    }

    bool IsCombatScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName.Contains("Combat");
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
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
        }

        if (skillPointsText != null)
        {
            skillPointsText.text = skillPoints.ToString();
        }

        /* if(pauseMenu.GetPauseMenuIsOpen())
        {
            GameObject skillPointsTextObject = GameObject.FindWithTag("skillPointsText");
            skillPointsText.text = skillPoints.ToString();

            GameObject healthLevelTextObject = GameObject.FindWithTag("healthLevelText");
            GameObject damageLevelTextObject = GameObject.FindWithTag("damageLevelText");
            GameObject critsLevelTextObject = GameObject.FindWithTag("critsLevelText");

            healthLevelText.text = "Health: " + maxHealth;
            damageLevelText.text = "Dmg: " + maxDamage;
            critsLevelText.text = "Crits: " + crits + "%";
        } */

        if (HasSceneChanged() && IsCombatScene())
        {
            Debug.Log("Updating combat scene parameters.");
            GameObject healthBarObject = GameObject.FindWithTag("HealthBar");
            GameObject skillPointsTextObject = GameObject.FindWithTag("skillPointsText");
            GameObject characterNameTextObject = GameObject.FindWithTag("characterNameText");

            if (healthBarObject != null)
            {
                healthBar = healthBarObject.GetComponent<HealthBar>();
            }

            if (skillPointsTextObject != null)
            {
                skillPointsText = skillPointsTextObject.GetComponent<Text>();
            }

            if (characterNameTextObject != null)
            {
                characterNameText = characterNameTextObject.GetComponent<Text>();
            }

            if (healthBar != null && skillPointsText != null && characterNameText != null)
            {
                skillPointsText.text = skillPoints.ToString();
                characterNameText.text = characterName;

                currentHealth = maxHealth;
                healthBar.SetMaxHealth(maxHealth);
                isDead = false;

                currentSceneName = SceneManager.GetActiveScene().name;
            }
            else
            {
                Debug.LogError("Failed to reassign Text components.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void AddSkillPoint(int nbPoints)
    {
        skillPoints += nbPoints;
    }

    public void RemoveSkillPoint(int nbPoints)
    {
        skillPoints -= nbPoints;
    }

    public void AddHealth()
    {
        if (skillPoints > 0)
        {
            maxHealth += maxHealthAdded;
            Debug.Log("Health added. New Health: " + maxHealth);
            RemoveSkillPoint(1);
            healthLevel++;
            if (healthLevelText != null)
            {
                healthLevelText.text = "Health: " + maxHealth;
            }
        }
    }

    public void AddDamage()
    {
        if (skillPoints > 0)
        {
            maxDamage += maxDamageAdded;
            Debug.Log("Damage added. New Damage: " + maxDamage);
            RemoveSkillPoint(1);
            damageLevel++;
            if (damageLevelText != null)
            {
                damageLevelText.text = "Dmg: " + maxDamage;
            }
        }
    }

    public void AddCrits()
    {
        if (skillPoints > 0)
        {
            crits += critsAdded;
            Debug.Log("Crits added. New Crits: " + crits);
            RemoveSkillPoint(1);
            critsLevel++;
            if (critsLevelText != null)
            {
                critsLevelText.text = "Crits: " + crits + "%";
            }
        }
    }

    public void RemoveHealth()
    {
        if (maxHealth > 100)
        {
            maxHealth -= maxHealthAdded;
            Debug.Log("Health reduced. New Health: " + maxHealth);
            AddSkillPoint(1);
            healthLevel--;
            if (healthLevelText != null)
            {
                healthLevelText.text = "Health: " + maxHealth;
            }
        }
    }

    public void RemoveDamage()
    {
        if (maxDamage > 15)
        {
            maxDamage -= maxDamageAdded;
            Debug.Log("Damage reduced. New Damage: " + maxDamage);
            AddSkillPoint(1);
            damageLevel--;
            if (damageLevelText != null)
            {
                damageLevelText.text = "Dmg: " + maxDamage;
            }
        }
    }

    public void RemoveCrits()
    {
        if (crits > 0.01f)
        {
            crits -= critsAdded;
            Debug.Log("Crits reduced. New Crits: " + crits);
            AddSkillPoint(1);
            critsLevel--;
            if (critsLevelText != null)
            {
                critsLevelText.text = "Crits: " + crits + "%";
            }
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
