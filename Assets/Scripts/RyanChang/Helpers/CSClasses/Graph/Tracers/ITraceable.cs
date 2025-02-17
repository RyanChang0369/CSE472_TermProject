using System;
using System.Collections.Generic;

public interface ITraceable<T> where T : IEquatable<T>
{
    public IEnumerator<GraphEdge<T>> GetTraces();
}