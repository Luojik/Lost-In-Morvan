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

    private AudioClip currentBackgroundMusic;
    public AudioSource backgroundMusicSource;

    private SceneLoader sceneLoader;

    public Animator speakerAnimator;

    private string previousSpeakerName = "";

    private float defaultBackgroundMusicVolume = 0.5f;

    void Start()
    {
        DisplayDialogue();

        // defaultBackgroundMusicVolume = backgroundMusicSource.volume;

        sceneLoader = GetComponent<SceneLoader>();

        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader component is missing on this GameObject.");
        }

        if (speakerImage != null)
        {
            speakerAnimator = speakerImage.GetComponent<Animator>();
            if (speakerAnimator == null)
            {
                Debug.LogError("Animator component is missing on speakerImage.");
            }
        }
        else
        {
            Debug.LogError("Speaker Image is not assigned.");
        }
    }

    void DisplayDialogue()
    {   
        StopAllCoroutines();

        dialogueText.text = "";
        speakerNameText.text = "";

        if (currentDialogueIndex < dialogueSequence.dialogues.Count)
        {
            Dialogue currentDialogue = dialogueSequence.dialogues[currentDialogueIndex];

            if (speakerAnimator != null && currentDialogue.speakerName != previousSpeakerName)
            {
                speakerAnimator.SetTrigger("Speaks");
            }
            else
            {
                Debug.LogError("SpeakerAnimator is not assigned.");
            }
            previousSpeakerName = currentDialogue.speakerName;

            dialogueText.text = currentDialogue.dialogueText;
            speakerNameText.text = currentDialogue.speakerName;
            speakerImage.sprite = currentDialogue.speakerImage;

            AdjustAudioVolumes(currentDialogue.speakerName);

            if (currentDialogue.voiceClip != null)
            {
                audioSource.clip = currentDialogue.voiceClip;
                audioSource.Play();
            }

            if (currentBackgroundMusic != currentDialogue.backgroundMusic)
            {
                backgroundMusicSource.clip = currentDialogue.backgroundMusic;
                backgroundMusicSource.Play();
                currentBackgroundMusic = currentDialogue.backgroundMusic;
            }

            StartCoroutine(TypeText(currentDialogue.dialogueText, currentDialogue.voiceClip));
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

    IEnumerator TypeText(string text, AudioClip voiceClip)
    {
        dialogueText.text = "";

        float typingSpeed = CalculateTypingDuration(voiceClip, text);

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void GotoNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1) % totalScenes;
        
        string nextSceneName = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        
        sceneLoader.LoadLevel(nextSceneName);
    }

    void EndDialogue()
    {
        GotoNextScene();
    }

    float CalculateTypingDuration(AudioClip voiceClip, string text)
    {
        if (voiceClip == null || text.Length == 0)
        {
            return 0.1f;
        }

        float clipDuration = voiceClip.length;
        float textLength = text.Length;

        float typingDuration = clipDuration / textLength * 0.7f;

        return typingDuration;
    }

    void AdjustAudioVolumes(string speakerName)
{
    backgroundMusicSource.volume = defaultBackgroundMusicVolume * 0.3f;

    if (audioSource != null)
    {
        audioSource.volume = 1.0f;
        audioSource.pitch = 1.0f;

        // Check if the speaker is William Fletcher and double the audio volume
        if (speakerName == "William Fletcher")
        {
            audioSource.volume = audioSource.volume * 2.0f;  // Double the volume
            Debug.Log("Audio volume doubled for William Fletcher.");  // Log the message
        }
    }
}

}
