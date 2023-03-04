// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    public class DataGridViewRowsRemovedEventArgs : EventArgs
    {
        public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
            ArgumentOutOfRangeException.ThrowIfNegative(rowCount);

            RowIndex = rowIndex;
            RowCount = rowCount;
        }

        public int RowIndex { get; }

        public int RowCount { get; }
    }
}
