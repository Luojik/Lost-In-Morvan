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
    public GameObject SkillPanel;
    public GameObject EndOptions;

    public Text timerText;
    private float timeRemaining = 10f;
    private bool isTimerRunning = false;

    public float xOffset;
    public float yOffset;

    private int nbMistakes;
    private bool once;

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

        if (soldier != null)
        {
            soldierAnimator = soldier.GetComponent<Animator>();
            soldierStats = soldier.GetComponent<PlayerStats>();
            soldierDamageEffect = soldier.GetComponent<DamageEffect>();
        }
        else
        {
            Debug.LogError("Player GameObject not found with tag 'Player'.");
        }

        if (orc != null)
        {
            orcAnimator = orc.GetComponent<Animator>();
            orcStats = orc.GetComponent<OrcStats>();
            orcDamageEffect = orc.GetComponent<DamageEffect>();
        }
        else
        {
            Debug.LogError("Orc GameObject not assigned.");
        }

        nbMistakes = 0;
        once = true;
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

    void DisplayNextQuestion()
    {
        isFeedbackActive = false;
        DisplayQuestion();
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

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
    }

    void Update()
    {
        soldierStats.skillPointsText.text = soldierStats.skillPoints.ToString();

        if (!soldierStats.GetIsDead() && soldierAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            soldierAnimator.SetTrigger("Restart");
        }

        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
                UpdateTimerUI();
                CheckAnswer(-1);
            }
        }

        if (soldierStats.GetIsDead())
        {
            soldierAnimator.SetTrigger("Death");
            isTimerRunning = false;
            EndOptions.gameObject.SetActive(true);
            resetButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);
            feedbackText.text = "You Lose!";
            
            if(once)
            {
                soldierStats.skillPoints += GiveSkillPoints();
                once = false;
            }
        }
        
        else if (orcStats.GetIsDead())
        {   
            orcAnimator.SetTrigger("Death");
            isTimerRunning = false;
            EndOptions.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);
            feedbackText.text = "You Win!";

            levelUpPanel.gameObject.SetActive(true);
            SkillPanel.gameObject.SetActive(true);
           
            if (once)
            {
                GameObject healthLevelTextObject = GameObject.FindWithTag("healthLevelText");
                soldierStats.healthLevelText = healthLevelTextObject.GetComponent<Text>();
                   
                GameObject damageLevelTextObject = GameObject.FindWithTag("damageLevelText");
                soldierStats.damageLevelText = damageLevelTextObject.GetComponent<Text>();
                
                GameObject critsLevelTextObject = GameObject.FindWithTag("critsLevelText");
                soldierStats.critsLevelText = critsLevelTextObject.GetComponent<Text>();

                soldierStats.skillPoints += GiveSkillPoints();

                soldierStats.healthLevelText.text = "Health: " + soldierStats.maxHealth;
                soldierStats.damageLevelText.text = "Dmg: " + soldierStats.maxDamage;
                soldierStats.critsLevelText.text = "Crits: " + soldierStats.crits + "%";

                once = false;
            }

            soldierStats.healthLevelText.text = "Health: " + soldierStats.maxHealth;
            soldierStats.damageLevelText.text = "Dmg: " + soldierStats.maxDamage;
            soldierStats.critsLevelText.text = "Crits: " + soldierStats.crits + "%";
        }  
    }

    public int GiveSkillPoints()
    {
        if (nbMistakes == 0)
        {
            return 3;
        }
        else if (nbMistakes < 3)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}


