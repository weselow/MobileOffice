/*
setTimeout(() => {
    document.location.reload();
}, 3000);
*/

// ������� ��� ������ ���������� ���������� �������������
function updatePartialView() {
    // ����� �� ������ ��������� AJAX-������ ��� ��������� ������������ ���������� �������������
    // � �������� ��� � ��������������� ������� �� ��������
    // ������:
    $.ajax({
        url: '/Home/GetData', // �������� 'Controller' � 'Action' �� ���� ��������
        type: 'GET',
        success: function (data) {
            $('#partialViewContainer').html(data); // �������� 'partialViewContainer' �� ID ��������, � ������� ������ ������������ ��������� �������������
        },
        error: function (xhr, status, error) {
            // ��������� ������ ��� ���������� �������
        }
    });
}

// ����� ������� ���������� ������ 5 ������
setInterval(updatePartialView, 5000);

