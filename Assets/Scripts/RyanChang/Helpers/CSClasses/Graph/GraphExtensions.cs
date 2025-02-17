using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public static class GraphExtensions
{
    /// <summary>
    /// Use <paramref name="renderers"/> to draw a visualization of this graph.
    /// </summary>
    /// <param name="lr">The line renderer.</param>
    /// <param name="graph">The graph to trace</param>
    /// <param name="containerName">
    /// The name of the container to place the traces in.
    /// </param>
    public static void Trace(this Graph<Vector3> graph, LineRenderer lr,
        string containerName = "TraceContainer")
    {
        if (graph.Count < 2) return;

        // Breadth first traversal
        Guid g = Guid.NewGuid();
        Queue<Vertex<Vector3>> q = new Queue<Vertex<Vector3>>();
        q.Enqueue(graph.Vertices.Values.First());
        GameObject traceContainer = new(containerName);
        
        while (q.Count > 0)
        {
            Vertex<Vector3> currV = q.Dequeue();

            if (currV.GetVisited(g)) continue;
            currV.SetVisited(g, true);

            foreach (var adjP in currV.Adjacent)
            {
                Vertex<Vector3> adjV = graph.Vertices[adjP.Key];

                q.Enqueue(adjV);

                // Now build the renderers
                var rendererGameObj = new GameObject("GridRenderLine");
                rendererGameObj.transform.SetParent(traceContainer.transform);
                rendererGameObj.transform.localPosition = Vector3.zero;
                var newRenderer = lr.CopyComponent(rendererGameObj);
                Vector3[] renderPositions = { currV.id, adjV.id };
                newRenderer.SetPositions(renderPositions);
                newRenderer.enabled = true;
            }
        }

        traceContainer.transform.SetParent(lr.transform);
        traceContainer.transform.localPosition = Vector3.zero;

        graph.ResetVisitedVertices(g);
    }
}