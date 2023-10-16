﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal partial class DesignBindingValueUIHandler
{
    private class LocalUIItem : PropertyValueUIItem
    {
        internal LocalUIItem(DesignBindingValueUIHandler handler, Binding binding)
            : base(handler.DataBitmap, new PropertyValueUIItemInvokeHandler(OnPropertyValueUIItemInvoke), GetToolTip(binding))
        {
            Binding = binding;
        }

        internal Binding Binding { get; }

        private static string GetToolTip(Binding binding)
        {
            string? name = "";
            if (binding.DataSource is IComponent { Site: { } site })
            {
                name = site.Name;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "(List)";
            }

            return $"{name} - {binding.BindingMemberInfo.BindingMember}";
        }
    }
}
