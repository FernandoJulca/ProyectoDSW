document.addEventListener("DOMContentLoaded", () => {

    fetch("https://localhost:7021/api/Grafico/ProveedorProducto") // url de tu API
        .then(response => {
            if (!response.ok) {
                console.log("Error no se obtuvo la data", response);
            }
            return response.json();
        })
        .then(data => {
            console.log("Obtuviste data de la API", data);

            const chartData = data.map(item => ({
                name: item.razonSocial,
                y: item.totalProductos,
                z: item.totalProductos
            }));

            Highcharts.chart('container5', {
                chart: {
                    type: 'variablepie'
                },
                title: {
                    text: 'Distribución de productos por proveedor'
                },
                tooltip: {
                    headerFormat: '',
                    pointFormat: `
                <span style="color:{point.color}">\u25CF</span>
                <b>{point.name}</b><br/>
                Total productos: <b>{point.y}</b><br/>
            `
                },
                series: [{
                    minPointSize: 10,
                    innerSize: '20%',
                    zMin: 0,
                    name: 'Proveedores',
                    borderRadius: 5,
                    data: chartData,
                    colors: Highcharts.getOptions().colors // usa la paleta por defecto
                }]
            })
        })

   

    
});



