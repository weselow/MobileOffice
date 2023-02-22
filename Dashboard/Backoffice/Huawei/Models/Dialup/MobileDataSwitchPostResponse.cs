using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Dialup
{
    [XmlRoot(ElementName = "response")]
    public class MobileDataSwitchPostResponse
    {
        [XmlText] public string Text { get; set; } = string.Empty;
    }
}
