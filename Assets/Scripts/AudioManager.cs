using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource; // Reference to the AudioSource that will play dialogue audio

    void Start()
    {
        // Automatically find the first AudioSource component in the scene
        audioSource = FindObjectOfType<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found in the scene.");
        }

        // Test the AdjustAudioVolumeToTarget method
        // Example audio clip - replace with your own clip path or assign via inspector
        AudioClip clip = Resources.Load<AudioClip>("YourAudioClipPath"); // Ensure this is a valid path in the Resources folder
        if (clip != null)
        {
            AdjustAudioVolumeToTarget(clip, 0.5f); // Set volume to 0.5 as an example
        }
        else
        {
            Debug.LogError("Audio clip could not be loaded. Check the path or clip name.");
        }
    }

    // Function to adjust the volume of an audio clip to a consistent level
    public void AdjustAudioVolumeToTarget(AudioClip clip, float targetVolume = 0.5f)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing. Cannot adjust volume.");
            return;
        }

        // Calculate RMS (Root Mean Square) or peak value for loudness comparison
        float rms = GetRMSValue(clip);
        float volumeFactor = targetVolume / rms;

        // Clamp volume factor to ensure it's within a valid range
        audioSource.volume = Mathf.Clamp(volumeFactor, 0f, 1f);

        // Apply the clip to the AudioSource and play it
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Function to calculate the RMS value of an audio clip (used for volume normalization)
    float GetRMSValue(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Calculate the RMS (Root Mean Square) of the sample data
        float sum = 0.0f;
        foreach (float sample in samples)
        {
            sum += sample * sample; // square each sample value
        }

        return Mathf.Sqrt(sum / samples.Length); // return RMS value
    }
}



