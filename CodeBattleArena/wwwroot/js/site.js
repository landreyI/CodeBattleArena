function showMessage(message, isSuccess = true) {
    var alertClass = isSuccess ? 'alert-success' : 'alert-danger';
    var alert = $('<div class="alert ' + alertClass + ' position-fixed top-0 end-0 p-3" style="z-index: 1050;">')
        .text(message)
        .appendTo('body');

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            alert.remove();
        });
    }, 3000);
}
$(document).ready(function () {
    $(document).on('submit', '.ajax-form', function(event) {
        event.preventDefault();

        var form = $(this);
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize(),
            success: function(response) {
                if (response.success) {
                    // Закрытие модальных окон, если они открыты
                    form.closest('.modal').modal('hide');
                    if (form.attr('id') === 'authorizationForm' || form.attr('id') === 'registrationForm'
                        || form.attr('id') === 'joinForm' || form.attr('id') === 'editProfile') {
                        location.reload();
                    }
                    showMessage("Successful!");
                } else {
                    var errorMessage = response.errorMessage || 'An unknown error occurred.';
                    showMessage(errorMessage, false);
                }
            },
            error: function(xhr, status, error) {
                var errorMessage = xhr.status + ': ' + xhr.statusText;
                showMessage('Error - ' + errorMessage, false);
            }
        });
    });
    $(document).on('submit', '.ajax-form-reload', function (event) {
        event.preventDefault();

        var form = $(this);
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    location.reload();
                    showMessage("Successful!");
                } else {
                    var errorMessage = response.errorMessage || 'An unknown error occurred.';
                    showMessage(errorMessage, false);
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = xhr.status + ': ' + xhr.statusText;
                showMessage('Error - ' + errorMessage, false);
            }
        });
    });
});

document.getElementById('theme').addEventListener('change', function () {
    var selectedTheme = this.value;

    fetch('/Home/ChangeTheme', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `theme=${encodeURIComponent(selectedTheme)}`
    })
        .then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                return response.text().then(errorText => {
                    console.error('Error:', errorText);
                });
            }
        })
        .catch(error => console.error('Error:', error));
});

function togglePasswordVisibility() {
    var passwordInput = document.getElementById("formSignupPassword");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
}

function togglePasswordVisibilityAuth() {
    var passwordInput = document.getElementById("formSignupPasswordAuth");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }
}
function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
}
