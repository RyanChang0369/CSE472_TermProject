using UnityEngine;

/// <summary>
/// Changes the pitch of an audio source based on the velocity of a rigidbody.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public class SpeedAudioSourceHelper : BaseAudioSourceHelper
{
    protected float targetPitch;

    public float minPitch = 0.8f, maxPitch = 1.2f;

    [Tooltip("Pitch change per second.")]
    public float pitchChange = 0.5f;

    [Header("Specify max speed and rigidbody.")]
    public float maxSpeed = 10;
    public Rigidbody2D measureFrom;

    protected override void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        targetPitch = Mathf.Lerp(minPitch, maxPitch,
            measureFrom.linearVelocity.magnitude / maxSpeed);

        DeltaPitch();
    }

    private void DeltaPitch()
    {
        if (!targetPitch.Approx(audioSource.pitch))
            audioSource.pitch += ((audioSource.pitch > targetPitch) ? -pitchChange : pitchChange) * Time.deltaTime;

        if (audioSource.pitch <= 0 || audioSource.pitch.Approx(0))
        {
            audioSource.pitch = 0;
        }

        audioSource.mute = audioSource.pitch <= 0;
    }
}
