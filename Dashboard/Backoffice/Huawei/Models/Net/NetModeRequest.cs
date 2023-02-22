using System.Xml.Serialization;

namespace Dashboard.Backoffice.Huawei.Models.Net
{
    [XmlRoot(ElementName = "request")]
    public class NetModeRequest
    {
        [XmlElement(ElementName = "NetworkMode")] public string NetworkMode { get; set; } = string.Empty;
        [XmlElement(ElementName = "NetworkBand")] public string NetworkBand { get; set; } = string.Empty;
        [XmlElement(ElementName = "LTEBand")] public string LTEBand { get; set; } = string.Empty;
    }
}

/*
 * Режим работы сети (1) - настройки (мне кажется, что речь идет о настройке, на каких частотах модем может подключаться. Если у кого-то есть лучшее имя, не стесняйтесь писать Язык)
URL: http://192.168.8.1/api/net/net-mode
Метод: POST
требование:
Код:
<request>
    <NetworkMode>00</NetworkMode>
    <NetworkBand>3FFFFFFF</NetworkBand>
    <LTEBand>800C5</LTEBand>
</request>
ответ:
Код:
<response>OK</response>

Комментарии:

Сетевой режим:
00 - автоматический (все сети)
01 - Только 2G
02 - Только 3G
03 - Только 4G (LTE)
99 - без изменений
сетевой режим может быть подключен, например, '0302' эквивалентен 'LTE -> 3G предпочтительно'

Сетевая полоса:
80 - GSM1800
300 - GSM900
80000 - GSM850
200000 - GSM1900
400000 - УТМС Б1 (2100)
800000 - УТМС Б2 (1900)
4000000 - УТМС Б5 (850)
2000000000000 - УТМС Б8 (900)
1 - BC0A
2 - БК0Б
4 - BC1
8 - BC2
10 - BC3
20 - BC4
40 - BC5
400 - BC6
800 - BC7
1000 - BC8
2000 - BC9
4000 - BC10
8000 - BC11
10000000 - BC12
20000000 - BC13
80000000 - BC14
3FFFFFFF - все поддерживается
другое обозначение - шестнадцатеричная сумма вышеуказанных сетей (например, GSM900 + UTMS B8 = 0x2000000000000 + 0x300 = 0x2000000000300)

Диапазон LTE:
1 - B1 (FDD 2100)
4 - B3 (FDD 1800)
40 - B7 (FDD 2600)
80 - B8 (FDD 900)
80000 - B20 (FDD 800)
2000000000 - B38 (TDD 2600)
8000000000 - B40 (TDD 2300)
800C5 - Диапазоны ЕС (LTE Band 1, 3, 7, 8, 20)
800D5 - Диапазоны ЕС/Азии/Африки (LTE Band 1, 3, 5, 7, 8, 20)
7FFFFFFFFFFFFF - все диапазоны
Другая нотация - шестнадцатеричная сумма вышеуказанных сетей
     */
