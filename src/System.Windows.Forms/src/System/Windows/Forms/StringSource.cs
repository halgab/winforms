﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

/// <summary>
///  Represents an internal class that is used by ComboBox and TextBox AutoCompleteCustomSource property.
///  This class is responsible for initializing the SHAutoComplete COM object and setting options in it.
///  The StringSource contains an array of Strings which is passed to the COM object as the custom source.
/// </summary>
internal unsafe class StringSource : IEnumString.Interface, IManagedWrapper<IEnumString>
{
    private string[] strings;
    private int current;
    private int size;
    private IAutoComplete2* _autoComplete2;

    /// <summary>
    ///  Constructor.
    /// </summary>
    public StringSource(string[] strings)
    {
        Array.Clear(strings, 0, size);
        this.strings = strings;

        current = 0;
        size = strings.Length;

        PInvokeCore.CoCreateInstance(
            in CLSID.AutoComplete,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            out _autoComplete2).ThrowOnFailure();
    }

    /// <summary>
    ///  This is the method that binds the custom source with the IAutoComplete interface.The "hWndEdit" is the handle
    ///  to the edit Control and the "options' are the options that need to be set in the AUTOCOMPLETE mode.
    /// </summary>
    public bool Bind(IHandle<HWND> edit, AUTOCOMPLETEOPTIONS options)
    {
        if (_autoComplete2 is null || _autoComplete2->SetOptions((uint)options).Failed)
        {
            return false;
        }

        _autoComplete2->Init(
            edit.Handle,
            (IUnknown*)ComHelpers.GetComPointer<IEnumString>(this),
            (PCWSTR)null,
            (PCWSTR)null);

        GC.KeepAlive(edit.Wrapper);
        return true;
    }

    public void ReleaseAutoComplete()
    {
        if (_autoComplete2 is not null)
        {
            _autoComplete2->Release();
            _autoComplete2 = null;
        }
    }

    public void RefreshList(string[] newSource)
    {
        Array.Clear(strings, 0, size);
        strings = newSource;
        current = 0;
        size = strings.Length;
    }

    public unsafe HRESULT Clone(IEnumString** ppenum)
    {
        if (ppenum is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppenum = ComHelpers.GetComPointer<IEnumString>(new StringSource(strings) { current = current });
        return HRESULT.S_OK;
    }

    public unsafe HRESULT Next(uint celt, PWSTR* rgelt, [Optional] uint* pceltFetched)
    {
        if (celt < 0)
        {
            return HRESULT.E_INVALIDARG;
        }

        uint fetched = 0;

        while (current < size && celt > 0)
        {
            rgelt[fetched] = (char*)Marshal.StringToCoTaskMemUni(strings[current]);
            current++;
            fetched++;
            celt--;
        }

        if (pceltFetched is not null)
        {
            *pceltFetched = fetched;
        }

        return celt == 0 ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    public HRESULT Skip(uint celt)
    {
        int newCurrent = current + (int)celt;
        if (newCurrent >= size)
        {
            return HRESULT.S_FALSE;
        }

        current = newCurrent;
        return HRESULT.S_OK;
    }

    public HRESULT Reset()
    {
        current = 0;
        return HRESULT.S_OK;
    }
}
