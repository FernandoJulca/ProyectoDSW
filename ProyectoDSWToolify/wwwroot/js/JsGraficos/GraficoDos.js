document.addEventListener("DOMContentLoaded", () => {
    fetch("https://localhost:7021/api/Grafico/CategoriaProducto") // url de tu API
        .then(response => {
            if (!response.ok) {
                console.log("Error no se obtuvo la data", response);
            }
            return response.json();
        })
        .then(data => {
            console.log("Obtuviste data de la API", data);

            const categorias = data.map(item => item.descripcion);
            const totalProductos = data.map(item => item.totalProductos);

            const seriesData = categorias.map((cat, i) => ({

                name: cat,
                y: totalProductos[i]
            }));

            Highcharts.chart('container2', {
                chart: {
                    type: 'pie',
                    backgroundColor: '#FFFFFF',
                    plotBackgroundColor: '#FFFFFF',
                    height: 500,
                    zooming: {
                        type: 'xy'
                    },
                    panning: {
                        enabled: true,
                        type: 'xy'
                    },
                    panKey: 'shift'
                },
                title: {
                    text: 'Productos Por categoria',
                    style: {
                        color: '#000000'
                    }
                },
                tooltip: {
                    valueSuffix: ' unidades'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: [{
                            enabled: true,
                            distance: 20
                        }, {
                            enabled: true,
                            distance: -40,
                            format: '{point.percentage:.1f}%',
                            style: {
                                fontSize: '1.2em',
                                textOutline: 'none',
                                opacity: 0.7
                            },
                            filter: {
                                operator: '>',
                                property: 'percentage',
                                value: 10
                            }
                        }]
                    }
                },
                series: [
                    {
                        name: 'Productos',
                        colorByPoint: true,
                        data: seriesData
                    }
                ]
            });
        })
        .catch(error => {
            console.error("Error al obtener datos:", error);
        });
});
