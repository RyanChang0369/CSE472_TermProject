using UnityEngine;

/// <summary>
/// Contains methods pertaining to quaternions.
/// </summary>
public static class QuaternionExt
{
    /// <summary>
    /// Moves a quaternion current towards target, using its euler angles.
    /// 
    /// See documentation for
    /// https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html
    /// </summary>
    /// <param name="current">Current quaternion.</param>
    /// <param name="target">Where to move to.</param>
    /// <param name="currentVelocity">The current velocity of the euler angles.</param>
    /// <param name="smoothTime">smoothTime input for SmoothDampAngle.</param>
    /// <param name="maxSpeed">maxSpeed input for SmoothDampAngle.</param>
    /// <param name="deltaTime">deltaTime input for SmoothDampAngle.</param>
    /// <returns></returns>
    public static Quaternion SmoothDampQuaternion(Quaternion current,
        Quaternion target, ref Vector3 currentVelocity, float smoothTime,
        float maxSpeed, float deltaTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler
        (
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime,
                maxSpeed, deltaTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime,
                maxSpeed, deltaTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime,
                maxSpeed, deltaTime)
        );
    }

    /// <inheritdoc cref="Quaternion.Euler(Vector3)"/>
    /// <param name="euler">The euler angles to convert to a quaternion.</param>
    /// <remarks>
    /// Convenience function for <see cref="Quaternion.Euler(Vector3)"/>.
    /// </remarks>
    public static Quaternion ToEuler(this Vector3 euler) =>
        Quaternion.Euler(euler);
}