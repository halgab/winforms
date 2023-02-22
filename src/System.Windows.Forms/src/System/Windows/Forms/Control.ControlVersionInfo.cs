// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private class ControlVersionInfo
        {
            private string? _companyName;
            private string? _productName;
            private string? _productVersion;
            private FileVersionInfo? _versionInfo;
            private readonly Control _owner;

            internal ControlVersionInfo(Control owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  The company name associated with the component.
            /// </summary>
            [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
            internal string CompanyName
            {
                get
                {
                    if (_companyName is null)
                    {
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                        }

                        if (string.IsNullOrEmpty(_companyName) && !OwnerIsInMemoryAssembly)
                        {
                            _companyName = GetFileVersionInfo().CompanyName?.Trim();
                        }

                        if (string.IsNullOrEmpty(_companyName))
                        {
                            string ns = _owner.GetType().Namespace ?? "";

                            int firstDot = ns.IndexOf('/');
                            if (firstDot != -1)
                            {
                                _companyName = ns.Substring(0, firstDot);
                            }
                            else
                            {
                                _companyName = ns;
                            }
                        }
                    }

                    return _companyName;
                }
            }

            /// <summary>
            ///  The product name associated with this component.
            /// </summary>
            [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
            internal string ProductName
            {
                get
                {
                    if (_productName is null)
                    {
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _productName = ((AssemblyProductAttribute)attrs[0]).Product;
                        }

                        if (string.IsNullOrEmpty(_productName) && !OwnerIsInMemoryAssembly)
                        {
                            _productName = GetFileVersionInfo().ProductName?.Trim();
                        }

                        if (string.IsNullOrEmpty(_productName))
                        {
                            string? ns = _owner.GetType().Namespace;

                            ns ??= string.Empty;

                            int firstDot = ns.IndexOf('.');
                            if (firstDot != -1)
                            {
                                _productName = ns.Substring(firstDot + 1);
                            }
                            else
                            {
                                _productName = ns;
                            }
                        }
                    }

                    return _productName;
                }
            }

            /// <summary>
            ///  The product version associated with this component.
            /// </summary>
            [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
            internal string ProductVersion
            {
                get
                {
                    if (_productVersion is null)
                    {
                        // custom attribute
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                        }

                        // win32 version info
                        if (string.IsNullOrEmpty(_productVersion) && !OwnerIsInMemoryAssembly)
                        {
                            _productVersion = GetFileVersionInfo().ProductVersion?.Trim();
                        }

                        // fake it
                        if (string.IsNullOrEmpty(_productVersion))
                        {
                            _productVersion = "1.0.0.0";
                        }
                    }

                    return _productVersion;
                }
            }

            /// <summary>
            ///  Retrieves the FileVersionInfo associated with the main module for
            ///  the component.
            /// </summary>
            [RequiresAssemblyFiles("Throws if " + nameof(_owner) + " is an in-memory assembly. Check " + nameof(OwnerIsInMemoryAssembly) + " first")]
            private FileVersionInfo GetFileVersionInfo()
            {
                if (_versionInfo is null)
                {
                    string path = _owner.GetType().Module.FullyQualifiedName;

                    _versionInfo = FileVersionInfo.GetVersionInfo(path);
                }

                return _versionInfo;
            }

            private bool OwnerIsInMemoryAssembly => _owner.GetType().Assembly.Location.Length == 0;
        }
    }
}
