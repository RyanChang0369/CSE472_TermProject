using System;

public struct GraphEdge<T> where T : IEquatable<T>
{
    public static readonly GraphEdge<T> InvalidEdge = new(null, null, float.NaN);

    public float weight;

    public Vertex<T> from;

    public Vertex<T> to;

    public bool IsValid => float.IsFinite(weight);

    public GraphEdge(Vertex<T> from, Vertex<T> to, float weight) : this()
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }

    public override string ToString()
    {
        return $"[({from}, {to}), weight:{weight}]";
    }
}