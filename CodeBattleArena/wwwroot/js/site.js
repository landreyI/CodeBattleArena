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

    // Фильтрация 
    $('.filtration').on('input', function () {
        let searchValue = $(this).val().toLowerCase();

        $('.main-item').each(function () {
            let name = $(this).attr('data-main').toLowerCase();

            if (name.includes(searchValue)) {
                console.log('Search name:', name);
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

    document.querySelectorAll(".card-3d").forEach(card => {
        let offsetCard = card.dataset.offset || 20;

        card.addEventListener("mousemove", (e) => {
            let rect = card.getBoundingClientRect();
            let offsetX = 0.5 - (e.clientX - rect.left) / rect.width;
            let offsetY = 0.5 - (e.clientY - rect.top) / rect.height;

            card.style.transform = `translateY(${-offsetY * offsetCard}px)
                                        rotateX(${-offsetY * offsetCard}deg)
                                        rotateY(${offsetX * (offsetCard * 2)}deg)`;
        });

        card.addEventListener("mouseleave", () => {
            card.style.transform = "rotateY(0deg) rotateX(0deg)";
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

function typeText(element, text, speed, callback) {
    let i = 0;
    function type() {
        if (i < text.length) {
            if (text.charAt(i) === '\n') {
                element.innerHTML += '<br>';
            } else {
                element.innerHTML += text.charAt(i);
            }
            i++;
            setTimeout(type, speed);
        }
        else {
            if (callback) callback(); // Вызываем callback после завершения печати текста
        }
    }
    type();
}
