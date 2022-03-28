namespace GDFiddle.UI.Controls
{
    public record ItemWithMetaData<TMetaData>(Control Control, TMetaData? MetaData);
}
