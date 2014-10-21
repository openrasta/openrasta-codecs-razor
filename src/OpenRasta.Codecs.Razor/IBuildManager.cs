using System;
using System.IO;

namespace OpenRasta.Codecs.Razor
{
    public interface IBuildManager
    {
        Type GetCompiledType(string path);
    }
}