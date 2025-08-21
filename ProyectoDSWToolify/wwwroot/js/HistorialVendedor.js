document.addEventListener("DOMContentLoaded", () => {
    cargarDatosTotales();
    cargarVentas();
});

async function cargarDatosTotales() {
    try {
        const [ventas, productos, ingresos] = await Promise.all([
            fetch("Reporte/totalVentas").then(r => r.json()),
            fetch("Reporte/totalProductosVendidos").then(r => r.json()),
            fetch("Reporte/ingresosTotales").then(r => r.json())
        ]);

        document.getElementById("totalVentas").textContent = ventas;
        document.getElementById("totalProductos").textContent = productos;
        document.getElementById("ingresosTotales").textContent = `S/ ${ingresos.toFixed(2)}`;
    } catch (error) {
        console.error("Error cargando datos totales:", error);
    }
}

async function cargarVentas() {
    try {
        const ventas = await fetch("/Reporte/ventas").then(r => r.json());
        const tbody = document.getElementById("tablaVentas");
        tbody.innerHTML = "";

        ventas.forEach(v => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td class="font-monospace">#${v.id}</td>
                <td>${new Date(v.fecha).toLocaleDateString()}</td>
                <td class="fw-medium">${v.cliente}</td>
                <td>${v.vendedor}</td>
                <td><span class="badge bg-light text-dark">${v.productos} productos</span></td>
                <td class="fw-bold">S/. ${v.total.toFixed(2)}</td>
                <td><span class="badge ${v.estado === "Completada" ? "bg-success" : "bg-warning"} text-white">${v.estado}</span></td>
                <td>
                    <button class="btn btn-outline-secondary btn-sm me-1"><i class="bi bi-eye"></i></button>
                    <button class="btn btn-outline-secondary btn-sm"><i class="bi bi-download"></i></button>
                </td>
            `;
            tbody.appendChild(tr);
        });

        document.getElementById("cantidadVentas").textContent = `${ventas.length} resultados`;
    } catch (error) {
        console.error("Error cargando ventas:", error);
    }
}