using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Net
{
    [XmlRoot(ElementName = "response")]
    public class NetModeResponse
    {
        [XmlText] public string Text { get; set; } = string.Empty;
    }
}
