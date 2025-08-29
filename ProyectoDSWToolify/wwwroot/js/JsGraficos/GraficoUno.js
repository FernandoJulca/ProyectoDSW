document.addEventListener("DOMContentLoaded", () => {
    fetch("https://localhost:7021/api/Grafico/VentaPorMesAndTipoVenta") // url de tu API
        .then(response => {
            if (!response.ok) {
                console.log("Error no se obtuvo la data", response);
            }
            return response.json();
        })
        .then(data => {
            console.log("Obtuviste data de la API", data);

            const meses = [...new Set(data.map(item => item.mes))];
            const tipos = [...new Set(data.map(item => item.tipoVenta))];
           
            const seriesData = tipos.map(tipo => {
                return {
                    name: tipo === 'R' ? "Repartidor" : "Presencial",
                    data: meses.map(mes => {
                        const registro = data.find(d => d.mes === mes && d.tipoVenta === tipo);
                       
                        return registro ? registro.cantidadVentas : 0;
                    })
                };
            });
            Highcharts.chart('container1', {
                chart: {
                    type: 'spline'
                },
                title: {
                    text: 'Ventas Por Repartidor/Presencial'
                },
                xAxis: {
                    categories: meses,
                    accessibility: {
                        description: 'Meses del año'
                    }
                },
                yAxis: {
                    title: {
                        text: 'Cantidad de Ventas'
                    }
                },
                tooltip: {
                    shared: true,
                    crosshairs: true
                },
                plotOptions: {
                    spline: {
                        marker: {
                            radius: 4,
                            lineColor: '#666666',
                            lineWidth: 1
                        }
                    }
                },
                series: seriesData
            }

            );

        })
        .catch(error => {
            console.error("Error en la petición:", error);
        });
});

