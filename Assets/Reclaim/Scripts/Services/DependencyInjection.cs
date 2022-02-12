﻿using System;
using System.Collections.Generic;

public static class DependencyInjection
{
    public static IDictionary<Type, object> _instanceMap = new Dictionary<Type, object>();

    public static void Clear()
    {
        _instanceMap.Clear();
    }

    public static void Register<T>(T instance)
    {
        var type = typeof(T);
        if (_instanceMap.ContainsKey(type))
            throw new InvalidOperationException("An instance of that type already exists: " + type);

        _instanceMap[type] = instance;
    }

    public static T GetInstance<T>()
    {
        return (T)_instanceMap[typeof(T)];
    }
}
