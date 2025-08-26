document.addEventListener("DOMContentLoaded", () => {
    const user = window.currentUser || "";
    console.log("Usuario conectado:", user);

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7021/chatHub", {
            withCredentials: true
        })
        .build();

    connection.start()
        .then(() => {
            console.log("✅ SignalR conectado");
            const modalButtons = document.querySelectorAll('[data-idventa]');
            modalButtons.forEach(button => {
                const idVenta = button.getAttribute('data-idventa');
                connection.invoke("JoinGroup", idVenta)
                    .then(() => console.log(`Unido al grupo ${idVenta}`))
                    .catch(err => console.error(`Error al unirse al grupo ${idVenta}:`, err));
            });
        })
        .catch(err => console.error("❌ Error al conectar:", err));

    function saveMessageToLocalStorage(idVenta, sender, message) {
        const key = `chat-${idVenta}`;
        const storedHistory = JSON.parse(localStorage.getItem(key)) || [];
        storedHistory.push({ sender: sender, message: message });
        localStorage.setItem(key, JSON.stringify(storedHistory));
    }

    function displayMessage(chatBox, sender, message) {
        let msgHtml;
        if (sender === user) {
            msgHtml = `
                <div class="d-flex justify-content-end mb-2">
                    <div class="bg-color-primary text-white p-2 rounded-3 shadow-sm" style="max-width: 70%;">
                        ${message}
                    </div>
                </div>`;
        } else {
            msgHtml = `
                <div class="d-flex justify-content-start mb-2">
                    <div class="bg-white text-dark p-2 rounded-3 shadow-sm d-flex justify-content-start flex-column" style="max-width: 70%;">
                        <strong>${sender}:</strong> <p style="margin:0;">${message}</p>
                    </div>
                </div>`;
        }
        chatBox.insertAdjacentHTML("beforeend", msgHtml);
        chatBox.scrollTop = chatBox.scrollHeight;
    }

    connection.on("ReceiveMessage", (groupId, userFromServer, message) => {
        console.log("Mensaje recibido:", groupId, userFromServer, message);
        const chatBox = document.getElementById('messagesList');
        const currentIdVenta = chatBox.getAttribute('data-idventa');

        if (chatBox && currentIdVenta === groupId) {
            saveMessageToLocalStorage(groupId, userFromServer, message);
            displayMessage(chatBox, userFromServer, message);
        }
    });

    const singleModal = document.getElementById('chatModal');
    if (singleModal) {
        const form = singleModal.querySelector('#formMensaje');
        const input = singleModal.querySelector('#messageInput');
        const chatBox = singleModal.querySelector('#messagesList');

        form.onsubmit = event => {
            event.preventDefault();
            const idVenta = chatBox.getAttribute('data-idventa');
            const mensaje = input.value.trim();
            if (mensaje === "") return;

            connection.invoke("SendMessageToGroup", idVenta.toString(), user, mensaje)
                .then(() => {
                    input.value = "";
                })
                .catch(err => console.error(err));
        };

        singleModal.addEventListener('show.bs.modal', event => {
            const button = event.relatedTarget;
            const idVenta = button.getAttribute('data-idventa');

            const modalTitle = singleModal.querySelector('.modal-title');
            modalTitle.textContent = `Chat de la Venta #${idVenta}`;

            chatBox.setAttribute('data-idventa', idVenta);
            chatBox.innerHTML = '';

            const storedHistory = JSON.parse(localStorage.getItem(`chat-${idVenta}`));
            if (storedHistory) {
                storedHistory.forEach(msg => {
                    displayMessage(chatBox, msg.sender, msg.message);
                });
            }
        });

        singleModal.addEventListener('hidden.bs.modal', () => {
            chatBox.removeAttribute('data-idventa');
        });
    }
});
