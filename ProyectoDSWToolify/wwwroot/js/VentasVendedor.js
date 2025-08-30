document.addEventListener("DOMContentLoaded", () => {
    let productos = [];
    let seleccionados = [];
    let carrito = [];

    const apiUrl = "/Vendedor/ListarProductos";
    const apiVentaUrl = "/Vendedor/GenerarVentaVendedor";

    const tablaProductos = document.getElementById("tablaProductos");
    const contenedorSeleccionados = document.getElementById("contenedorSeleccionados");
    const contenedorCarrito = document.getElementById("contenedorCarrito");
    const countSeleccionados = document.getElementById("countSeleccionados");
    const countCarrito = document.getElementById("countCarrito");
    const alertPlaceholder = document.getElementById("alert-placeholder");

    const userIdElement = document.getElementById("currentUserId");
    const idUsuario = userIdElement ? parseInt(userIdElement.value, 10) : 0;
    const inputBusqueda = document.getElementById("txtBusqueda");

    if (idUsuario === 0) {
        console.error("No se pudo obtener el ID del usuario. Asegúrate de que el campo oculto existe y tiene un valor válido.");
    }

    console.log(idUsuario);

    function mostrarMensaje(mensaje, tipo) {
        if (!alertPlaceholder) return;
        alertPlaceholder.innerHTML = `
            <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
                ${mensaje}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        setTimeout(() => {
            if (alertPlaceholder.firstChild) {
                alertPlaceholder.removeChild(alertPlaceholder.firstChild);
            }
        }, 5000);
    }

    //fetch al controller pa cargar Lstproductos
    async function cargarProductos() {
        try {
            const resp = await fetch(apiUrl);
            if (!resp.ok) {
                const error = await resp.text();
                throw new Error(`Error del servidor: ${resp.status} - ${error}`);
            }
            productos = await resp.json();
            renderProductos();
        } catch (err) {
            console.error("Error al cargar productos", err);
            mostrarMensaje("Error al cargar productos. Por favor, intenta de nuevo más tarde.", "danger");
        }
    }

    // Nueva función para filtrar los productos de forma dinámica
    function filtrarProductos() {
        const textoBusqueda = inputBusqueda.value.toLowerCase();
        const productosFiltrados = productos.filter(producto =>
            producto.nombre.toLowerCase().includes(textoBusqueda) ||
            producto.categoria.descripcion.toLowerCase().includes(textoBusqueda) ||
            String(producto.idProducto).includes(textoBusqueda)
        );
        renderProductos(productosFiltrados);
    }

    function pasarAlCarrito() {
        if (seleccionados.length === 0) {
            mostrarMensaje("No hay productos para agregar a la venta.", "warning");
            return;
        }
        carrito.push(...seleccionados);
        seleccionados = [];
        renderSeleccionados();
        renderCarrito();
    }

    async function finalizarVenta() {
        if (carrito.length === 0) {
            mostrarMensaje("El carrito está vacío. Agrega productos para finalizar la venta.", "danger");
            return;
        }

        console.log("El ID de usuario que se enviará es:", idUsuario);

        if (idUsuario === 0) {
            mostrarMensaje("No se pudo obtener el ID de usuario. No se puede procesar la venta.", "danger");
            return;
        }

        const venta = {
            idVenta: 0,
            idUsuario: idUsuario,
            fecha: new Date().toISOString(),
            total: carrito.reduce((acc, d) => acc + d.subTotal, 0),
            tipoVenta: null,
            estado: null,
            detalles: carrito.map(item => ({
                idProducto: item.producto.idProducto,
                cantidad: item.cantidad,
                subTotal: item.subTotal
            }))
        };

        try {
            const resp = await fetch(apiVentaUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(venta)
            });

            if (resp.ok) {
                const result = await resp.json();
                mostrarMensaje(`¡Venta #${result.idVenta} realizada con éxito!`, "success");
                carrito = [];
                renderCarrito();
                cargarProductos();
            } else {
                const error = await resp.text();
                mostrarMensaje(`Error al finalizar la venta: ${resp.statusText}. Detalles: ${error}`, "danger");
                console.error("Error al finalizar la venta:", resp.status, error);
            }
        } catch (err) {
            console.error("Error en la venta:", err);
            mostrarMensaje("Ocurrió un error al procesar la venta. Inténtalo de nuevo.", "danger");
        }
    }

    //Renderizar
    function renderProductos(listaDeProductos = productos) {
        tablaProductos.innerHTML = listaDeProductos.map(p => `
            <tr>
                <td>${p.idProducto}</td>
                <td>${p.nombre}</td>
                <td>${p.categoria.descripcion}</td>
                <td>${p.precio.toFixed(2)}</td>
                <td>${p.stock}</td>
                <td>
                    <button class="btn btn-sm btn-primary" data-id="${p.idProducto}">
                        Agregar
                    </button>
                </td>
            </tr>
        `).join("");
    }

    function renderSeleccionados() {
        if (seleccionados.length === 0) {
            contenedorSeleccionados.innerHTML = `<p class="text-center text-muted py-4">No hay productos seleccionados</p>`;
        } else {
            contenedorSeleccionados.innerHTML = `
                <table class="table table-striped">
                    <thead><tr><th>Nombre</th><th>Precio</th><th>Cantidad</th><th>Subtotal</th><th>Acción</th></tr></thead>
                    <tbody>
                        ${seleccionados.map(item => `
                            <tr>
                                <td>${item.producto.nombre}</td>
                                <td>${item.producto.precio.toFixed(2)}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-secondary btn-disminuir" data-id="${item.producto.idProducto}">-</button>
                                    ${item.cantidad}
                                    <button class="btn btn-sm btn-outline-secondary btn-aumentar" data-id="${item.producto.idProducto}">+</button>
                                </td>
                                <td>${item.subTotal.toFixed(2)}</td>
                                <td><button class="btn btn-sm btn-danger btn-eliminar-seleccionado" data-id="${item.producto.idProducto}">
                                    <i class="bi bi-trash"></i>
                                </button></td>
                            </tr>
                        `).join("")}
                    </tbody>
                </table>
                <div class="d-flex justify-content-end">
                    <button class="btn btn-success" id="btnPasarAlCarrito">Agregar a Venta</button>
                </div>
            `;
        }
        countSeleccionados.textContent = `${seleccionados.length} productos`;
    }

    function renderCarrito() {
        if (carrito.length === 0) {
            contenedorCarrito.innerHTML = `<p class="text-center text-muted py-4">El carrito está vacío</p>`;
        } else {
            const total = carrito.reduce((acc, d) => acc + d.subTotal, 0);
            contenedorCarrito.innerHTML = `
                <table class="table table-bordered">
                    <thead><tr><th>Producto</th><th>Cant.</th><th>Precio</th><th>Subtotal</th><th>Acción</th></tr></thead>
                    <tbody>
                        ${carrito.map(item => `
                            <tr>
                                <td>${item.producto.nombre}</td>
                                <td>${item.cantidad}</td>
                                <td>${item.producto.precio}</td>
                                <td>${item.subTotal.toFixed(2)}</td>
                                <td><button class="btn btn-sm btn-danger btn-eliminar-carrito" data-id="${item.producto.idProducto}">
                                    <i class="bi bi-trash"></i>
                                </button></td>
                            </tr>
                        `).join("")}
                    </tbody>
                </table>
                <h4 class="text-end">Total: ${total.toFixed(2)}</h4>
                <div class="d-flex justify-content-end">
                    <button class="btn btn-success" id="btnFinalizarVenta">Finalizar Venta</button>
                </div>
            `;
        }
        countCarrito.textContent = `${carrito.length} productos`;
    }

    //Agregar, disminuir y eliminar de prodSeleccionados/carrito
    function agregarSeleccionado(id) {
        const producto = productos.find(p => p.idProducto === id);
        const existente = seleccionados.find(d => d.producto.idProducto === id);

        if (producto.stock === 0) {
            mostrarMensaje("El producto no tiene stock disponible.", "info");
            return;
        }

        if (existente) {
            if (existente.cantidad < producto.stock) {
                existente.cantidad++;
                existente.subTotal = existente.cantidad * producto.precio;
            } else {
                mostrarMensaje("No hay suficiente stock disponible.", "warning");
            }
        } else {
            seleccionados.push({
                producto,
                cantidad: 1,
                subTotal: producto.precio
            });
        }
        renderSeleccionados();
    }

    function aumentarCantidad(id) {
        const item = seleccionados.find(d => d.producto.idProducto === id);
        if (item.cantidad < item.producto.stock) {
            item.cantidad++;
            item.subTotal = item.cantidad * item.producto.precio;
            renderSeleccionados();
        } else {
            mostrarMensaje("Stock insuficiente para este producto.", "warning");
        }
    }

    function disminuirCantidad(id) {
        let itemIndex = seleccionados.findIndex(d => d.producto.idProducto === id);
        if (itemIndex > -1) {
            let item = seleccionados[itemIndex];
            if (item.cantidad > 1) {
                item.cantidad--;
                item.subTotal = item.cantidad * item.producto.precio;
            } else {
                seleccionados.splice(itemIndex, 1);
            }
        }
        renderSeleccionados();
    }

    function eliminarSeleccionado(id) {
        seleccionados = seleccionados.filter(item => item.producto.idProducto !== id);
        renderSeleccionados();
    }

    function eliminarDelCarrito(id) {
        carrito = carrito.filter(item => item.producto.idProducto !== id);
        renderCarrito();
    }

    // Eventos de escucha
    document.addEventListener('click', (e) => {
        if (e.target.closest('button')) {
            const button = e.target.closest('button');
            const id = parseInt(button.dataset.id, 10);

            if (button.textContent.trim() === 'Agregar' || button.parentElement.textContent.trim() === 'Agregar') {
                agregarSeleccionado(id);
            } else if (button.classList.contains('btn-disminuir')) {
                disminuirCantidad(id);
            } else if (button.classList.contains('btn-aumentar')) {
                aumentarCantidad(id);
            } else if (button.classList.contains('btn-eliminar-seleccionado')) {
                eliminarSeleccionado(id);
            } else if (button.classList.contains('btn-eliminar-carrito')) {
                eliminarDelCarrito(id);
            }
        }
    });

    document.addEventListener('click', (e) => {
        if (e.target.id === 'btnPasarAlCarrito') {
            pasarAlCarrito();
        }
        if (e.target.id === 'btnFinalizarVenta') {
            finalizarVenta();
        }
    });

    // Evento de escucha para el campo de búsqueda
    inputBusqueda.addEventListener("input", filtrarProductos);

    // Carga inicial de productos y renderizado
    cargarProductos().then(() => {
        renderSeleccionados();
        renderCarrito();
    });
});