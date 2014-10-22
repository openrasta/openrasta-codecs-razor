using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;

using OpenRasta.Collections.Specialized;
using OpenRasta.DI;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenRasta.Codecs.Razor
{
    [MediaType("application/xhtml+xml;q=0.9", "xhtml")]
    [MediaType("text/html", "html")]
    [MediaType("application/vnd.openrasta.htmlfragment+xml;q=0.5")]
    [SupportedType(typeof(RazorViewBase))]
    public class RazorCodec : IMediaTypeWriter
    {
        private static readonly string[] DEFAULT_VIEW_NAMES = { "index", "default", "view", "get" };
        private readonly IBuildManager _buildManager;
        private readonly IRequest _request;
        private IDictionary<string, string> _configuration;

        public RazorCodec(IRequest request)
        {
            _request = request;
            _buildManager = CreateBuildManager();
        }

        public object Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (value != null)
                {
                    _configuration = value.ToCaseInvariantDictionary();
                }
            }
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            // The default webforms renderer only associate the last parameter in the codecParameters
            // with a page that has been defined in the rendererParameters.

            var codecParameterList = new List<string>(codecParameters);
            if (!string.IsNullOrEmpty(_request.UriName))
            {
                codecParameterList.Add(_request.UriName);
            }

            string templateAddress = GetViewVPath(_configuration, codecParameterList.ToArray(), _request.UriName);

            Type type = _buildManager.GetCompiledType(templateAddress);

            var renderTarget = DependencyManager.GetService(type) as RazorViewBase;

            if (renderTarget == null)
            {
                throw new InvalidOperationException("View page doesn't inherit from RazorViewBase");
            }

            renderTarget.SetResource(entity);
            renderTarget.Errors = response.Errors;
            RenderTarget(response, renderTarget);
        }

        private static IBuildManager CreateBuildManager()
        {
            if (HostingEnvironment.IsHosted)
            {
                return new AspNetBuildManager();
            }
            return new StandAloneBuildManager(DependencyManager.GetService<IViewProvider>());
        }

        public static string GetViewVPath(IDictionary<string, string> codecConfiguration, string[] codecUriParameters, string uriName)
        {
            // if no pages were defined, return 501 not implemented
            if (codecConfiguration == null || codecConfiguration.Count == 0)
            {
                return null;
            }

            // if no codec parameters in the uri, take the default or return null
            if (codecUriParameters == null || codecUriParameters.Length == 0)
            {
                if (uriName != null && codecConfiguration.ContainsKey(uriName))
                {
                    return codecConfiguration[uriName];
                }
                return GetDefaultVPath(codecConfiguration);
            }

            // if there's a codec parameter, take the first one and try to return the view if it exists
            string requestParameter = codecUriParameters[codecUriParameters.Length - 1];
            if (codecConfiguration.Keys.Contains(requestParameter))
            {
                return codecConfiguration[requestParameter];
            }

            // if theres a codec parameter and a uri name that doesn't match it, return teh default
            if (!uriName.IsNullOrEmpty())
            {
                return GetDefaultVPath(codecConfiguration);
            }
            return null;
        }

        private static string GetDefaultVPath(IDictionary<string, string> codecConfiguration)
        {
            foreach (string defaultViewName in DEFAULT_VIEW_NAMES)
            {
                if (codecConfiguration.Keys.Contains(defaultViewName))
                {
                    return codecConfiguration[defaultViewName];
                }
            }
            return null;
        }

        private static void RenderTarget(IHttpEntity response, RazorViewBase target)
        {
            Encoding targetEncoding = Encoding.UTF8;
            response.ContentType.CharSet = targetEncoding.HeaderName;
            TextWriter writer = null;
            var isDisposable = target as IDisposable;
            bool ownsWriter = false;
            try
            {
                if (response is ISupportsTextWriter)
                {
                    writer = ((ISupportsTextWriter)response).TextWriter;
                }
                else
                {
                    writer = new DeterministicStreamWriter(response.Stream, targetEncoding, StreamActionOnDispose.None);
                    ownsWriter = true;
                }

                target.Output = writer;
                target.Execute();
            }
            finally
            {
                if (isDisposable != null)
                {
                    isDisposable.Dispose();
                }
                if (ownsWriter)
                {
                    writer.Dispose();
                }
            }
        }
    }
}