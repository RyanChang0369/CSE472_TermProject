using UnityEngine;

/// <summary>
/// Contains methods pertaining to audio sources.
/// </summary>
public static class AudioSourceExt
{
    /// <summary>
    /// Plays a sound using clip.
    /// </summary>
    /// <param name="audioSource">The audio source that plays the sound.</param>
    /// <param name="clip">The audio clip to use.</param>
    /// <param name="volume">Volume to play the clip at.</param>
    public static void PlaySound(this AudioSource audioSource, AudioClip clip, float volume)
    {
        if (clip)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}