namespace OpenRasta.Codecs.Razor
{
    public interface IViewProvider
    {
        ViewDefinition GetViewDefinition(string path);
    }
}