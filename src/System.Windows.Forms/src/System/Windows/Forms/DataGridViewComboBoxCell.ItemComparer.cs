﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxCell : DataGridViewCell
    {
        private sealed class ItemComparer : IComparer<object?>
        {
            private readonly DataGridViewComboBoxCell dataGridViewComboBoxCell;

            public ItemComparer(DataGridViewComboBoxCell dataGridViewComboBoxCell)
            {
                this.dataGridViewComboBoxCell = dataGridViewComboBoxCell;
            }

            public int Compare(object? item1, object? item2)
            {
                if (IComparerHelpers.CompareReturnIfNull(item1, item2, out int returnValue))
                {
                    return returnValue;
                }

                string itemName1 = dataGridViewComboBoxCell.GetItemDisplayText(item1);
                string itemName2 = dataGridViewComboBoxCell.GetItemDisplayText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}
