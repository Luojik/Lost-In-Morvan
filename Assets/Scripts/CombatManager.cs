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

    public GameObject SoldierSlashAnimator;
    public GameObject OrcSlashAnimator;

    public AudioSource soundEffectSource;

    public AudioClip correctSound;
    public AudioClip incorrectSound;

    public Animator soldierAnimator;
    public Animator orcAnimator; 

    public GameObject soldier;
    public GameObject orc;

    private SoldierHealth soldierHealth;
    private OrcHealth orcHealth;

    public int soldierDamage;
    public int orcDamage;

    public int score = 0;
    public Text scoreText;

    public string fileName;

    private bool isFeedbackActive = false;

    public Button continueButton;
    public Button resetButton;

    public GameObject combatPanel;

    public Text timerText;
    private float timeRemaining = 10f;
    private bool isTimerRunning = false;

    void Start()
    {
        LoadQuestionsFromJSON();
        DisplayQuestion();

        soldierHealth = soldier.GetComponent<SoldierHealth>();
        orcHealth = orc.GetComponent<OrcHealth>();
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
            soundEffectSource.PlayOneShot(clip); // Play the sound effect
        }
    }

    void CheckAnswer(int index)
    {
        if (isFeedbackActive) return;

        isFeedbackActive = true;

        isTimerRunning = false;

        if (index == currentQuestion.indexBonneReponse)
        {
            feedbackText.text = "Correct!";
            soldierAnimator.SetTrigger("Attack");
            SoldierSlashAnimator.GetComponent<Animator>().SetTrigger("SoldierSlash");
            orcAnimator.SetTrigger("Hurt");
            PlaySound(correctSound);
            orcHealth.TakeDamage(soldierDamage);
            //score += soldierDamage;
            UpdateScoreUI();
        }
        else
        {
            feedbackText.text = $"Incorrect! The correct answer was: '{currentQuestion.correctAnswer}'";
            orcAnimator.SetTrigger("Attack");
            OrcSlashAnimator.GetComponent<Animator>().SetTrigger("OrcSlash");
            soldierAnimator.SetTrigger("Hurt");
            PlaySound(incorrectSound);
            soldierHealth.TakeDamage(orcDamage);
            //score -= orcDamage;
            UpdateScoreUI();
        }

        EnableButtons(false);
        Invoke("DisplayNextQuestion", 3);
    }

    void DisplayNextQuestion()
    {
        isFeedbackActive = false;
        DisplayQuestion();
    }

    void UpdateScoreUI()
    {
        //scoreText.text = "Score: " + score.ToString();
    }

    public void EndCombat()
    {
        feedbackText.text = "Combat terminé!";
        
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                Debug.Log("Time Remaining: " + timeRemaining);
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

        if (orcHealth.die)
        {   
            orcAnimator.SetTrigger("Death");
            Invoke("EndCombat", 2);
            isTimerRunning = false;
            continueButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);
        }
        else if (soldierHealth.die)
        {
            soldierAnimator.SetTrigger("Death");
            Invoke("EndCombat", 2);
            isTimerRunning = false;
            resetButton.gameObject.SetActive(true);
            combatPanel.gameObject.SetActive(false);
        }
    }

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
}


