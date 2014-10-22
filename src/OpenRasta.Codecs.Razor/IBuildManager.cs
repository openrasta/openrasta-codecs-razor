using System;

namespace OpenRasta.Codecs.Razor
{
    public interface IBuildManager
    {
        Type GetCompiledType(string path);
    }
}