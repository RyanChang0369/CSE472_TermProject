using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Helper class to build valid CSV.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public class CSVBuilder
{
    #region Variables
    /// <summary>
    /// Consists of the header (the keys) and the data (the values).
    /// </summary>
    public Dictionary<string, List<string>> internalData = new();
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds the header and data to the csv.
    /// </summary>
    /// <param name="header">The header of the data.</param>
    /// <param name="data">The data.</param>
    public void Add(string header, string data)
    {
        internalData.AddToDictList(header, data);
    }

    /// <summary>
    /// Adds the header and data to the csv, at some specified row index.
    /// </summary>
    /// <param name="header">The header of the data.</param>
    /// <param name="data">The data.</param>
    /// <param name="index">The row which to add the data, creating a new row if
    /// necessary.</param>
    public void AddAt(string header, string data, int index)
    {
        List<string> list = internalData.GetValueOrNew(header);
        list.AddOrReplaceWithBuffer(data, index);
    }

    // public override string ToString()
    // {
    //     return ToString("");
    // }

    public string ToString(string headerPrefix)
    {
        // This first chunk of code writes the body of the CSV by transposing
        // the rows of internalData into columns and vice versa.
        StringBuilder sb = new();

        // Find the longest length of all the data values in the internalData.
        int longestLength = internalData.Values.Max(l => l.Count);

        // The keys of the dict.
        var keys = internalData.Keys;

        for (int i = 0; i < longestLength; i++)
        {
            try
            {
                sb.AppendLine(String.Join(", ",
                    keys.Select(k => internalData[k][i])));
            }
            catch (System.ArgumentOutOfRangeException)
            {
                // Do nothing.
            }
        }

        var headers = keys.Select(k => $"{headerPrefix}.{k}");
        string headerStr = string.Join(", ", headers);

        return $"{headerStr}\r\n{sb}";
    }
    #endregion
}