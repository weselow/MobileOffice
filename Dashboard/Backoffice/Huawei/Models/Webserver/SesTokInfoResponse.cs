using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Webserver
{
    [XmlRoot(ElementName = "response")]
    public class SesTokInfoResponse
    {
        [XmlElement(ElementName = "SesInfo")]
        public string SesInfo { get; set; } = string.Empty;
        [XmlElement(ElementName = "TokInfo")]
        public string TokInfo { get; set; } = string.Empty;
    }
}

/*
 <?xml version="1.0" encoding="UTF-8"?>
<response>
<SesInfo>SessionID=O2rql6FRT6XkaEua0IJtmKx0DuYsQ6WWn9wmq4TswZrvPkUo4PMu7+p3hvlI/Y2BEax7POIkkHZSCtjlM7QvCVi4POYPnjBlvwNN6ejs55VW9uYzx8BrbC5rgB9vYlNH</SesInfo>
<TokInfo>roldNtk28Pj6qkRw/gnUFzSBg1Y0Lv60</TokInfo>
</response>
 */