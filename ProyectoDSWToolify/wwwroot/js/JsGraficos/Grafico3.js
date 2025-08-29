document.addEventListener("DOMContentLoaded", () => {
    fetch("https://localhost:7021/api/Grafico/VentaPorMes")
        .then(response => {
            if (!response.ok) {
                console.error("Error no se obtuvo la data", response);
            }
            return response.json();
        })
        .then(data => {
            console.log("Obtuviste data de la API", data);

            // propiedades reales que devuelve tu API
            const meses = data.map(item => item.mes);
            const ventas = data.map(item => item.ventasTotales);

            Highcharts.chart('container3', {
                chart: {
                    type: 'line' 
                },
                title: {
                    text: 'Ventas del año 2025'
                },
                xAxis: {
                    categories: meses,
                    title: {
                        text: 'Mes'
                    }
                },
                yAxis: {
                    type: 'logarithmic', 
                    title: {
                        text: 'Ventas Totales (escala logarítmica)'
                    },
                    minorTickInterval: 0.1
                },
                tooltip: {
                    headerFormat: '<b>{point.key}</b><br/>',
                    pointFormat: '{point.y} ventas'
                },
                series: [{
                    name: 'Ventas',
                    data: ventas,
                    color: '#2caffe'
                }]
            });
        })
        .catch(err => console.error("Error en fetch:", err));
});
