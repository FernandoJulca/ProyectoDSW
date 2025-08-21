
let connection;
let MY_USER_ID;
let txtMensajeVista = document.getElementById("txtMensaje");
let idUserVista = document.getElementById("idMensajeEnviar");
function añadirMensajeDiv(remitente, mensaje) {

    const contenedorMensajes = document.querySelector(".bodyChat");
    const nuevoMensaje = document.createElement("div");

    nuevoMensaje.textContent = remitente + ": " + mensaje
    contenedorMensajes.appendChild(nuevoMensaje);
}

// Inicializamos conexión
async function initConexion() {

    if (!MY_USER_ID) {
        MY_USER_ID = prompt("Ingresa tu USER ID para este navegador:"); 
        console.log("El usuario para esta ventana es: " + MY_USER_ID)
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7021/chatHub")
        .build();

    // Escuchamos los mensajes que envía el Hub
    connection.on("ReceiveMessage", (mensaje) => {
        añadirMensajeDiv("Otro", mensaje)
    });

    try {
        await connection.start();
        console.log("Conectado al hub");

        // Registramos nuestro usuario en el servidor
        await connection.invoke("RegisterUser", parseInt(MY_USER_ID));
    } catch (err) {
        console.error(err);
    }
}

// Llamar a este método para enviar a otro usuario
function enviarMensaje() {

    let mensaje = txtMensajeVista.value;
    let idRuta = parseInt(idUserVista.value)

    console.log("ID PASANDO: " + idRuta)
    console.log("Mensaje pasando: " + mensaje)
    if (!mensaje || isNaN(idRuta)) {
        return console.log("Error")
    }
    if (connection.state === "Connected") {
        connection.invoke("SendMessage", idRuta, mensaje);
    }
    añadirMensajeDiv("Yo", mensaje)
    txtMensajeVista.value = "";
}

/*function enviarMensaje(destinoUserId, texto) {

    if (connection.state === "Connected") {
        connection.invoke("SendMessage", destinoUserId, texto);
    }
    añadirMensajeDiv("Yo", texto)

    txtMensajeVista.value = "";
}*/



// Ejecutamos la conexión al cargar la página
initConexion();