﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class ToolStripPanel
    {
        internal partial class ToolStripPanelControlCollection : TypedControlCollection
        {
            // sort by Y, then X
            public class YXComparer : IComparer<IArrangedElement>
            {
                public YXComparer() { }

                public int Compare(IArrangedElement? first, IArrangedElement? second)
                {
                    Control? one = first as Control;
                    Control? two = second as Control;

                    if (IComparerHelpers.CompareReturnIfNull(one, two, out int returnValue))
                    {
                        return returnValue;
                    }

                    if (one.Bounds.Y < two.Bounds.Y)
                    {
                        return -1;
                    }

                    if (one.Bounds.Y == two.Bounds.Y)
                    {
                        return one.Bounds.X < two.Bounds.X ? -1 : 1;
                    }

                    return 1;
                }
            }
        }
    }
}
