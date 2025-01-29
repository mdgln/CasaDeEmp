using CasaDeEmpeño.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasaDeEmpeño.Controllers
{
    public class ProductoController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: Producto
        public ActionResult Index()
        {
            List<Producto> productos = GetProductos();

            OfertaController ofertaController = new OfertaController();
            List<Oferta> ofertas = ofertaController.GetOfertas();

            DevolucionController devolucionController = new DevolucionController();
            List<Devolucion> devoluciones = devolucionController.GetDevoluciones();

            productos.ForEach(p =>
            {
                if (ofertas.Select(o => o.ProductoId).Contains(p.Id) || devoluciones.Select(d => d.ProductoId).Contains(p.Id))
                {
                    p.Editable = 0;
                }
                else
                {
                    p.Editable = 1;
                }
            });

            return View(productos);
        }

        public ActionResult Agregar()
        {
            EstadoProductoController estadoProductoController = new EstadoProductoController();
            List<EstadoProducto> estadosProducto = estadoProductoController.GetEstadosProducto();

            TipoProductoController tipoProductoController = new TipoProductoController();
            List<TipoProducto> tiposProducto = tipoProductoController.GetTiposProducto();

            ViewBag.Estados = estadosProducto.Where(e => e.Estatus);
            ViewBag.Tipos = tiposProducto.Where(t => t.Estatus);

            return View();
        }

        [HttpPost]
        public ActionResult Agregar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"INSERT INTO Producto VALUES ('{producto.Nombre}', {producto.EstadoId}, '{producto.FechaIngreso.ToString("yyyy-MM-dd")}', {producto.Valor}, {producto.TipoId}) ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    int resultado = command.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        TempData["Agregado"] = "Producto creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(producto);
                    }
                }
                catch
                {
                    producto.MensajeError = "Ocurrió un error al agregar el registro. Intente de nuevo, por favor.";
                    return View(producto);
                }
            } 
            else
            {
                return Agregar();
            }
        }

        public ActionResult Editar(int id)
        {
            Producto producto = GetProductoById(id);

            EstadoProductoController estadoProductoController = new EstadoProductoController();
            List<EstadoProducto> estadosProducto = estadoProductoController.GetEstadosProducto();

            TipoProductoController tipoProductoController = new TipoProductoController();
            List<TipoProducto> tiposProducto = tipoProductoController.GetTiposProducto();

            ViewBag.Estados = estadosProducto.Where(e => e.Estatus);
            ViewBag.Tipos = tiposProducto.Where(t => t.Estatus);

            return (producto != null) ? View(producto) : View("Error");
        }

        [HttpPost]
        public ActionResult Editar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"UPDATE Producto " +
                        $"SET Nombre = '{producto.Nombre}', EstadoId = {producto.EstadoId}, FechaIngreso = '{producto.FechaIngreso.ToString("yyyy-MM-dd")}', " +
                        $"Valor = {producto.Valor}, TipoId = {producto.TipoId} " +
                        $"WHERE Id = {producto.Id} ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    command.ExecuteReader();
                    TempData["Editado"] = "Producto editado exitosamente";
                    return RedirectToAction("Index");
                }
                catch
                {
                    producto.MensajeError = "Ocurrió un error al editar el registro. Intente de nuevo, por favor.";
                    return View(producto);
                }
            }
            else
            {
                return Agregar();
            }
        }

        public ActionResult Borrar(int id)
        {
            Producto producto = GetProductoById(id);

            EstadoProductoController estadoProductoController = new EstadoProductoController();
            List<EstadoProducto> estadosProducto = estadoProductoController.GetEstadosProducto();

            TipoProductoController tipoProductoController = new TipoProductoController();
            List<TipoProducto> tiposProducto = tipoProductoController.GetTiposProducto();

            ViewBag.Estados = estadosProducto.Where(e => e.Id == producto.EstadoId);
            ViewBag.Tipos = tiposProducto.Where(t => t.Id == producto.TipoId);

            return (producto != null) ? View(producto) : View("Error");
        }

        [HttpDelete]
        public ActionResult Borrar(Producto producto)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"DELETE FROM Producto WHERE Id = {producto.Id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                command.ExecuteReader();
                TempData["Borrado"] = "Producto borrado exitosamente";
                return RedirectToAction("Index");
            }
            catch
            {
                producto.MensajeError = "Ocurrió un error al borrar el registro. Intente de nuevo, por favor.";
                return View(producto);
            }
        }

        public ActionResult Ver(int id)
        {
            Producto producto = GetProductoById(id);

            EstadoProductoController estadoProductoController = new EstadoProductoController();
            List<EstadoProducto> estadosProducto = estadoProductoController.GetEstadosProducto();

            TipoProductoController tipoProductoController = new TipoProductoController();
            List<TipoProducto> tiposProducto = tipoProductoController.GetTiposProducto();

            ViewBag.Estados = estadosProducto.Where(e => e.Id == producto.EstadoId);
            ViewBag.Tipos = tiposProducto.Where(t => t.Id == producto.TipoId);

            return (producto != null) ? View(producto) : View("Error");
        }

        public List<Producto> GetProductos()
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM Producto ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Producto producto = new Producto()
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        EstadoId = reader.GetInt32(2),
                        FechaIngreso = reader.GetDateTime(3),
                        Valor = reader.GetDecimal(4),
                        TipoId = reader.GetInt32(5)
                    };

                    productos.Add(producto);
                }

                return productos;
            }
            catch
            {
                return productos;
            }
        }

        public Producto GetProductoById(int id)
        {
            Producto producto = null;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"SELECT * FROM Producto WHERE Id = {id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    producto = new Producto()
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        EstadoId = reader.GetInt32(2),
                        FechaIngreso = reader.GetDateTime(3),
                        Valor = reader.GetDecimal(4),
                        TipoId = reader.GetInt32(5)
                    };
                }
                return producto;
            }
            catch
            {
                return producto;
            }
        }

    }
}