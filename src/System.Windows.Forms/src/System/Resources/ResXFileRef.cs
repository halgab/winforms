// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace System.Resources;

/// <summary>
///  ResX File Reference class. This allows the developer to represent
///  a link to an external resource. When the resource manager asks
///  for the value of the resource item, the external resource is loaded.
/// </summary>
[TypeConverter(typeof(Converter))]
public partial class ResXFileRef
{
    /// <summary>
    ///  Creates a new ResXFileRef that points to the specified file.
    ///  The type refered to by typeName must support a constructor
    ///  that accepts a System.IO.Stream as a parameter.
    /// </summary>
    public ResXFileRef(string fileName, string typeName)
    {
        FileName = fileName.OrThrowIfNull();
        TypeName = typeName.OrThrowIfNull();
    }

    /// <summary>
    ///  Creates a new ResXFileRef that points to the specified file.
    ///  The type refered to by typeName must support a constructor
    ///  that accepts a System.IO.Stream as a parameter.
    /// </summary>
    public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding) : this(fileName, typeName)
    {
        TextFileEncoding = textFileEncoding;
    }

    internal ResXFileRef Clone()
        => TextFileEncoding is null ? new(FileName, TypeName) : new(FileName, TypeName, TextFileEncoding);

    public string FileName { get; private set; }

    public string TypeName { get; }

    public Encoding? TextFileEncoding { get; }

    /// <summary>
    ///  path1+result = path2
    ///  A string which is the relative path difference between path1 and
    ///  path2 such that if path1 and the calculated difference are used
    ///  as arguments to Combine(), path2 is returned
    /// </summary>
    private static string PathDifference(ReadOnlySpan<char> path1, string path2, bool compareCase)
        {
            int i;

        if (compareCase)
            {
                i = path1.CommonPrefixLength(path2);
            }
            else
            {
                for (i = 0; (i < path1.Length) && (i < path2.Length); ++i)
                {
                    if (path1[i] != path2[i]
                        && (char.ToLower(path1[i], CultureInfo.InvariantCulture) != char.ToLower(path2[i], CultureInfo.InvariantCulture)))
                    {
                        break;
                    }
                }
            }

        if (i == 0)
        {
            return path2;
        }

        int si = path1.Slice(0, i).LastIndexOf(Path.DirectorySeparatorChar);

            if ((i == path1.Length) && (i == path2.Length))
            {
                return string.Empty;
            }

        StringBuilder relPath = new StringBuilder();
            int slashCount = path1.Slice(i).Count(Path.DirectorySeparatorChar);

        for (int j = 0; j < slashCount; j++)
            {
                relPath.Append($"..{Path.DirectorySeparatorChar}");
            }

        relPath.Append(path2.AsSpan(si + 1));

        return relPath.ToString();
    }

    internal void MakeFilePathRelative(string basePath)
    {
        if (string.IsNullOrEmpty(basePath))
        {
            return;
        }

        FileName = PathDifference(basePath, FileName, false);
    }

    public override string ToString()
    {
        string result;

        if (FileName.Contains(';') || FileName.Contains('\"'))
        {
            result = $"\"{FileName}\";{TypeName}";
        }
        else
        {
            result = $"{FileName};{TypeName}";
        }

        if (TextFileEncoding is not null)
        {
            result += $";{TextFileEncoding.WebName}";
        }

        return result;
    }
}
