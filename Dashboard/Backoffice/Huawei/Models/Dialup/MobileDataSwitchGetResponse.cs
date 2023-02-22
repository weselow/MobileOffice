using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Dialup
{
    /// <summary>
    /// Получение сведений о подключении.
    /// </summary>
    [XmlRoot(ElementName = "response")]
    public class MobileDataSwitchGetResponse
    {
        [XmlElement(ElementName = "dataswitch")]
        public string Dataswitch { get; set; } = string.Empty;
    }
}

/*
     Получение сведений о подключении
URL: http://192.168.8.1/api/dialup/mobile-dataswitch
Метод: GET
Запрос: Нет
ответ:
Код:
<response>
   <dataswitch></dataswitch>
</response>
Комментарии:
Переключатель данных:
0 - Мобильные данные отключены
1 - Мобильные данные включены
     
 */
