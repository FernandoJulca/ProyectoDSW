using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Data.Repositorios;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Controllers
{
    public class ProovedorController : Controller
    {

        private readonly ICrud<Proveedor> proveRepo;
        private readonly ICrud<Distrito> distritoRepo;
        public ProovedorController( ICrud<Proveedor> proveedorRepo, ICrud<Distrito>distritoRepo) 
        {
            this.proveRepo = proveedorRepo;
            this.distritoRepo = distritoRepo;
        }

        public IActionResult Index()
        {
            var lista = proveRepo.ListaCompleta();
            return View(lista);
        }

        public Proveedor provEncontrado(int id) {
            return proveRepo.ObtenerId("detalle",id);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewBag.distritos = new SelectList(distritoRepo.ListaCompleta(), "idDistrito", "nombre");
            return View(new Proveedor());
        }

        [HttpPost]
        public IActionResult Create(Proveedor provedor) {

 
            int registrado = proveRepo.Registrar("registrar", provedor);
            TempData["exitoCrear"] = ($"El usuario {registrado} fue registrado.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            ViewBag.distritos = new SelectList(distritoRepo.ListaCompleta(), "idDistrito", "nombre");
            Proveedor proveObtenido = provEncontrado(id);

            return View(proveObtenido);
        }

        [HttpPost]
        public IActionResult Edit(Proveedor provedor) {

            bool actualizado = proveRepo.Actualizar("actualizar", provedor);

            if (actualizado)
            {
                TempData["exitoActualizar"] = ($"El proveedor con codigo {provedor.idProveedor} se actualizo");
                return RedirectToAction("Index");
            }
            else {
                TempData["errorActualizar"] = ($"El proveedor con codigo {provedor.idProveedor} no se pudo actualizar");
                return View(provedor);
            }
            
        }

        public IActionResult Details(int id) { 
            return View(provEncontrado(id));
        }

        [HttpGet]
        public IActionResult Delete(int id) { 
            return View(provEncontrado(id));
        }

        [HttpPost, ActionName("Delete")]

        public IActionResult Delete_Confirmar(int id)
        {

            bool eliminado = proveRepo.Eliminar("desactivar", id);
            if (eliminado)
            {
                TempData["exitoEliminar"] = ($"El proveedor con codigo {id} fue desactivado");
                return RedirectToAction("Index");
            }
            else {
                TempData["errorEliminar"] = ($"El proveedor con codigo {id} no se pudo eliminar");
                return View(provEncontrado(id));
            }
        
        }
    }
}
