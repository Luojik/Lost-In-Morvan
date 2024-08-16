using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Text speakerNameText;
    public AudioSource audioSource;
    public Image speakerImage;

    public DialogueSequence dialogueSequence;
    private int currentDialogueIndex = 0;

    void Start()
    {
        DisplayDialogue();
    }

    void DisplayDialogue()
    {   
        StopAllCoroutines();

        dialogueText.text = "";
        speakerNameText.text = "";

        if (currentDialogueIndex < dialogueSequence.dialogues.Count)
        {
            Dialogue currentDialogue = dialogueSequence.dialogues[currentDialogueIndex];

            dialogueText.text = currentDialogue.dialogueText;
            speakerNameText.text = currentDialogue.speakerName;
            speakerImage.sprite = currentDialogue.speakerImage;


            if (currentDialogue.voiceClip != null)
            {
                audioSource.clip = currentDialogue.voiceClip;
                audioSource.Play();
            }

            StartCoroutine(TypeText(currentDialogue.dialogueText));
        }
        else
        {
            EndDialogue();
        }
    }

    public void NextDialogue()
    {
        currentDialogueIndex++;
        DisplayDialogue();
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void GotoNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1) % totalScenes;
        SceneManager.LoadScene(nextSceneIndex);
    }

    void EndDialogue()
    {
        GotoNextScene();
    }
}
