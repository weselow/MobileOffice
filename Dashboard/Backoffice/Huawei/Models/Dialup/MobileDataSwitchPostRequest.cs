using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Dialup
{
    /// <summary>
    /// Мобильная коммутация данных
    /// </summary>
    [XmlRoot(ElementName = "request")]
    public class MobileDataSwitchPostRequest
    {
        [XmlElement(ElementName = "dataswitch")]
        public int Dataswitch { get; set; }
    }
}

/*
     Мобильная коммутация данных
URL: http://192.168.8.1/api/dialup/mobile-dataswitch
Метод: POST
требование:
Код:
<request>
    <dataswitch></dataswitch>
</request>
ответ:
Код:
<response>OK</response>
Комментарии:
Переключатель данных:
0 - Мобильные данные отключены
1 - Мобильные данные включены
     */
