using UnityEngine;

public class AdvancedRotation : MonoBehaviour
{
    [System.Serializable]
    public struct RotationEquation
    {
        public enum TrigFunction
        {
            Sin, Cos
        }

        public TrigFunction trigFunction;

        public float amplitude;

        public float frequency;

        public float phase;

        public float offset;

        public RotationEquation(float a, float f, float p, float o)
        {
            amplitude = a;
            frequency = f;
            phase = p;
            offset = o;

            trigFunction = TrigFunction.Sin;
        }

        public float Evaluate(float t)
        {
            switch (trigFunction)
            {
                default:
                case TrigFunction.Sin:
                    return amplitude *
                        Mathf.Sin(frequency * t + phase) +
                        offset;
                case TrigFunction.Cos:
                    return amplitude *
                        Mathf.Cos(frequency * t + phase) +
                        offset; 
            }
        }
    }

    public RotationEquation xEquation = new(1, 1, 0, 0);
    public RotationEquation yEquation = new(1, 1, 0, 0);
    public RotationEquation zEquation = new(1, 1, 0, 0);

    private void Update()
    {
        transform.localEulerAngles = new
        (
            xEquation.Evaluate(Time.timeSinceLevelLoad),
            yEquation.Evaluate(Time.timeSinceLevelLoad),
            zEquation.Evaluate(Time.timeSinceLevelLoad)
        );
    }
}