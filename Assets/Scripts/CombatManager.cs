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

    public int damage;

    public int score = 0;

    public Text scoreText;

    void Start()
    {
        LoadQuestionsFromJSON();
        DisplayQuestion();

        soldierHealth = soldier.GetComponent<SoldierHealth>();
        orcHealth = orc.GetComponent<OrcHealth>();

        damage = 15;
    }

    void LoadQuestionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("questions");

        if (jsonFile != null)
        {
            QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonFile.text);
            questions = questionList.questions;
        }
        else
        {
            Debug.LogError("Le fichier JSON n'a pas �t� trouv�.");
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
        // Obtenir une question al�atoire
        currentQuestion = GetRandomQuestion();

        if (currentQuestion != null)
        {
            // Mettre � jour le texte de la question
            motText.text = currentQuestion.mot;

            // Mettre � jour les boutons avec les options de r�ponse
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < currentQuestion.options.Length)
                {
                    optionButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];

                    int index = i; 
                    optionButtons[i].onClick.RemoveAllListeners(); // Nettoyer les anciens listeners
                    optionButtons[i].onClick.AddListener(() => CheckAnswer(index));
                }
            }

            feedbackText.text = ""; // R�initialiser le texte de feedback
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
        if (index == currentQuestion.indexBonneReponse)
        {
            feedbackText.text = "Correct!";
             // StartCoroutine(ActivateForDuration(soldierAnimator, SoldierSlashAnimator, 0.5f));
             soldierAnimator.SetTrigger("Attack");
             PlaySound(correctSound);
             orcHealth.TakeDamage(damage);
             score += 75;
             UpdateScoreUI();
             //OrcHealth.instance.TakeDamage(15);
            // Appliquer des d�g�ts ou d'autres actions appropri�es
        }
        else
        {
            feedbackText.text = $"Incorrect! The correct answer was: '{currentQuestion.correctAnswer}'";
            
             // StartCoroutine(ActivateForDuration(orcAnimator, OrcSlashAnimator, 0.5f));
             orcAnimator.SetTrigger("Attack");
             //soldier.TakeDamage(15);
             PlaySound(incorrectSound);
             soldierHealth.TakeDamage(damage);
            // Appliquer des d�g�ts ou d'autres actions appropri�es
        }

        // Passer � la question suivante apr�s un court d�lai
        currentQuestionIndex++;
        Invoke("DisplayQuestion", 2); // Afficher la question suivante apr�s 2 secondes
    }

    void UpdateScoreUI()
{
    scoreText.text = "Score: " + score.ToString();
}

    public void EndCombat()
    {
        feedbackText.text = "Combat termin�!";
    }

    /* IEnumerator ActivateForDuration(Animator animator, GameObject obj, float duration)
    {
        // Activer le GameObject
        if (obj != null)
        {
            animator.SetTrigger("Attack");
            obj.SetActive(true);
            
        }

        // Attendre pendant la dur�e sp�cifi�e
        yield return new WaitForSeconds(duration);

        // D�sactiver le GameObject
        if (obj != null)
        {
            obj.SetActive(false);
        }
    } */

    void Update()
    {
        if(orcHealth.die)
        {
            GotoNextScene();
        }
        else if(soldierHealth.die)
        {
            Reset();
        }
    }

    public void GotoNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1) % totalScenes;
        SceneManager.LoadScene(nextSceneIndex);
    }

    void Reset()
    {
        SceneManager.LoadScene(0);
    }
}

