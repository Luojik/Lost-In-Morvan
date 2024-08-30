using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public Text motText;
    public Button[] optionButtons;
    public Text feedbackText;

    public List<Question> questions;
    private Question currentQuestion;

    private int currentQuestionIndex = 0;

    public string fileName;

    public AudioSource soundEffectSource; 

    private bool isFeedbackActive = false;

    public Button continueButton;
    public Button resetButton;

    public GameObject combatPanel;
    public GameObject levelUpPanel;

    public GameObject endOfCombatOptions;

    public Text timerText;
    private float timeRemaining = 10f;
    private bool isTimerRunning = false;

    public float xOffset;
    public float yOffset;

    public int nbMistakes;

    [Header("Soldier Parameters")]
    public GameObject soldier;
    private Animator soldierAnimator;
    private PlayerStats soldierStats;
    private DamageEffect soldierDamageEffect;
    public GameObject SoldierSlashAnimator;
    public AudioClip soldierSlashSound;
    
    [Header("Orc Parameters")]
    public GameObject orc;
    private Animator orcAnimator; 
    private OrcStats orcStats;
    private DamageEffect orcDamageEffect;
    public GameObject OrcSlashAnimator;
    public AudioClip orcSlashSound;

    void Start()
    {
        LoadQuestionsFromJSON();
        DisplayQuestion();

        soldier = GameObject.FindWithTag("Player");

        soldierAnimator = soldier.GetComponent<Animator>();
        soldierStats = soldier.GetComponent<PlayerStats>();
        soldierDamageEffect = soldier.GetComponent<DamageEffect>();

        orcAnimator = orc.GetComponent<Animator>();
        orcStats = orc.GetComponent<OrcStats>();

        nbMistakes = 0;
    }

    void LoadQuestionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonFile.text);
            questions = questionList.questions;
        }
        else
        {
            Debug.LogError("Le fichier JSON n'a pas été trouvé.");
        }
    }

    Question GetRandomQuestion()
    {
        if (questions != null && questions.Count > 0)
        {
            int randomIndex = Random.Range(0, questions.Count);
            return questions[randomIndex];
        }
        else
        {
            Debug.LogError("Aucune question disponible.");
            return null;
        }
    }

    void DisplayQuestion()
    {
        currentQuestion = GetRandomQuestion();

        if (currentQuestion != null)
        {
            motText.text = currentQuestion.mot;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < currentQuestion.options.Length)
                {
                    optionButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];

                    int index = i; 
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => CheckAnswer(index));
                }
            }

            feedbackText.text = "";
            EnableButtons(true);

            timeRemaining = 10f;
            isTimerRunning = true;
            UpdateTimerUI();
        }
    }

    void EnableButtons(bool isEnabled)
    {
        foreach (Button button in optionButtons)
        {
            button.interactable = isEnabled;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (soundEffectSource != null && clip != null)
        {
            soundEffectSource.PlayOneShot(clip);
        }
    }

    void CheckAnswer(int index)
    {
        if (isFeedbackActive) return;

        isFeedbackActive = true;

        isTimerRunning = false;

        if (index == -1)
        {
            feedbackText.text = $"Time's up! The correct answer was: '{currentQuestion.correctAnswer}'";
            orcAnimator.SetTrigger("Attack");
            OrcSlashAnimator.GetComponent<Animator>().SetTrigger("OrcSlash");
            soldierAnimator.SetTrigger("Hurt");
            PlaySound(orcSlashSound);

            int damage = orcStats.GetDamage();
            soldierStats.TakeDamage(damage);

            soldierDamageEffect.ShowDamage(damage, soldier.transform.position, -xOffset, yOffset);

            nbMistakes++;
        }
        else if (index != currentQuestion.indexBonneReponse)
        {
            feedbackText.text = $"Incorrect! The correct answer was: '{currentQuestion.correctAnswer}'";
            orcAnimator.SetTrigger("Attack");
            OrcSlashAnimator.GetComponent<Animator>().SetTrigger("OrcSlash");
            soldierAnimator.SetTrigger("Hurt");
            PlaySound(orcSlashSound);

            int damage = orcStats.GetDamage();
            soldierStats.TakeDamage(damage);

            soldierDamageEffect.ShowDamage(damage, soldier.transform.position, -xOffset, yOffset);

            nbMistakes++;
        }
        else
        {
            feedbackText.text = "Correct !";
            soldierAnimator.SetTrigger("Attack");
            SoldierSlashAnimator.GetComponent<Animator>().SetTrigger("SoldierSlash");
            orcAnimator.SetTrigger("Hurt");
            PlaySound(soldierSlashSound);

            int damage = soldierStats.GetDamage();
            orcStats.TakeDamage(damage);

            orcDamageEffect.ShowDamage(damage, orc.transform.position, xOffset, yOffset);
        }

        EnableButtons(false);
        Invoke("DisplayNextQuestion", 3);
    }

    void DisplayNextQuestion()
    {
        isFeedbackActive = false;
        DisplayQuestion();
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
    }

    private bool isTextComponentsAssigned = false;

    void Update()
    {
        
        soldierStats.skillPointsText.text = "Skill Points: " + soldierStats.skillPoints;

        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                // Debug.Log("Time Remaining: " + timeRemaining);
                UpdateTimerUI();
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
                Debug.Log("Timer Expired");
                UpdateTimerUI();
                CheckAnswer(-1);
            }
        }

        if (orcStats.GetIsDead())
        {   
            orcAnimator.SetTrigger("Death");
            isTimerRunning = false;
            // endOfCombatOptions.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);

            feedbackText.text = "You Won!";

            levelUpPanel.gameObject.SetActive(true);
           
            if (!isTextComponentsAssigned)
            {
                GameObject healthLevelTextObject = GameObject.FindWithTag("healthLevelText");
                if (healthLevelTextObject != null)
                {
                    Debug.Log("GameObject with tag 'healthLevelText' found.");
                    soldierStats.healthLevelText = healthLevelTextObject.GetComponent<Text>();
                    if (soldierStats.healthLevelText != null)
                    {
                        Debug.Log("Text component successfully retrieved for 'healthLevelText'.");
                    }
                    else
                    {
                        Debug.LogError("Text component not found on the GameObject with tag 'healthLevelText'.");
                    }
                }
                else
                {
                    Debug.LogError("No GameObject with the tag 'healthLevelText' found.");
                }

                GameObject damageLevelTextObject = GameObject.FindWithTag("damageLevelText");
                if (damageLevelTextObject != null)
                {
                    Debug.Log("GameObject with tag 'damageLevelText' found.");
                    soldierStats.damageLevelText = damageLevelTextObject.GetComponent<Text>();
                    if (soldierStats.damageLevelText != null)
                    {
                        Debug.Log("Text component successfully retrieved for 'damageLevelText'.");
                    }
                    else
                    {
                        Debug.LogError("Text component not found on the GameObject with tag 'damageLevelText'.");
                    }
                }
                else
                {
                    Debug.LogError("No GameObject with the tag 'damageLevelText' found.");
                }

                GameObject critsLevelTextObject = GameObject.FindWithTag("critsLevelText");
                if (critsLevelTextObject != null)
                {
                    Debug.Log("GameObject with tag 'critsLevelText' found.");
                    soldierStats.critsLevelText = critsLevelTextObject.GetComponent<Text>();
                    if (soldierStats.critsLevelText != null)
                    {
                        Debug.Log("Text component successfully retrieved for 'critsLevelText'.");
                    }
                    else
                    {
                        Debug.LogError("Text component not found on the GameObject with tag 'critsLevelText'.");
                    }
                }
                else
                {
                    Debug.LogError("No GameObject with the tag 'critsLevelText' found.");
                }

                isTextComponentsAssigned = true;

                Debug.Log("Text components assigned.");

                soldierStats.skillPoints += GiveSkillPoints();

                soldierStats.healthLevelText.text = "Health: " + soldierStats.maxHealth;
                soldierStats.damageLevelText.text = "Damage: " + soldierStats.maxDamage;
                soldierStats.critsLevelText.text = "Crits: " + soldierStats.crits + "%";
            }
        }
        else if (soldierStats.GetIsDead())
        {
            soldierAnimator.SetTrigger("Death");
            isTimerRunning = false;
            resetButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);
            feedbackText.text = "You Lose!";
            
            if(once)
            {
                soldierStats.skillPoints += GiveSkillPoints();
                once = true;
            }
        }
    }

    private bool once = false;

    public void GotoNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1) % totalScenes;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public int GiveSkillPoints()
    {
        if(nbMistakes == 0)
        {
            return 3;
        }
        else if(nbMistakes > 0 || nbMistakes < 3)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}


