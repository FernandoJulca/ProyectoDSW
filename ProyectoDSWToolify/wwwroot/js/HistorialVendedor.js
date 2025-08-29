document.addEventListener("DOMContentLoaded", () => {
    cargarDatosTotales();
    cargarVentas();
});


fetch("/Vendedor/DatosTotales", { cache: "no-store" })
    .then(res => res.json())
    .then(data => {
        document.getElementById("totalVentas").textContent = data.totalVentas;
        document.getElementById("totalProductos").textContent = data.totalProductosVendidos;
        document.getElementById("ingresosTotales").textContent = `S/ ${data.ingresosTotales.toFixed(2)}`;
    })
    .catch(err => console.error("Error al cargar datos totales:", err));


async function cargarVentas() {
    try {
        const ventas = await fetch("/Vendedor/Historial").then(r => r.json());
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