﻿using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;
using System;
using System.Linq;

namespace Gordon360.Extensions;

public static class SystemTextJsonMergeExtensions
{
    /// <summary>
    /// Merges the specified Json Node into the base JsonNode for which this method is called.
    /// It is null safe and can be easily used with null-check & null coalesce operators for fluent calls.
    /// NOTE: JsonNodes are context aware and track their parent relationships therefore to merge the values both JsonNode objects
    ///         specified are mutated. The Base is mutated with new data while the source is mutated to remove references to all
    ///         fields so that they can be added to the base.
    ///
    /// Source taken directly from the open-source Gist here:
    /// https://gist.github.com/cajuncoding/bf78bdcf790782090d231590cbc2438f
    ///
    /// </summary>
    /// <param name="jsonBase"></param>
    /// <param name="jsonMerge"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static JsonNode Merge(this JsonNode jsonBase, JsonNode jsonMerge)
    {
        if (jsonBase == null || jsonMerge == null)
            return jsonBase;

        switch (jsonBase)
        {
            case JsonObject jsonBaseObj when jsonMerge is JsonObject jsonMergeObj:
                {
                    //NOTE: We must materialize the set (e.g. to an Array), and then clear the merge array so the node can then be 
                    //      re-assigned to the target/base Json; clearing the Object seems to be the most efficient approach...
                    var mergeNodesArray = jsonMergeObj.ToArray();
                    jsonMergeObj.Clear();

                    foreach (var prop in mergeNodesArray)
                    {
                        jsonBaseObj[prop.Key] = jsonBaseObj[prop.Key] switch
                        {
                            JsonObject jsonBaseChildObj when prop.Value is JsonObject jsonMergeChildObj => jsonBaseChildObj.Merge(jsonMergeChildObj),
                            JsonArray jsonBaseChildArray when prop.Value is JsonArray jsonMergeChildArray => jsonBaseChildArray.Merge(jsonMergeChildArray),
                            JsonValue jsonBaseValue when prop.Value is null => jsonBaseValue,
                            _ => prop.Value
                        };
                    }
                    break;
                }
            case JsonArray jsonBaseArray when jsonMerge is JsonArray jsonMergeArray:
                {
                    //NOTE: We must materialize the set (e.g. to an Array), and then clear the merge array,
                    //      so they can then be re-assigned to the target/base Json...
                    var mergeNodesArray = jsonMergeArray.ToArray();
                    jsonMergeArray.Clear();
                    foreach (var mergeNode in mergeNodesArray) jsonBaseArray.Add(mergeNode);
                    break;
                }
            default:
                throw new ArgumentException($"The JsonNode type [{jsonBase.GetType().Name}] is incompatible for merging with the target/base " +
                                                    $"type [{jsonMerge.GetType().Name}]; merge requires the types to be the same.");

        }

        return jsonBase;
    }
}