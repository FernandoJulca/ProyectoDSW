function agregarAlCarrito(botonOId, cantidad = 1) {
    let id;
    if (typeof botonOId === 'object') {
        id = botonOId.getAttribute('data-id');
    } else {
        id = botonOId;
    }

    const data = new URLSearchParams();
    data.append('id', id);
    data.append('cantidad', cantidad);

    fetch('/Carro/AgregarAlCarrito', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: data.toString()
    })
        .then(async response => {
            if (response.ok) {
                actualizarContadorCarrito();

                const carritoAbierto = document.querySelector('#offcanvasCarrito.show');
                if (carritoAbierto) {
                    cargarCarrito();
                }

            } else {
                const errorData = await response.json();
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: errorData.mensaje || 'Error al agregar el producto',
                });
            }
        })
        .catch(error => {
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error de red o inesperado',
            });
        });
}
function cargarDetallesProducto(idProducto) {
    fetch(`/Cliente/ObtenerProductoPorId?id=${idProducto}`)
        .then(response => {
            if (!response.ok) throw new Error("No se encontró el producto");
            return response.json();
        })
        .then(data => {
            document.getElementById("productoModalLabel").textContent = data.nombre;
            document.getElementById("modalDescripcionProducto").textContent = data.descripcion;
            document.getElementById("modalCategoriaProducto").textContent = data.categoria;
            document.getElementById("modalPrecioProducto").textContent = data.precio;
            console.log("Valor de data.imagen:", data.imagen);
            const imgElem = document.getElementById("modalImagenProducto");
            imgElem.src = data.imagen;  // Solo asignar la ruta

            // Opcional: para manejar error y poner imagen por defecto
            imgElem.onerror = function () {
                this.onerror = null;
                this.src = '/assets/no-imagen.jpg';
            };


            const stockWarning = document.getElementById("stockWarning");
            const btnAgregar = document.getElementById("btnAgregarCarrito");

            if (data.stock === 0) {
                stockWarning.classList.remove("d-none");
                btnAgregar.disabled = true;
            } else {
                stockWarning.classList.add("d-none");
                btnAgregar.disabled = false;
            }

            btnAgregar.onclick = function () {
                const cantidad = parseInt(document.getElementById('inputCantidad').value);
                if (cantidad > 0) {
                    agregarAlCarrito(data.id, cantidad);
                    document.getElementById('inputCantidad').value = 1; // reset cantidad
                } else {
                    iziToast.error({
                        title: 'Error',
                        message: 'Ingrese una cantidad válida',
                        position: 'topRight'
                    });
                }
            };
        })
        .catch(err => {
            alert("Error al cargar el producto");
            console.error(err);
        });
}
