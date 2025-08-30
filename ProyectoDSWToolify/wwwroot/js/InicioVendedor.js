document.addEventListener("DOMContentLoaded", function () {
    const fechaCompleta = document.getElementById("fechaCompleta");
    const fechaCorta = document.getElementById("fechaCorta");
    const inicialUsuario = document.getElementById("inicialUsuario");
    const fotoUsuario = document.getElementById("fotoUsuario");
    const inputFoto = document.getElementById("inputFoto");

    const nombreUsuario = document.getElementById("nombreUsuario");
    const correoUsuario = document.getElementById("correoUsuario");

    let usuario = {
        nombre: "Juan",
        correo: "juan.gonzales@example.com",
        foto: null
    };

    const fecha = new Date();
    fechaCompleta.textContent = fecha.toLocaleDateString("es-ES", {
        weekday: "long",
        day: "numeric",
        month: "long",
        year: "numeric"
    });
    fechaCorta.textContent = fecha.toLocaleDateString("es-ES");

    // Usuario
    nombreUsuario.textContent = usuario.nombre;
    correoUsuario.textContent = usuario.correo;
    inicialUsuario.textContent = usuario.nombre.charAt(0).toUpperCase();

    if (usuario.foto) {
        inicialUsuario.classList.add("d-none");
        fotoUsuario.classList.remove("d-none");
        fotoUsuario.src = usuario.foto;
    }

    // Subir foto
    inputFoto.addEventListener("change", (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = () => {
                usuario.foto = reader.result;
                fotoUsuario.src = usuario.foto;
                fotoUsuario.classList.remove("d-none");
                inicialUsuario.classList.add("d-none");
            };
            reader.readAsDataURL(file);
        }
    });

    // CONUSMIR ENDPOINT API REST del controller
    fetch("/Vendedor/DatosMensuales", { cache: "no-store" })
        .then(res => res.json())
        .then(data => {
            document.getElementById("ventasMensual").textContent = data.ventasMensuales;
            document.getElementById("prodsVendidosMensual").textContent = data.productosMensuales;
            document.getElementById("clisAtendidosMensual").textContent = data.clientesMensuales;
            document.getElementById("ingresosMensuales").textContent = data.ingresosMensuales.toFixed(2);
        })
        .catch(err => console.error("Error al cargar datos mensuales:", err));
});
