document.addEventListener("DOMContentLoaded", () => {

});

const botonesDetalle = document.querySelectorAll('.btn-detalle-venta');
const modalBody = document.querySelector('#detalleVentaModal .modal-body');
const modalTitle = document.querySelector('#detalleVentaModal .modal-title');

fetch("/Vendedor/DatosTotales", { cache: "no-store" })
    .then(res => res.json())
    .then(data => {
        document.getElementById("totalVentas").textContent = data.totalVentas;
        document.getElementById("totalProductos").textContent = data.totalProductosVendidos;
        document.getElementById("ingresosTotales").textContent = `S/ ${data.ingresosTotales.toFixed(2)}`;
    })
    .catch(err => console.error("Error al cargar datos totales:", err));

botonesDetalle.forEach(boton => {
    boton.addEventListener('click', function () {

        const idVenta = this.getAttribute('data-idventa');

        modalBody.innerHTML = '<p class="text-center text-muted">Cargando detalles de la venta...</p>';
        modalTitle.textContent = 'Detalle de Venta...';

        fetch(`/Vendedor/DetalleVenta?idVenta=${idVenta}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error al obtener los datos de la venta.');
                }
                return response.json();
            })
            .then(data => {
                modalTitle.textContent = `Detalle de Venta #${data.idVenta}`;

                modalBody.innerHTML = '';

                const estadoTexto = data.estado === "G" ? "Generado" :
                    data.estado === "T" ? "Transportado" :
                        data.estado === "E" ? "Entregado" :
                            data.estado === "P" ? "Pendiente" :
                                data.estado === "C" ? "Cancelado" :
                                    "Desconocido";

                const estadoClase = data.estado === "G" ? "bg-warning" :
                    data.estado === "T" ? "bg-info" :
                        data.estado === "E" ? "bg-success" :
                            data.estado === "C" ? "bg-danger" :
                                "bg-secondary";

                const htmlContent = `
                        <p><strong>Vendedor:</strong> ${data.usuario.nombre}</p>
                        <p><strong>Documento:</strong> ${data.usuario.nroDoc}</p>
                        <p><strong>Fecha:</strong> ${new Date(data.fecha).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })}</p>
                        <p><strong>Total:</strong> S/. ${data.total.toFixed(2)}</p>
                        <p>
                            <strong>Estado:</strong>
                            <span class="badge ${estadoClase} text-white">${estadoTexto}</span>
                        </p>
                        <hr>
                        <h6>Productos</h6>
                        <table class="table table-sm table-striped">
                            <thead>
                                <tr>
                                    <th>Producto</th>
                                    <th>Cantidad</th>
                                    <th>Subtotal</th>
                                </tr>
                            </thead>
                            <tbody id="detalles-productos">
                                <!-- Los detalles de los productos se insertarán aquí dinámicamente -->
                            </tbody>
                        </table>
                    `;

                modalBody.innerHTML = htmlContent;

                const tbody = document.getElementById('detalles-productos');
                if (data.detalles && data.detalles.length > 0) {
                    data.detalles.forEach(det => {
                        const row = document.createElement('tr');

                        row.innerHTML = `
                                <td>${det.producto.nombre}</td>
                                <td>${det.cantidad}</td>
                                <td>S/. ${det.subTotal.toFixed(2)}</td>
                            `;
                        tbody.appendChild(row);
                    });
                } else {

                    const row = document.createElement('tr');
                    row.innerHTML = `<td colspan="3" class="text-muted text-center">No hay detalles de productos para esta venta.</td>`;
                    tbody.appendChild(row);
                }
            })
            .catch(error => {
                console.error('Error:', error);

                modalBody.innerHTML = `<p class="text-center text-danger">Ocurrió un error: ${error.message}</p>`;
            });
    });
});

