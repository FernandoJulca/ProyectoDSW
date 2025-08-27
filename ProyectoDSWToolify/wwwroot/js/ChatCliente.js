// Obtenemos el nombre de usuario desde el backend
const user = window.currentUser || "";
console.log("Usuario conectado:", user);

// Inicializamos la conexión de SignalR
const connection = new signalR.HubConnectionBuilder()
	.withUrl("https://localhost:7021/chatHub", {
		withCredentials: true
	})
	.build();

// Iniciar la conexión y unirse a los grupos
connection.start()
	.then(() => {
		console.log("✅ SignalR conectado");

		// Unirse a los grupos de las ventas al cargar la página
		// Esto asegura que siempre estemos escuchando los cambios de estado
		const ventaIds = document.querySelectorAll('.accordion-item');
		ventaIds.forEach(item => {
			const idVenta = item.querySelector('.accordion-header').id.replace('headingVenta-', '');
			connection.invoke("JoinGroup", idVenta)
				.then(() => console.log(`✅ Unido al grupo ${idVenta}`))
				.catch(err => console.error(`❌ Error al unirse al grupo ${idVenta}:`, err));
		});
	})
	.catch(err => console.error("❌ Error al conectar:", err));

// Guardar mensaje en localStorage
function saveMessageToLocalStorage(idVenta, sender, message) {
	const key = `chat-${idVenta}`;
	const storedHistory = JSON.parse(localStorage.getItem(key)) || [];
	storedHistory.push({
		sender: sender,
		message: message
	});
	localStorage.setItem(key, JSON.stringify(storedHistory));
}

// Mostrar mensaje en chatBox
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

// Evento para recibir mensajes y mostrarlos
connection.on("ReceiveMessage", (groupId, userFromServer, message) => {
	console.log("Mensaje recibido:", groupId, userFromServer, message);
	const chatBox = document.getElementById('chatBox');
	const currentIdVenta = chatBox.getAttribute('data-idventa');

	if (chatBox && currentIdVenta === groupId) {
		saveMessageToLocalStorage(groupId, userFromServer, message);
		displayMessage(chatBox, userFromServer, message);
	}
});

// Evento que se dispara cuando el repartidor acepta una venta (cambia el estado a 'Transportado').
// Actualiza la interfaz en tiempo real: cambia el estado del pedido en el acordeón correspondiente,
// agrega el botón de mensajería, y muestra una alerta informativa al usuario.
connection.on("VentaAceptada", (idVenta, mensaje) => {
	console.log(`Venta aceptada para venta ${idVenta}: ${mensaje}`);

	const heading = document.getElementById(`headingVenta-${idVenta}`);

	if (heading) {
		const accordionBody = heading.closest('.accordion-item').querySelector('.accordion-body');
		if (accordionBody) {
			const estadoParagraph = accordionBody.querySelector('p strong');
			if (estadoParagraph) {
				const existingText = estadoParagraph.nextSibling;
				if (existingText && existingText.nodeType === 3) { 
					existingText.textContent = ' Transportado';
				}
			}
		}

		const messageButtonHtml = `
            <button type="button" class="btn btn-warning btn-sm ms-3 shadow-sm fw-semibold d-flex align-items-center justify-content-center mensaje-btn" data-bs-toggle="modal" data-bs-target="#mensajeModal" data-idventa="${idVenta}" style="min-width:110px;">
                <i class="fas fa-envelope me-2"></i> Mensajería
            </button>
        `;
		heading.insertAdjacentHTML('beforeend', messageButtonHtml);
	}

	Swal.fire({
		title: '¡Pedido Aceptado!',
		text: mensaje,
		icon: 'info',
		confirmButtonText: 'Aceptar'
	});
});

// Evento para cuando el repartidor cancela la venta
connection.on("VentaCancelada", (idVenta, mensaje) => {
	console.log(`Venta cancelada para venta ${idVenta}: ${mensaje}`);

	const modal = document.getElementById('mensajeModal');
	const chatBox = modal.querySelector('#chatBox');
	const ventaModal = chatBox.getAttribute('data-idventa');

	// Cierra el modal y recarga la página, similar al evento de "EntregaConfirmada"
	if (ventaModal === idVenta.toString()) {
		if (window.bootstrapModalInstance) {
			window.bootstrapModalInstance.hide();

			Swal.fire({
				title: '¡Pedido Cancelado!',
				text: mensaje,
				icon: 'error',
				confirmButtonText: 'Aceptar'
			}).then(() => {
				window.location.reload();
			});
		}
	}
});

// Evento para cuando el repartidor confirma la entrega
connection.on("EntregaConfirmada", (idVenta, mensaje) => {
	console.log(`Entrega confirmada para venta ${idVenta}: ${mensaje}`);

	const modal = document.getElementById('mensajeModal');
	const chatBox = modal.querySelector('#chatBox');
	const ventaModal = chatBox.getAttribute('data-idventa');

	if (ventaModal === idVenta.toString()) {
		if (window.bootstrapModalInstance) {
			window.bootstrapModalInstance.hide();

			Swal.fire({
				title: '¡Pedido Entregado!',
				text: mensaje,
				icon: 'success',
				confirmButtonText: 'Aceptar'
			}).then((result) => {
				if (result.isConfirmed) {
					window.location.reload();
				}
			});
		} else {
			console.warn("No se encontró instancia del modal para cerrar.");
		}
	}
});

// Get DOM elements
const singleModal = document.getElementById('mensajeModal');
const form = singleModal.querySelector('#formMensaje');
const input = singleModal.querySelector('#mensajeText');
const chatBox = singleModal.querySelector('#chatBox');

// Enviar mensaje al hacer submit
form.onsubmit = event => {
	event.preventDefault();
	const idVenta = chatBox.getAttribute('data-idventa');
	const mensaje = input.value.trim();
	if (mensaje === "") return;

	connection.invoke("SendMessageToGroup", idVenta.toString(), user, mensaje)
		.then(() => {
			console.log("Mensaje enviado correctamente");
			input.value = "";
		})
		.catch(err => console.error("Error al enviar mensaje:", err));
};

// Evento para manejar la apertura del modal
singleModal.addEventListener('show.bs.modal', event => {
	const button = event.relatedTarget;
	const idVenta = button.getAttribute('data-idventa');

	if (connection.state === signalR.HubConnectionState.Connected) {
		connection.invoke("JoinGroup", idVenta)
			.then(() => console.log(`✅ Unido al grupo ${idVenta} al abrir el modal.`))
			.catch(err => console.error(`❌ Error al unirse al grupo ${idVenta}:`, err));
	}

	const modalTitle = singleModal.querySelector('.modal-title');
	modalTitle.textContent = `Chat de la Venta #${idVenta}`;

	chatBox.setAttribute('data-idventa', idVenta);
	chatBox.innerHTML = '';

	const storedHistory = JSON.parse(localStorage.getItem(`chat-${idVenta}`));
	if (storedHistory) {
		storedHistory.forEach(msg => {
			displayMessage(chatBox, msg.sender, msg.message);
		});
		chatBox.scrollTop = chatBox.scrollHeight;
	}
});

// Evento para manejar el cierre del modal
singleModal.addEventListener('hidden.bs.modal', () => {
	chatBox.removeAttribute('data-idventa');
});
