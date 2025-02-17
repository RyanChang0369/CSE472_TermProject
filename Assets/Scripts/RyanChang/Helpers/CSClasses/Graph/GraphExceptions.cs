using System;

/// <summary>
/// The base exception relating to pathfinding.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class PathfindingException : InvalidOperationException
{
    public PathfindingException(string message) : base(message)
    {

    }

    public PathfindingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Thrown when a vertex id cannot be found in the <see
/// cref="Vertex{T}.adjacent"/> dictionary.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class VertexNotInAdjacentException : PathfindingException
{
    public VertexNotInAdjacentException(object vertex, object adjacentID)
        : base($"Vertex {adjacentID} is not adjacent to {vertex}.")
    {
    }
}

/// <summary>
/// Thrown when the vertex cannot be found inside of a graph.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class VertexNotInGraphException : PathfindingException
{
    public VertexNotInGraphException(object vertexID, object graph)
        : base($"Vertex {vertexID} cannot be found in graph {graph}.")
    {

    }

    public VertexNotInGraphException(object vertexID, object graph,
        Exception innerException)
        : base($"Vertex {vertexID} cannot be found in graph {graph}.",
            innerException)
    {

    }

    public VertexNotInGraphException(string message, object vertexID,
        object graph)
        : base(
            $"{message}: Vertex {vertexID} cannot be found in graph {graph}."
        )
    {

    }

    public VertexNotInGraphException(string message, object vertexID,
        object graph, Exception innerException)
        : base(
            $"{message}: Vertex {vertexID} cannot be found in graph {graph}.",
            innerException
        )
    {

    }
}

/// <summary>
/// Thrown when an edge between two vertices does not exist within the graph.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class EdgeNotInGraphException : PathfindingException
{
    public EdgeNotInGraphException(object fromID, object toID, object graphValue)
        : base($"Edge from {fromID} to {toID} cannot be found in graph {graphValue}.")
    {

    }

    public EdgeNotInGraphException(object fromID, object toID, object graphValue,
        Exception innerException)
        : base($"Edge from {fromID} to {toID} cannot be found in graph {graphValue}.",
            innerException)
    {

    }
}

/// <summary>
/// Thrown when the graph detects that it is disjoint, that is, it would be
/// impossible to travel from vertex A to vertex B.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class DisjointGraphException : PathfindingException
{
    public DisjointGraphException(object vertexA, object vertexB)
        : base(
            "Graph is disjoint. Trying to navigate from " +
            $"{vertexA} to {vertexB}."
        ) { }

    public DisjointGraphException(object vertexA, object vertexB,
        Exception innerException)
        : base(
            "Graph is disjoint. Trying to navigate from " +
            $"{vertexA} to {vertexB}.",
            innerException
        ) { }

    public DisjointGraphException(string message, object vertexA, object vertexB)
        : base(
            $"{message}: Graph is disjoint. Trying to navigate from " +
            $"{vertexA} to {vertexB}."
        ) { }

    public DisjointGraphException(string message, object vertexA, object vertexB,
        Exception innerException)
        : base(
            $"{message}: Graph is disjoint. Trying to navigate from " +
            $"{vertexA} to {vertexB}.",
            innerException
        ) { }
}

/// <summary>
/// Thrown when the graph runs out of vertices to iterate over.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class RanOutOfVerticesException : PathfindingException
{
    public RanOutOfVerticesException(object stuck, object end) :
        base(
            "Ran out of vertices during iteration. " +
            $"Iteration stuck at {stuck}. Cannot get to {end}. " +
            "Is graph disjoint?"
        ) {}
}

/// <summary>
/// Throw if both start and end vertices are the same.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public class StartIsEndVertexException : PathfindingException
{
    public StartIsEndVertexException(object startAndEnd) :
    base(
        $"Start and end vertices are both {startAndEnd}."
    ) {}

    public StartIsEndVertexException(string message, object startAndEnd) :
    base(
        $"{message} : Start and end vertices are both {startAndEnd}."
    ) {}

    public StartIsEndVertexException(object startAndEnd, Exception innerException) :
    base(
        $"Start and end vertices are both {startAndEnd}.",
        innerException
    ) {}
    
    public StartIsEndVertexException(string message, object startAndEnd, Exception innerException) :
    base(
        $"{message} : Start and end vertices are both {startAndEnd}.",
        innerException
    ) {}
}

/// <summary>
/// Thrown when a vertex GUID has not be initialized from <see
/// cref="Guid.Empty"/>.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
public class GuidUnsetVertexException : PathfindingException
{
    public GuidUnsetVertexException(string message, object vertex) :
        base($"{message}: GUID of {vertex} has not been initialized.") {}

    public GuidUnsetVertexException(object vertex) :
        base($"GUID of {vertex} has not been initialized.") {}

    public GuidUnsetVertexException(string message, object vertex,
        Exception innerException) :
        base(
            $"{message}: GUID of {vertex} has not been initialized.",
            innerException
        ) {}

    public GuidUnsetVertexException(object vertex, Exception innerException) :
        base(
            $"GUID of {vertex} has not been initialized.",
            innerException
        ) {}
}