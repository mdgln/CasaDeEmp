using CasaDeEmpeño.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasaDeEmpeño.Controllers
{
    public class DevolucionController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: Devolucion
        public ActionResult Index()
        {
            List<Devolucion> devoluciones = GetDevoluciones();
            return View(devoluciones);
        }

        public ActionResult Agregar()
        {
            try
            {
                //// Obtener productos disponibles, que estén dentro del tiempo para solicitar devolución
                ProductoController productoController = new ProductoController();
                List<Producto> productos = productoController.GetProductos();

                OfertaController ofertaController = new OfertaController();
                List<Oferta> ofertas = ofertaController.GetOfertas();

                List<Devolucion> devoluciones = GetDevoluciones();

                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM SettingDevolucion";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                TimeSpan tiempo = new TimeSpan();

                while (reader.Read())
                {
                    tiempo = TimeSpan.FromSeconds(reader.GetInt64(0));
                }

                command.Connection.Close();

                // Obtener productos dentro de fecha para solicitud de devolución
                var productosFechaDevolucionVencida = productos
                                                    .Select(p => p)
                                                    .Where(p => DateTime.Now <= p.FechaIngreso.Add(tiempo));

                // Se filtra la lista anterior dejando los productos que no tengan una devolución hecha
                var productosSinDevolucion = productosFechaDevolucionVencida
                                            .Select(p => p)
                                            .Where(p => !devoluciones.Select(d => d.ProductoId).Contains(p.Id));

                // Obtener el total de ofertas realizadas por producto y mostrar solo las que tengan un total menor a 3
                var productosSinOfertasDisponibles = ofertas
                                                    .GroupBy(o => o.ProductoId)
                                                    .Select(g => new { ProductoId = g.Key, Cantidad = g.Count() })
                                                    .Where(p => p.Cantidad >= 3)
                                                    .ToList();

                // Mostrar solo productos que cumplan con todas las características anteriores
                ViewBag.Productos = productosSinDevolucion
                                    .Select(p => p)
                                    .Where(p => !productosSinOfertasDisponibles.Select(pr => pr.ProductoId).Contains(p.Id));

                return View();
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Agregar(Devolucion devolucion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"INSERT INTO Devolucion VALUES ({devolucion.ProductoId}, '{devolucion.Comentario}') ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    int resultado = command.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        TempData["Agregado"] = "Registro agregado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(devolucion);
                    }
                }
                catch
                {
                    devolucion.MensajeError = "Ocurrió un error al agregar el registro. Intente de nuevo, por favor.";
                    return View(devolucion);
                }
            }
            else
            {
                return Agregar();
            }
        }

        public ActionResult Ver(int id)
        {
            try
            {
                ProductoController productoController = new ProductoController();
                List<Producto> productos = productoController.GetProductos();

                Devolucion devolucion = null;

                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"SELECT * FROM Devolucion WHERE Id = {id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    devolucion = new Devolucion()
                    {
                        Id = reader.GetInt32(0),
                        ProductoId = reader.GetInt32(1),
                        Comentario = reader.GetString(2)
                    };
                }

                ViewBag.Producto = productos.Where(p => p.Id == devolucion.ProductoId);

                if (devolucion != null)
                {
                    return View(devolucion);
                }
                else
                {
                    devolucion.MensajeError = "Ocurrió un error al intentar visualizar el registro. Intente de nuevo, por favor.";
                    return View(devolucion);
                }
            }
            catch
            {
                return View("Error");
            }
        }

        public List<Devolucion> GetDevoluciones()
        {
            List<Devolucion> devoluciones = new List<Devolucion>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                //string query = "SELECT * FROM Devolucion ";
                string query = "SELECT A.*, B.Nombre " +
                                "FROM Devolucion A " +
                                "INNER JOIN Producto B ON A.ProductoId = B.Id ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Devolucion devolucion = new Devolucion()
                    {
                        Id = reader.GetInt32(0),
                        ProductoId = reader.GetInt32(1),
                        Comentario = reader.GetString(2),
                        NombreProducto = reader.GetString(3)
                    };

                    devoluciones.Add(devolucion);
                }
                return devoluciones;
            }
            catch
            {
                return devoluciones;
            }
        }

    }
}