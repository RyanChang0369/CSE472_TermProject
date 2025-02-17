using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Vertex used in graph. Weights are stored in the graph.
/// </summary>
/// <typeparam name="T">Any IEquatable type.</typeparam>
[Serializable]
public class Vertex<T> : ISerializationCallbackReceiver, IEquatable<Vertex<T>> where T : IEquatable<T>
{
    #region Fields
    /// <summary>
    /// This is unique for each vertex and serves as its id. 
    /// </summary>
    public T id;

    [SerializeField]
    private List<IPathNode> nodes;

    /// <summary>
    /// Dictionary of outgoing edges. <id, graph weight>.
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private UnityDictionary<T, float> adjacent = new();

    /// <summary>
    /// Used in the search algorithms. Key is the ID of the thing using the
    /// vertex, bool is if it has been visited or not.
    /// </summary>
    /// <remarks>
    /// A concurrent dictionary can be access from multiple threads.
    /// </remarks>
    private ConcurrentDictionary<Guid, bool> visited = new();

    /// <summary>
    /// Used in search algorithms. Key is the ID of the thing using the vertex.
    /// Value is the total cost of the iteration to get to this point.
    /// </summary>
    /// <inheritdoc cref="visited"/>
    private ConcurrentDictionary<Guid, float> aggregateCosts = new();

    /// <summary>
    /// Heuristic for A* search. Smaller values means A* will most likely take
    /// that path.
    /// </summary>
    [SerializeField]
    public float heuristic;

    /// <summary>
    /// What section of the graph is this vertex in?
    /// </summary>
    /// <remarks>
    /// A section is defined as the exhaustive set of vertices of which all are
    /// connected. All vertices that are connected are of the same section.
    /// </remarks>
    public Guid sectionID;

    [SerializeField]
    [ReadOnly]
    private string sectionIDSerialized;
    #endregion

    #region Properties
    /// <summary>
    /// Alias for <see cref="id"/>.
    /// </summary>
    public T Value => id;

    /// <summary>
    /// Degree of this vertex (number of outgoing edges).
    /// </summary>
    public int Degree => Adjacent.Count;

    public List<IPathNode> Nodes
    {
        get
        {
            nodes ??= new();
            return nodes;
        }
        set
        {
            nodes ??= new();
            nodes = value;
        }
    }

    public UnityDictionary<T, float> Adjacent
    {
        get
        {
            lock (adjacent)
            {
                adjacent ??= new();
                return adjacent; 
            }
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a vertex.
    /// </summary>
    /// <param name="id">ID of this vertex.</param>
    public Vertex(T id)
    {
        this.id = id;
        heuristic = 0;
    }

    /// <summary>
    /// Creates a vertex with a custom heuristic.
    /// </summary>
    /// <param name="id">ID of this vertex.</param>
    /// <param name="heuristic">The heuristic.</param>
    public Vertex(T id, float heuristic)
    {
        this.id = id;
        this.heuristic = heuristic;
    }
    #endregion

    #region Adjacent
    /// <summary>
    /// Gets an IEnumerable whose members are the adjacent vertices that are
    /// specified in <see cref="Adjacent"/>.
    /// </summary>
    /// <param name="graph">The graph this vertex belongs to.</param>
    /// <returns></returns>
    public IEnumerable<Vertex<T>> AdjacentVertices(Graph<T> graph)
    {
        foreach (var adjID in Adjacent.Keys)
        {
            yield return graph.GetVertex(adjID);
        }
    }

    /// <summary>
    /// Gets an IEnumerable whose members are the adjacent edges that are
    /// specified in <see cref="Adjacent"/>.
    /// </summary>
    /// <param name="graph">The graph this vertex belongs to.</param>
    /// <returns></returns>
    public IEnumerable<GraphEdge<T>> AdjacentEdges(Graph<T> graph)
    {
        foreach (var adjKVP in Adjacent)
        {
            yield return new(this, graph.GetVertex(adjKVP.Key), adjKVP.Value);
        }
    }
    #endregion

    #region Visited
    /// <summary>
    /// Uses the id to check if this vertex is visited or not.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    /// <returns>True if vertex has been visited by the thing with ID, else
    /// false.</returns>
    public bool GetVisited(Guid id)
    {
        if (!visited.TryGetValue(id, out bool output))
            return false;

        return output;
    }

    /// <summary>
    /// Uses the id to set this vertex's visited status.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    /// <param name="visited">What to set visited to.</param>
    public void SetVisited(Guid id, bool visited)
    {
        this.visited[id] = visited;
    }

    /// <summary>
    /// Removes the id from the vertex's visited.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    public void ResetVisited(Guid id)
    {
        visited.Remove(id, out _);
    }
    #endregion

    #region Aggregate Cost
    /// <summary>
    /// Uses ID to get the aggregate cost of a vertex during traversal.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    /// <returns></returns>
    public float GetAggregateCost(Guid id)
    {
        if (!aggregateCosts.TryGetValue(id, out float cost))
            return 0;

        return cost;
    }

    /// <summary>
    /// Uses the id to set this vertex's aggregate cost.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    /// <param name="visited">What to set visited to.</param>
    public void SetAggregateCost(Guid id, float cost)
    {
        aggregateCosts[id] = cost;
    }

    /// <summary>
    /// Removes the id from the vertex's aggregate costs.
    /// </summary>
    /// <param name="id">ID used for lookup.</param>
    public void ResetAggregateCost(Guid id)
    {
        aggregateCosts.Remove(id, out _);
    }
    #endregion

    #region Generic Overrides
    public override string ToString()
    {
        return $"{id} [{sectionID}]";
    }
    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
    #endregion

    #region ISerializationCallbackReceiver Implementation
    public void OnBeforeSerialize()
    {
        sectionIDSerialized = sectionID.ToString();
    }

    public void OnAfterDeserialize()
    {
        sectionID = Guid.Parse(sectionIDSerialized);
        visited ??= new();
        aggregateCosts ??= new();
    }
    #endregion

    #region IEquatable Implementation
    public bool Equals(Vertex<T> other)
    {
        return this.id.Equals(other.id);
    }

    public override bool Equals(object obj)
    {
        if (obj is Vertex<T> vertex)
        {
            return Equals(vertex);
        }
        else if (obj is T tea)
        {
            return this.id.Equals(tea);
        }
        
        return base.Equals(obj);
    }
    #endregion
}