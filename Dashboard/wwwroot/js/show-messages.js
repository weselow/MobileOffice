// ������� ��� ���������� ��������� � ����������� toast


function updateMessagesAndShowToast() {
     
    // AJAX-������ ��� ��������� ����� ��������� ������ �����
    $.ajax({
        url: '/Home/GetNewMessages', // �������� 'My' �� ��� ������ �����������
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // ������� ������ toast, ������� ���� �������� ������ 30 ������ �����
            $('.toast').each(function () {
                var toastTimestamp = $(this).data('timestamp');
                var currentTimestamp = new Date().getTime();
                var elapsedSeconds = Math.floor((currentTimestamp - toastTimestamp) / 1000);
                if (elapsedSeconds > 30) {
                    $(this).toast('hide');
                }
            });

            // ���������� ����� ��������� � ���� toast-�����������
            data.danger.forEach(function (message) {
                showNotificationToast(message, 'danger', 'fas fa-exclamation-triangle');
            });

            data.warning.forEach(function (message) {
                showNotificationToast(message, 'warning', 'fas fa-exclamation-circle');
            });

            data.info.forEach(function (message) {
                showNotificationToast(message, 'info', 'fas fa-info-circle');
            });
        },
        error: function (xhr, status, error) {
            // ��������� ������ ��� ���������� �������
        }
    });
}

// ������� ��� ����������� toast-����������� ������������� ����
function showNotificationToast(message, type, iconClass) {
    var toast = `
        <div class="toast align-items-center bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true" data-delay="30000" 
        data-autohide="true" data-timestamp="${new Date().getTime()}">     
            <div class="d-flex">
                <div class="toast-body">
                   <i class="${iconClass} mr-2"></i> ${message}                    
                </div>
                <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="�������"></button>
            </div>
        </div>
    `;

    $('.toast-container').append(toast);
    $('.toast').toast('show');
}

// ����� ������� ���������� ������ 3 ������
setInterval(updateMessagesAndShowToast, 3000);
