// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal class ContextMenuStripGroupCollection
{
    private readonly Dictionary<string, ContextMenuStripGroup> _innerDictionary = new();

    public ContextMenuStripGroupCollection()
    {
    }

    public ContextMenuStripGroup this[string key]
    {
        get
        {
            if (!_innerDictionary.TryGetValue(key, out ContextMenuStripGroup? group))
            {
                group = _innerDictionary[key] = new ContextMenuStripGroup(key);
            }

            return group;
        }
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out ContextMenuStripGroup? value)
    {
        return _innerDictionary.TryGetValue(key, out value);
    }
}
