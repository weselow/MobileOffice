/*
setTimeout(() => {
    document.location.reload();
}, 3000);
*/

// Функция для вызова обновления частичного представления
function updatePartialView() {
    // Здесь вы можете выполнить AJAX-запрос для получения обновленного частичного представления
    // и вставить его в соответствующий элемент на странице
    // Пример:
    $.ajax({
        url: '/Home/GetData', // Замените 'Controller' и 'Action' на свои значения
        type: 'GET',
        success: function (data) {
            $('#partialViewContainer').html(data); // Замените 'partialViewContainer' на ID элемента, в котором должно отображаться частичное представление
        },
        error: function (xhr, status, error) {
            // Обработка ошибок при выполнении запроса
        }
    });
}

// Вызов функции обновления каждые 5 секунд
setInterval(updatePartialView, 5000);

