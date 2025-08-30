using Azure;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Controllers
{
    [Authorize(Roles = "A")]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration inyect)
        {
            this._configuration = inyect;
        }

        #region Json

        private async Task<List<ListadoVentaFechaAndTipoVentaDTO>> ListadoVentaFechaAndTipoVenta(
            DateTime? fechaInicio, DateTime? fechaFin, string? tipo = null)
        {
            var listado = new List<ListadoVentaFechaAndTipoVentaDTO>();
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);
                var Url = $"reportes/ListadoPorMesAndTipoVenta?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                if (!string.IsNullOrEmpty(tipo)) { 
                    Url += $"&tipo={tipo}";
                }
                var msg = await httpCliente.GetAsync(Url);
                var data = await msg.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoVentaFechaAndTipoVentaDTO>>(data);
            }
            return listado;
        }

        private async Task<List<ListarProductosPorCategoriaDTO>> ListarProductosPorCategoria(
          int? idCategoria, string?orden )
        {
            var listado = new List<ListarProductosPorCategoriaDTO>();
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);
                var Url = $"reportes/ListarProductosPorCategoria?idCategoria={idCategoria}";
                if (!string.IsNullOrEmpty(orden))
                {
                    Url += $"&orden={orden}";
                }
                var msg = await httpCliente.GetAsync(Url);
                var data = await msg.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListarProductosPorCategoriaDTO>>(data);
            }
            return listado;
        }
        private async Task<List<Categoria>> ListaCategoria()
        {
            var listaCategoria = new List<Categoria>();

            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await httpCliente.GetAsync("Categoria");
                var data = await msg.Content.ReadAsStringAsync();

                listaCategoria = JsonConvert.DeserializeObject<List<Categoria>>(data);
            }
            return listaCategoria;
        }
        #endregion


        

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Reporte1(DateTime? fechaInicio , DateTime? fechaFin, string? tipo ,int pag = 1) {

            #region FILTROS
            var listado = ListadoVentaFechaAndTipoVenta(fechaInicio,fechaFin,tipo).Result;

            var FechaInicioParseado = fechaInicio?.ToString("yyyy-MM-dd");
            var FechaFinParseado = fechaFin?.ToString("yyyy-MM-dd");


            if (!listado.Any())
            {
                TempData["ErrorListado"] = ($"No hay Ventas para el rango de fecha {FechaInicioParseado}/{FechaFinParseado}");
                listado = ListadoVentaFechaAndTipoVenta(null, null, null).Result;
            }
            else {
                string mensaje = ($" desde {(fechaInicio.HasValue && fechaFin.HasValue ? $"{FechaInicioParseado}/{FechaFinParseado}":"")}");

                var mensajeAlert = ($"Filtrando ventas {mensaje} ");
                if (!string.IsNullOrEmpty(tipo)) {
                    mensajeAlert += $" tipo de venta {tipo}";
                }
                TempData["ExitoListado"] = mensajeAlert;
            }
            #endregion

            #region ExportToExcel
            if (Request.Query.ContainsKey("export") && Request.Query["export"] == "excel")
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Ventas");

                    var headerRange = worksheet.Range("A1:G1");
                    headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Font.Bold = true;

                    worksheet.Cell(1, 1).Value = "ID Venta";
                    worksheet.Cell(1, 2).Value = "Fecha";
                    worksheet.Cell(1, 3).Value = "Cliente";
                    worksheet.Cell(1, 4).Value = "Direccion";
                    worksheet.Cell(1, 5).Value = "total";
                    worksheet.Cell(1, 6).Value = "Estado";
                    worksheet.Cell(1, 6).Value = "Tipo Venta";
                    int fila = 2;
                    foreach (var venta in listado)
                    {
                        worksheet.Cell(fila, 1).Value = venta.idVenta;
                        worksheet.Cell(fila, 2).Value = venta.fechaGenerada.ToString("yyyy-MM-dd");
                        worksheet.Cell(fila, 3).Value = venta.nombresCompletos;
                        worksheet.Cell(fila, 4).Value = venta.direccion;
                        worksheet.Cell(fila, 5).Value = venta.total;
                      

                        string estado = "";
                        var celdaEstado = worksheet.Cell(fila, 6);
                        switch (venta.estado.ToString()) {
                            case "G":
                                estado = "Generado";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 235, 59);
                                celdaEstado.Style.Font.FontColor = XLColor.FromArgb(51, 51, 51);

                                break;
                            case "P":
                                estado = "Pendiente";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 152, 0);
                                celdaEstado.Style.Font.FontColor = XLColor.White;
                                break;
                            case "C":
                                estado= "Cancelado";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(244, 67, 54); 
                                celdaEstado.Style.Font.FontColor = XLColor.White;
                                break;
                            case "T":
                                estado= "Transportando";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(33, 150, 243);
                                celdaEstado.Style.Font.FontColor = XLColor.White;
                                break;
                            case "E":
                                estado = "Entregado";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(76, 175, 80);
                                celdaEstado.Style.Font.FontColor = XLColor.White;
                                break;
                            default:
                                estado = "ERROR";
                                celdaEstado.Style.Fill.BackgroundColor = XLColor.FromArgb(158, 158, 158);
                                celdaEstado.Style.Font.FontColor = XLColor.White;
                                break;
                        }

                        celdaEstado.Value = estado;
                        celdaEstado.Style.Font.Bold = true;

                        worksheet.Cell(fila, 6).Value = estado;
                        worksheet.Cell(fila, 7).Value = venta.tipoVenta == "P" ? "Presencial" : "Remota";
                        fila++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"ReporteVentas_{DateTime.Now:yyyyMMdd}.xlsx");
                    }
                }
            }
            #endregion

            #region ExportToPdf
            if (Request.Query.ContainsKey("export") && Request.Query["export"] == "pdf") { 
                using (var ms = new MemoryStream()) {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(document, ms);
                document.Open();
                

                var titulo = new Paragraph("Reportes de Ventas ") { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20f };
                document.Add(titulo);

                PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 10f, 20f, 20f, 15f, 10f, 10f, 15f });

                var headers = new[] { "ID", "Cliente", "Dirección", "Fecha", "Total", "Estado", "Tipo" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }


                    foreach (var venta in listado)
                    {
                        table.AddCell(venta.idVenta.ToString());
                        table.AddCell(venta.nombresCompletos ?? "");
                        table.AddCell(venta.direccion ?? "");
                        table.AddCell(venta.fechaGenerada.ToString("yyyy-MM-dd"));
                        table.AddCell(venta.total.ToString());

                        BaseColor colorEstado = BaseColor.WHITE;

                        string estado;
                        switch (venta.estado.ToString())
                        {
                            case "G":
                                estado = "Generado";
                                { colorEstado = new BaseColor(255, 235, 59); }

                                break;
                            case "P":
                                estado = "Pendiente";
                                { colorEstado = new BaseColor(255, 152, 0); }
                                break;
                            case "C":
                                estado = "Cancelado";
                                { colorEstado = new BaseColor(244, 67, 54); }
                                break;
                            case "T":
                                estado = "Transportando";
                                { colorEstado = new BaseColor(33, 150, 243); }
                                break;
                            case "E":
                                estado = "Entregado";
                                { colorEstado = new BaseColor(76, 175, 80); }
                                break;
                            default:
                                estado = "ERROR";
                                { colorEstado = new BaseColor(158, 158, 158); }
                                break;
                        }
                        PdfPCell estadoCell = new PdfPCell(new Phrase(estado)) { BackgroundColor = colorEstado };
                        table.AddCell(estadoCell);

                        string tipoTexto = venta.tipoVenta == "P" ? "Presencial" : "Remota";
                        table.AddCell(new PdfPCell(new Phrase(tipoTexto)));

                       
                    }
                    document.Add(table);
                    document.Close();
                    return File(ms.ToArray(), "application/pdf", $"ReporteVentas_{DateTime.Now:yyyyMMdd}.pdf");
                }
            }
            #endregion
            #region Paginación
            var paginasTotales = listado.Count;
            var paginasMax = 10;
            var numeroPag = (int)Math.Ceiling((double)paginasTotales / paginasMax);
            ViewBag.pagActual = pag;
            ViewBag.numeroPag = numeroPag;
            var skip = (pag - 1) * paginasMax;
            #endregion

            return View(listado.Skip(skip).Take(paginasMax));
        }

        public IActionResult Reporte2(int? idCategoria, string? orden, int pag = 1) 
        {
            #region FILTROS
            var categorias = ListaCategoria().Result;
            ViewBag.Categorias = new SelectList(ListaCategoria().Result, "idCategoria", "descripcion", idCategoria);
            Categoria categoriObtenid = new Categoria();

            var nombreCategoria = categorias.FirstOrDefault(x => x.idCategoria == idCategoria)?.descripcion;
            var listadp = ListarProductosPorCategoria(idCategoria, orden).Result;

            if (!listadp.Any()) {
                TempData["ErrorListado"] = ($"No hay productos para la cat. {nombreCategoria}");
                listadp = ListarProductosPorCategoria(null, null).Result;
            }

            TempData["ExitoListado"] = ($"Mostrando {listadp.Count} productos Cat. {nombreCategoria}");
            #endregion

            #region ExportToExcel
            #endregion

            #region ExportToPdf
            #endregion



            #region PAGINACIÓN
            var paginasTotales = listadp.Count;
            var paginasMax = 10;
            var numeroPag = (int)Math.Ceiling((double)paginasTotales / paginasMax);
            ViewBag.pagActual = pag;
            ViewBag.numeroPag = numeroPag;
            var skip = (pag - 1) * paginasMax;
            #endregion

            return View(listadp.Skip(skip).Take(paginasMax));
        }
    }
}
