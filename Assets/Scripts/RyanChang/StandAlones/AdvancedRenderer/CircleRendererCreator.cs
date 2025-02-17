using UnityEngine;

public class CircleRendererCreator : AdvancedLineRendererCreator
{
    [Header("User settings")]
    [Tooltip("Number of points that make up the circle.")]
    [SerializeField]
    private int points = 60;

    [Tooltip("Radius of the circle.")]
    [SerializeField]
    private float radius = 1;

    public float Radius => radius;

    protected override void Start()
    {
        base.Start();

        alr.transforms = new Transform[points];
        for (int i = 0; i < points; i++)
        {
            var pointGO = new GameObject($"point_{i}");
            pointGO.transform.parent = transform;
            alr.transforms[i] = pointGO.transform;
        }

        UpdateCircle(radius);
    }

    /// <summary>
    /// Updates circle to the new radius. The number of points is NOT updated.
    /// </summary>
    /// <param name="newRadius">New radius of circle.</param>
    public void UpdateCircle(float newRadius)
    {
        radius = newRadius;

        float deltaAngle = FloatAngle.PI2 / points;
        Vector2 topPosition = new(radius, 0);

        foreach (var child in alr.transforms)
        {
            child.localPosition = topPosition;
            topPosition = topPosition.RotateVector2(deltaAngle);
        }
    }
}