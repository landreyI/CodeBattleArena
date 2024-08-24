document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .withAutomaticReconnect()
        .build();

    // Personal Chat
    connection.on("ReceiveMessageFromUserChat", function (userSenderNick, message, fontColor) {
        var messagesList = document.getElementById("messagesLisChat");
        if (messagesList) {
            var li = document.createElement("li");
            li.classList.add("list-group-item", "d-flex", "justify-content-start", "align-items-center");

            var span = document.createElement("span");
            span.classList.add("alert", `alert-${fontColor}`, "d-inline-block", "mb-4", "me-1");

            // Получение текущего времени
            var now = new Date();
            var options = {
                day: '2-digit', // Двузначный день месяца
                month: 'long', // Полное название месяца
                hour: '2-digit', // Двузначный час
                minute: '2-digit', // Двузначные минуты
                hour12: false // 24-часовой формат времени
            };
            var formattedTime = now.toLocaleString('en-US', options);
            formattedTime = formattedTime.replace(',', '');

            // Формирование содержимого сообщения
            span.innerHTML = `<strong>${userSenderNick}:</strong> ${message} <small class="text-muted ms-2">(${formattedTime})</small>`;

            li.appendChild(span);
            messagesList.appendChild(li);
        }
    });

    connection.on("DeletePersonalChat", function () {
        if (window.location.pathname === '/Chat/PersonalChat') {
            window.location.replace('/Chat/GetPersonalChats');
        }
    });

    // Invitation
    connection.on("ReceiveMessageFromUser", function (nickname, message, isReload) {
        if (isReload) {
            sessionStorage.setItem("message", `${nickname}: ${message}`);

            setTimeout(function () {
                location.reload();
            }, 100);
        } else {
            var notification = document.getElementById("notificationFriend");
            if (notification) {
                notification.innerHTML = `<strong>${nickname}: ${message}</strong>`;
                notification.style.display = "block";
                setTimeout(() => {
                    notification.style.display = "none";
                }, 5000);
            }
        }
    });
    connection.on("ReceiveMessageSession", function (message, isReload, isBlackout) {
        if (isReload) {
            // Сохраняем данные в sessionStorage перед перезагрузкой страницы
            sessionStorage.setItem("message", message);
            sessionStorage.setItem("isBlackout", isBlackout);

            // Перезагружаем страницу
            setTimeout(function () {
                location.reload();
            }, 100);
        } else {
            // Если перезагрузка не требуется, просто показываем сообщение сразу
            showMessage(message, isBlackout);
        }
    });

    // Этот код срабатывает при загрузке страницы после перезагрузки
    window.onload = function () {
        // Проверяем, есть ли данные в sessionStorage
        var message = sessionStorage.getItem("message");
        var isBlackout = sessionStorage.getItem("isBlackout") === 'true';

        // Если есть сообщение, показываем его
        if (message) {
            showMessage(message, isBlackout);

            // Очищаем sessionStorage, чтобы сообщение не появлялось повторно
            sessionStorage.removeItem("message");
            sessionStorage.removeItem("isBlackout");
        }
    };

    // Функция для отображения сообщения
    function showMessage(message, isBlackout) {
        var notification;
        var overlay = document.getElementById("overlay");

        if (isBlackout) notification = document.getElementById("notificationSessionBlackout");
        else notification = document.getElementById("notificationSession");

        if (notification) {
            notification.innerHTML = `<strong>${message}</strong>`;
            notification.style.display = "block";

            if (isBlackout) overlay.style.display = "block";

            setTimeout(() => {
                notification.style.display = "none";
                if (isBlackout) overlay.style.display = "none";
            }, 5000);
        }
    }
    connection.on("ReceiveSessionStarGame", function () {
        window.location.href = '/Session/ProcessCode';
    });
    connection.on("ReceiveMessageSessionCreatorExit", function (message) {
        deleteCookies();

        sessionStorage.setItem("message", message);

        setTimeout(function () {
            location.reload();
        }, 100);
    });

    function deleteCookies() {
        deleteCookie('IdSession');
        deleteCookie('KeySession');
        deleteCookie('NameSession');
        deleteCookie('StateSassion');
        deleteCookie('AmountPiople');
        deleteCookie('Difficulty');
        deleteCookie('LangProgramming');
    }


    // JoinSession

    var toastElement = document.getElementById('notification');
    var toast = new bootstrap.Toast(toastElement, { autohide: false });

    connection.on("ReceiveMessageFromUserJoinSession", function (nickname, sessionId, message) {
        var notificationMessage = document.getElementById("notification-message");
        var joinButton = document.getElementById("join-btn");

        if (notificationMessage && joinButton) {
            notificationMessage.innerHTML = `<strong>${nickname}: ${message}</strong>`;
            joinButton.onclick = function () {
                joinSession(sessionId);
            };
            toast.show();
        }
    });

    window.closeNotification = function () { // Объявление функции в глобальном контексте
        var toastInstance = bootstrap.Toast.getInstance(toastElement);
        if (toastInstance) {
            toastInstance.hide();
        }
    };

    function joinSession(sessionId) {
        fetch(`/Session/JoinSession?sessionId=${sessionId}&isInvite=true`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ sessionId: sessionId }) // Убедиться, что сервер ожидает это тело
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok.');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    window.location.href = `/Session/InfoSession?sessionId=${sessionId}`;
                } else {
                    showMessage(data.errorMessage, false);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }




    console.log("Starting SignalR connection...");
    connection.start()
        .then(function () {
            console.log("SignalR connected successfully.");
        })
        .catch(function (err) {
            console.error("SignalR connection failed:", err);
        });

    connection.onreconnecting(function (err) {
        console.warn("SignalR reconnecting:", err);
    });

    connection.onreconnected(function () {
        console.log("SignalR reconnected successfully.");
    });

    connection.onclose(function (err) {
        console.error("SignalR connection closed:", err);
    });

});
