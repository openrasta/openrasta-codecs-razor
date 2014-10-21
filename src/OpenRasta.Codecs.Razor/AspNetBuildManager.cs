using System;
using System.Web.Compilation;

namespace OpenRasta.Codecs.Razor
{
    public class AspNetBuildManager : IBuildManager
    {
        public Type GetCompiledType(string path)
        {
            return BuildManager.GetCompiledType(path);
        }
    }
}