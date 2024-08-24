document.addEventListener("DOMContentLoaded", async function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    document.getElementById("sendButtonSessionChat").disabled = true;

    connection.on("ReceiveMessageGroupSession", function (nickname, message) {
        var messagesList = document.getElementById("messagesListSessionChat");
        if (messagesList) {
            var li = document.createElement("li");
            li.classList.add("list-group-item", "d-flex", "justify-content-start", "align-items-center");

            var span = document.createElement("span");
            span.classList.add("alert", "alert-success", "d-inline-block", "mb-4", "me-1");
            span.innerHTML = `<strong>${nickname}:</strong> ${message}`;

            li.appendChild(span);
            messagesList.appendChild(li);
        }
    });

    try {
        await connection.start();
        document.getElementById("sendButtonSessionChat").disabled = false;
    } catch (err) {
        console.error(err.toString());
    }

    document.getElementById("sendButtonSessionChat").addEventListener("click", async function (event) {
        var sessionId = document.getElementById("idSessionChat").value;
        var nickname = document.getElementById("userInputSessionChat").value;
        var message = document.getElementById("messageInputSessionChat").value;

        try {
            await connection.invoke("SendMessageToGroupSession", sessionId, nickname, message);
        } catch (err) {
            console.error(err.toString());
        }

        event.preventDefault();
    });
});