﻿// #pragma once
// #ifndef IEnumerableExtensions.cs_H_
// #define IEnumerableExtensions.cs_H_
// 
// #endif

using System;
using System.Collections.Generic;
using System.Linq;

namespace NebulaAPI.Extensions;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (action is null) throw new ArgumentNullException(nameof(action));

        foreach (var item in source)
        {
            action(item);
        }
    }
}
