function cargarCarrito() {
    console.log('cargarCarrito llamada');
    fetch('/Carro/ObtenerCarrito')
        .then(response => response.json())
        .then(data => {
            console.log('Datos recibidos:', data);
            const contenedor = document.getElementById('carritoProductos');
            const productosTotalesElem = document.getElementById('productosTotales');
            const totalCarritoElem = document.getElementById('totalCarrito');
            contenedor.innerHTML = '';

            if (data.length === 0) {
                contenedor.innerHTML = '<p class="text-center text-muted mt-3">El carrito está vacío</p>';
                productosTotalesElem.textContent = '0';
                totalCarritoElem.textContent = 'S/. 0.00';
                return;
            }

            let total = 0;
            let cantidadTotal = 0;

            data.forEach(item => {
                total += item.subTotal;
                cantidadTotal += item.cantidad;
                let src = item.imagen;

                if (!src || src === "null" || src.trim() === "") {
                    src = `/assets/productos/P${item.idProducto}.jpg`;
                }

                console.log("Imagen cargada en carrito:", src);
                const productoHtml = `
                    <div class="d-flex mb-3 align-items-start p-3 rounded bg-light shadow-sm position-relative">
                        <img src="${src}" alt="Producto" width="70" height="70" class="rounded me-3 object-fit-cover" onerror="this.onerror=null;this.src='/assets/no-imagen.jpg';" />
                        <div class="flex-grow-1">
                            <h5 class="mb-1 fw-semibold">${item.nombre}</h5>
                            <h6 class="d-block mb-2 titulos-Primary fw-bold">S/. ${item.precio.toFixed(2)}</h6>
                            <div class="d-flex align-items-center gap-2 mt-2">
                                <button class="btn btn-outline-secondary btn-icon d-flex align-items-center justify-content-center fs-2" onclick="disminuirCantidad(${item.idProducto})">-</button>
                                <span class="fw-bold fs-5 px-2">${item.cantidad}</span>
                                <button class="btn btn-outline-secondary btn-icon d-flex align-items-center justify-content-center fs-2" onclick="aumentarCantidad(${item.idProducto})">+</button>
                            </div>
                        </div>
                        <div class="d-flex flex-column align-items-end ms-2">
                            <button class="btn btn-sm btn-outline-danger mb-2" title="Eliminar" onclick="eliminarProducto(${item.idProducto})">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                            <span class="fw-bold color-gray-900">
                                S/. ${(item.subTotal).toFixed(2)}
                            </span>
                        </div>
                    </div>
                `;

                contenedor.insertAdjacentHTML('beforeend', productoHtml);
            });

            productosTotalesElem.textContent = cantidadTotal;
            totalCarritoElem.textContent = `S/. ${total.toFixed(2)}`;
        })
        .catch(error => {
            console.error('Error al cargar carrito:', error);
        });
}

document.addEventListener('DOMContentLoaded', () => {
    const offcanvasCarrito = document.getElementById('offcanvasCarrito');
    offcanvasCarrito.addEventListener('show.bs.offcanvas', cargarCarrito);
});

function actualizarContadorCarrito() {
    fetch('/Carro/ObtenerCarrito')
        .then(response => response.json())
        .then(data => {
            const contadorElems = document.querySelectorAll('.top-counter');
            const totalCantidad = data.length;
            contadorElems.forEach(elem => {
                elem.textContent = totalCantidad;
            });
        });
}

document.addEventListener('DOMContentLoaded', () => {
    actualizarContadorCarrito();
});

function aumentarCantidad(idProducto) {
    const formData = new URLSearchParams();
    formData.append('id', idProducto);

    fetch('/Carro/SumarCantidad', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => {
            if (!response.ok) throw new Error('Error al aumentar cantidad');
            return response.text();
        })
        .then(() => {
            cargarCarrito();
            actualizarContadorCarrito();
        })
        .catch(error => {
            console.error(error);
            Swal.fire('Error', 'No se pudo aumentar la cantidad', 'error');
        });
}

function disminuirCantidad(idProducto) {
    const formData = new URLSearchParams();
    formData.append('id', idProducto);

    fetch('/Carro/RestarCantidad', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => {
            if (!response.ok) throw new Error('Error al disminuir cantidad');
            return response.text();
        })
        .then(() => {
            cargarCarrito();
            actualizarContadorCarrito();
        })
        .catch(error => {
            console.error(error);
            Swal.fire('Error', 'No se pudo disminuir la cantidad', 'error');
        });
}


function eliminarProducto(idProducto) {
    fetch('/Carro/QuitarDelCarrito', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `idProducto=${idProducto}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                cargarCarrito();
                actualizarContadorCarrito();
            } else {
                Swal.fire('Error', data.mensaje, 'error');
            }
        })
        .catch(err => {
            console.error("Error al eliminar producto:", err);
            Swal.fire('Error', 'Error al eliminar el producto', 'error');
        });
}
