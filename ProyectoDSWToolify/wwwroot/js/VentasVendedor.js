document.addEventListener("DOMContentLoaded", () => {
    let productos = [];
    let seleccionados = [];
    let carrito = [];

    const apiUrl = "/api/Producto"; // cambia según tu endpoint
    const apiVentaUrl = "/api/venta";

    const tablaProductos = document.getElementById("tablaProductos");
    const contenedorSeleccionados = document.getElementById("contenedorSeleccionados");
    const contenedorCarrito = document.getElementById("contenedorCarrito");
    const countSeleccionados = document.getElementById("countSeleccionados");
    const countCarrito = document.getElementById("countCarrito");

    // Cargar productos del backend
    async function cargarProductos() {
        try {
            const resp = await fetch(apiUrl);
            productos = await resp.json();
            renderProductos();
        } catch (err) {
            console.error("Error cargando productos", err);
        }
    }

    function renderProductos() {
        tablaProductos.innerHTML = productos.map(p => `
            <tr>
                <td>${p.idProducto}</td>
                <td>${p.nombre}</td>
                <td>${p.categoria.descripcion}</td>
                <td>${p.precio}</td>
                <td>${p.stock}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="agregarSeleccionado(${p.idProducto})">
                        Agregar
                    </button>
                </td>
            </tr>
        `).join("");
    }

    window.agregarSeleccionado = (id) => {
        const producto = productos.find(p => p.idProducto === id);
        const existente = seleccionados.find(d => d.producto.idProducto === id);

        if (existente) {
            existente.cantidad++;
            existente.subTotal = existente.cantidad * existente.producto.precio;
        } else {
            seleccionados.push({
                producto,
                cantidad: 1,
                subTotal: producto.precio
            });
        }
        renderSeleccionados();
    };

    function renderSeleccionados() {
        if (seleccionados.length === 0) {
            contenedorSeleccionados.innerHTML = `<p class="text-center text-muted py-4">No hay productos seleccionados</p>`;
        } else {
            contenedorSeleccionados.innerHTML = `
                <table class="table table-striped">
                    <thead><tr><th>Nombre</th><th>Precio</th><th>Cantidad</th><th>Subtotal</th></tr></thead>
                    <tbody>
                        ${seleccionados.map(item => `
                            <tr>
                                <td>${item.producto.nombre}</td>
                                <td>${item.producto.precio}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-secondary" onclick="disminuirCantidad(${item.producto.idProducto})">-</button>
                                    ${item.cantidad}
                                    <button class="btn btn-sm btn-outline-secondary" onclick="aumentarCantidad(${item.producto.idProducto})">+</button>
                                </td>
                                <td>${item.subTotal}</td>
                            </tr>
                        `).join("")}
                    </tbody>
                </table>
                <div class="d-flex justify-content-end">
                    <button class="btn btn-success" onclick="pasarAlCarrito()">Agregar a Venta</button>
                </div>
            `;
        }
        countSeleccionados.textContent = `${seleccionados.length} productos`;
    }

    window.aumentarCantidad = (id) => {
        const item = seleccionados.find(d => d.producto.idProducto === id);
        if (item.cantidad < item.producto.stock) {
            item.cantidad++;
            item.subTotal = item.cantidad * item.producto.precio;
            renderSeleccionados();
        } else {
            alert("Stock insuficiente");
        }
    };

    window.disminuirCantidad = (id) => {
        const item = seleccionados.find(d => d.producto.idProducto === id);
        if (item.cantidad > 1) {
            item.cantidad--;
            item.subTotal = item.cantidad * item.producto.precio;
        } else {
            seleccionados = seleccionados.filter(d => d.producto.idProducto !== id);
        }
        renderSeleccionados();
    };

    window.pasarAlCarrito = () => {
        carrito.push(...seleccionados);
        seleccionados = [];
        renderSeleccionados();
        renderCarrito();
    };

    function renderCarrito() {
        if (carrito.length === 0) {
            contenedorCarrito.innerHTML = `<p class="text-center text-muted py-4">El carrito está vacío</p>`;
        } else {
            const total = carrito.reduce((acc, d) => acc + d.subTotal, 0);
            contenedorCarrito.innerHTML = `
                <table class="table table-bordered">
                    <thead><tr><th>Producto</th><th>Cant.</th><th>Subtotal</th></tr></thead>
                    <tbody>
                        ${carrito.map(item => `
                            <tr>
                                <td>${item.producto.nombre}</td>
                                <td>${item.cantidad}</td>
                                <td>${item.subTotal}</td>
                            </tr>
                        `).join("")}
                    </tbody>
                </table>
                <h4 class="text-end">Total: ${total}</h4>
                <div class="d-flex justify-content-end">
                    <button class="btn btn-success" onclick="finalizarVenta()">Finalizar Venta</button>
                </div>
            `;
        }
        countCarrito.textContent = `${carrito.length} productos`;
    }

    window.finalizarVenta = async () => {
        if (carrito.length === 0) {
            alert("El carrito está vacío");
            return;
        }

        const venta = {
            idVenta: 0,
            usuario: { idUsuario: 3 }, // se puede ajustar según sesión
            detalles: carrito,
            total: carrito.reduce((acc, d) => acc + d.subTotal, 0)
        };

        try {
            const resp = await fetch(apiVentaUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(venta)
            });

            if (resp.ok) {
                alert("Venta realizada con éxito");
                carrito = [];
                renderCarrito();
            } else {
                alert("Error al finalizar venta");
            }
        } catch (err) {
            console.error("Error en la venta", err);
        }
    };

    cargarProductos();
});
