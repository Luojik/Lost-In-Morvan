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
        // Obtenir une question aléatoire
        currentQuestion = GetRandomQuestion();

        if (currentQuestion != null)
        {
            // Mettre à jour le texte de la question
            motText.text = currentQuestion.mot;

            // Mettre à jour les boutons avec les options de réponse
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

            feedbackText.text = ""; // Réinitialiser le texte de feedback
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
            // Appliquer des dégâts ou d'autres actions appropriées
        }
        else
        {
            feedbackText.text = $"Incorrect! The correct answer was: '{currentQuestion.correctAnswer}'";
            
             // StartCoroutine(ActivateForDuration(orcAnimator, OrcSlashAnimator, 0.5f));
             orcAnimator.SetTrigger("Attack");
             //soldier.TakeDamage(15);
             PlaySound(incorrectSound);
             soldierHealth.TakeDamage(damage);
            // Appliquer des dégâts ou d'autres actions appropriées
        }

        // Passer à la question suivante après un court délai
        currentQuestionIndex++;
        Invoke("DisplayQuestion", 2); // Afficher la question suivante après 2 secondes
    }

    void UpdateScoreUI()
{
    scoreText.text = "Score: " + score.ToString();
}

    public void EndCombat()
    {
        feedbackText.text = "Combat terminé!";
    }

    /* IEnumerator ActivateForDuration(Animator animator, GameObject obj, float duration)
    {
        // Activer le GameObject
        if (obj != null)
        {
            animator.SetTrigger("Attack");
            obj.SetActive(true);
            
        }

        // Attendre pendant la durée spécifiée
        yield return new WaitForSeconds(duration);

        // Désactiver le GameObject
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

