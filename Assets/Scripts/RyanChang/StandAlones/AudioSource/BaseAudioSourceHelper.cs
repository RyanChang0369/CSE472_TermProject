using UnityEngine;


public abstract class BaseAudioSourceHelper : MonoBehaviour
{
    public AudioSource audioSource;

    // Use this for initialization
    protected virtual void Start()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }
}
