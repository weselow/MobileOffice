using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Device
{
    [XmlRoot(ElementName = "response")]
    public class ControlResponse
    {
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }
}
