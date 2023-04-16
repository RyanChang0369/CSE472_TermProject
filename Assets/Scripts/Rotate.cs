using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 eulersPerSecond;

    private void Update()
    {
        transform.Rotate(eulersPerSecond * Time.deltaTime);
    }
}