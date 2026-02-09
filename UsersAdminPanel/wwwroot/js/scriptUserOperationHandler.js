$(document).ready(function () {
    // Пример использования JWT для запроса
    const token = localStorage.getItem('jwtToken');

    if (token) {
        $.ajax({
            url: '/Users/List',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            success: function (data) {
                $('#userContainer').html(data);
            },
            error: function () {
                console.error('List request failed!');
            }
        });
    }    

});

$(document).on('click', '#checkAllBox', function () {
    $('.checkBoxClass').prop('checked', this.checked);
});

$(document).on('click', '#blockBtn', function () {
    const checkedValues = Array.from(
        document.querySelectorAll('.checkBoxClass:checked')
    ).map(cb => cb.value);
    console.log(checkedValues);
    $.ajax({
        url: '/Users/Block',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken')
        },
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(checkedValues),
        success: function (response) {
            if (response.success) {
                window.location.href = '/Home/Index';
            } else {
                $('#errorBlock').show();
                $('#errorBlock').text(response.message);                
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#errorBlock').text(jqXHR.responseText).show();
        }
    });
});

$(document).on('click', '#unblockBtn', function () {
    const checkedValues = Array.from(
        document.querySelectorAll('.checkBoxClass:checked')
    ).map(cb => cb.value);
    console.log(checkedValues);

    $.ajax({
        url: '/Users/Unblock',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken')
        },
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(checkedValues),
        success: function (response) {
            if (response.success) {
                window.location.href = '/Home/Index';
            } else {
                $('#errorBlock').show();
                $('#errorBlock').text(response.message);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#errorBlock').text(jqXHR.responseText).show();
        }
    });
});

$(document).on('click', '#deleteBtn', function () {
    const checkedValues = Array.from(
        document.querySelectorAll('.checkBoxClass:checked')
    ).map(cb => cb.value);
    console.log(checkedValues);

    $.ajax({
        url: '/Users/Delete',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken')
        },
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(checkedValues),
        success: function (response) {
            if (response.success) {
                window.location.href = '/Home/Index';
            } else {
                $('#errorBlock').show();
                $('#errorBlock').text(response.message);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#errorBlock').text(jqXHR.responseText).show();
        }
    });
});

$(document).on('click', '#deleteUnverBtn', function () {
    
    $.ajax({
        url: '/Users/DeleteUnconfirmed',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken')
        },
        type: 'POST',
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                window.location.href = '/Home/Index';
            } else {
                $('#errorBlock').show();
                $('#errorBlock').text(response.message);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('#errorBlock').text(jqXHR.responseText).show();
        }
    });
});

