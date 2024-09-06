using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaySoundOnButtonPress : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource != null)
        {
            Debug.LogError("audioSource not assigned in the Inspector");
            return;
        }

        if (hoverSound == null || clickSound == null)
        {
            Debug.LogError("HoverSound or ClickSound not assigned in the Inspector");
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }
}
