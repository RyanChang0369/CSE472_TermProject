using UnityEngine;

public class GraphPathTracerData : MonoBehaviour
{
    #region Variables
    public Gradient tracerGradient;

    public LineRenderer tracerRenderer;
    #endregion

    private void Awake()
    {
        if (!tracerRenderer)
        {
            tracerRenderer = Resources.Load<LineRenderer>(
                "Pathfinding/Visual/DefaultTracerData"
            );
        }
    }
}