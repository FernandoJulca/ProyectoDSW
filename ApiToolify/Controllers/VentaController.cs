using System.Reflection.Metadata;
using ApiToolify.Models.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;
using Document = iTextSharp.text.Document;

namespace ApiToolify.Controllers
{
    [Route("api/venta")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVenta ventarepo;
        private readonly IHubContext<ApiToolify.ChatHub.ChatHub> _hubContext;

        public VentaController(IVenta ventarepo, IHubContext<ApiToolify.ChatHub.ChatHub> hubContext)
        {
            this.ventarepo = ventarepo;
            _hubContext = hubContext;
        }

        [HttpPost("confirmar-compra")]
        public IActionResult ConfirmarCompra([FromBody] VentaDTO venta)
        {
            try
            {
                var resultado = ventarepo.generarVentaCliente(venta);
                return Ok(new { mensaje = "Compra realizada con éxito", idVenta = resultado.idVenta });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error: " + ex.Message });
            }
        }

        [HttpPost("venta-generada")]
        public IActionResult GenerarVenta([FromBody] VentaDTO venta)
        {
            try
            {
                var resultado = ventarepo.generarVentaVendedor(venta);
                return Ok(new { mensaje = "Compra realizada con éxito", idVenta = resultado.idVenta });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error: " + ex.Message });

            }
        }
                
        [HttpGet("ventasPendientesyTransportada")]
        public IActionResult VentasPendientesR()
        {
            try
            {
                var ventasPendientes = ventarepo.obtenerVentasRemota();
                return Ok(ventasPendientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ventasPendientes")]
        public IActionResult VentasPendientesRepartidor()
        {
            try
            {
                var ventasPendientes = ventarepo.obtenerVentasRemotaPendientes();
                return Ok(ventasPendientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ventas/{idCliente}/pdf/{idVenta}")]
        public IActionResult DescargarVentaPdf(int idCliente, int idVenta)
        {
            var venta = ventarepo.obtenerVentaPorUsuario(idVenta, idCliente);
            if (venta == null)
                return NotFound("Venta no encontrada.");

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var colorPrimary = new BaseColor(87, 110, 170);
                var colorPrimaryDark = new BaseColor(74, 93, 148);
                var colorGray500 = new BaseColor(107, 114, 128);
                var colorGray700 = new BaseColor(55, 65, 81);

                var empresaFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, colorPrimary);
                var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, colorPrimary);
                var textoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, colorGray700);
                var textoGrisFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, colorGray500);
                var headerTableFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);

                string estadoTexto = venta.tipoVenta == "P" ? "Presencial" :
                                     venta.tipoVenta == "R" ? "Remota" : "Estado desconocido";

                string tipoRol = venta.usuario.rol.descripcion == "V" ? "Vendedor" :
                                venta.usuario.rol.descripcion == "C" ? "Cliente" : "Rol desconocido";

                var tituloEmpresa = new Paragraph("Toolify", empresaFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(tituloEmpresa);

                var bienvenida = new Paragraph("Gracias por su compra!", tituloFont) { Alignment = Element.ALIGN_CENTER };
                doc.Add(bienvenida);

                var subtitulo = new Paragraph("Detalles de la venta", textoGrisFont) { Alignment = Element.ALIGN_CENTER };
                doc.Add(subtitulo);

                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph($"Venta ID: {venta.idVenta}", tituloFont));
                doc.Add(new Paragraph($"{tipoRol}: {venta.usuario.nombre} {venta.usuario.apePaterno} {venta.usuario.apeMaterno}", textoFont));
                doc.Add(new Paragraph($"Dirección: {venta.usuario.direccion}", textoFont));
                doc.Add(new Paragraph($"Fecha: {venta.fecha:dd/MM/yyyy HH:mm}", textoFont));
                doc.Add(new Paragraph($"Tipo de venta: {estadoTexto}", textoFont));
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 50, 15, 20, 15 });

                foreach (var header in new[] { "Producto", "Cantidad", "Precio Unitario", "Subtotal" })
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerTableFont))
                    {
                        BackgroundColor = colorPrimaryDark,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                // Filas con datos
                foreach (var detalle in venta.Detalles)
                {
                    table.AddCell(new Phrase(detalle.producto.nombre, textoFont));
                    table.AddCell(new Phrase(detalle.cantidad.ToString(), textoFont));
                    table.AddCell(new Phrase($"{detalle.producto.precio:C}", textoFont));
                    table.AddCell(new Phrase($"{detalle.subTotal:C}", textoFont));
                }

                doc.Add(table);

                doc.Add(new Paragraph(" "));

                var totalParagraph = new Paragraph($"Total a pagar: {venta.total:C}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, colorPrimary))
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                doc.Add(totalParagraph);

                doc.Add(new Paragraph(" "));

                var gracias = new Paragraph("¡Gracias por confiar en nosotros! Esperamos volver a servirle pronto.", textoGrisFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(gracias);

                doc.Close();

                var bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"venta_{venta.idVenta}.pdf");
            }
        }

        [HttpPost("cambiarEstado")]
        public async Task<IActionResult> AceptarVenta(int idVenta)
        {
            try
            {
                ventarepo.CambiarEstadoVenta(idVenta, "T");
                await _hubContext.Clients.Group(idVenta.ToString()).SendAsync("VentaAceptada", idVenta, "El repartidor ha aceptado tu pedido.");
                return Ok(new { mensaje = "Estado actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("generarEstado")]
        public async Task<IActionResult> GenerarVenta(int idVenta)
        {
            try
            {

                ventarepo.CambiarEstadoVenta(idVenta, "E");
                await _hubContext.Clients.Group(idVenta.ToString()).SendAsync("EntregaConfirmada", idVenta, "Tu pedido ha sido confirmado y entregado.");
                return Ok(new { mensaje = "Estado generado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("cancelarEstado")]
        public async Task<IActionResult> CancelarVenta(int idVenta)
        {
            try
            {
                ventarepo.CambiarEstadoVenta(idVenta, "C");
                await _hubContext.Clients.Group(idVenta.ToString()).SendAsync("VentaCancelada", idVenta, "Tu pedido ha sido cancelado.");
                return Ok(new { mensaje = "Estado cancelado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("pendientescount")]
        public IActionResult ContarPendientesRemotas()
        {
            try
            {
                int total = ventarepo.ContarRemotas("P");
                return Ok(new { totalPendientes = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("entregadascount")]
        public IActionResult ContarEntregadasRemotas()
        {
            try
            {
                int total = ventarepo.ContarRemotas("E");
                return Ok(new { totalEntregadas = total });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { mensaje = argEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
