using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Device
{
    [XmlRoot(ElementName = "request")]
    public class ControlRequest
    {
        [XmlElement(ElementName = "Control")]
        public string Control { get; set; } = ControlData.Reboot;
    }
}

/*
Перезагрузка модема, восстановление конфигурации, резервное копирование конфигурации, завершение работы устройства
URL: http://192.168.8.1/api/device/control
Метод: POST
требование:
Код:
<request>
    <Control></Control>
</request>
ответ:
Код:
<response>OK</response>
Комментарии:
Контроль:
1 - Перезагрузка устройства
2 - Восстановление конфигурации (нет информации о том, как указать файл с сохраненной конфигурацией. Возможно, используется файл http://192.168.8.1/nvram.bak)
3 - Конфигурация резервного копирования (конфигурация доступна по адресу http://192.168.8.1/nvram.bak. Файл в кодировке base64)
4 - Выключение устройства
     */
