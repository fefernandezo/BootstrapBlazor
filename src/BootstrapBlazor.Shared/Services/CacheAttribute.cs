// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Metalama.Framework.Aspects;
using System.Collections.Concurrent;

namespace BootstrapBlazor.Shared;

/// <summary>
/// Cache AOP 标签
/// </summary>
public class CacheAttribute : OverrideMethodAspect
{
    /// <summary>
    /// 获得/设置 缓存键值
    /// </summary>
    [NotNull]
    public string? Key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override dynamic OverrideMethod()
    {
        if (SampleCache.Cache.TryGetValue(Key, out var value))
        {
            return value;
        }
        else
        {
            var result = meta.Proceed();

            // Add to cache.
            SampleCache.Cache.TryAdd(Key, result);

            return result;
        }
    }
}

// Placeholder implementation of a cache because the hosted try.metalama.net does not allow for MemoryCache.
internal static class SampleCache
{
    public static readonly ConcurrentDictionary<string, object> Cache = new();
}
