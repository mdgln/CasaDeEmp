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
    public class OfertaController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: Oferta
        public ActionResult Index()
        {
            List<Oferta> ofertas = GetOfertas();
            return View(ofertas);
        }


        public ActionResult Agregar()
        {
            try
            {
                //// Obtener productos disponibles, que estén dentro del tiempo para solicitar devolución
                ProductoController productoController = new ProductoController();
                List<Producto> productos = productoController.GetProductos();

                DevolucionController devolucionController = new DevolucionController();
                List<Devolucion> devoluciones = devolucionController.GetDevoluciones();

                List<Oferta> ofertas = GetOfertas();

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

                // Obtener productos fuera de fecha para solicitud de devolución
                var productosFechaDevolucionVencida = productos
                                                    .Select(p => p)
                                                    .Where(p => DateTime.Now > p.FechaIngreso.Add(tiempo));

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
        public ActionResult Agregar(Oferta oferta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"INSERT INTO Oferta VALUES ('{oferta.NombrePersona}', '{oferta.NumeroCelular}', {oferta.Monto}, {oferta.ProductoId}) ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    int resultado = command.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        TempData["ProductoId"] = oferta.ProductoId;
                        TempData["Agregado"] = "Registro agregado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(oferta);
                    }
                }
                catch
                {
                    return View("Error");
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

                Oferta oferta = null;

                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"SELECT * FROM Oferta WHERE Id = {id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    oferta = new Oferta()
                    {
                        Id = reader.GetInt32(0),
                        NombrePersona = reader.GetString(1),
                        NumeroCelular = reader.GetString(2),
                        Monto = reader.GetDecimal(3),
                        ProductoId = reader.GetInt32(4)
                    };
                }

                ViewBag.Producto = productos.Where(p => p.Id == oferta.ProductoId);

                if (oferta != null)
                {
                    return View(oferta);
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        public List<Oferta> GetOfertas()
        {
            List<Oferta> ofertas = new List<Oferta>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                //string query = "SELECT * FROM Oferta ";
                string query = "SELECT A.*, B.Nombre, B.Valor " +
                                "FROM Oferta A " +
                                "INNER JOIN Producto B ON A.ProductoId = B.Id ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Oferta oferta = new Oferta()
                    {
                        Id = reader.GetInt32(0),
                        NombrePersona = reader.GetString(1),
                        NumeroCelular = reader.GetString(2),
                        Monto = reader.GetDecimal(3),
                        ProductoId = reader.GetInt32(4),
                        NombreProducto = reader.GetString(5),
                        PrecioProducto = reader.GetDecimal(6)
                    };

                    ofertas.Add(oferta);
                }
                return ofertas;
            }
            catch
            {
                return ofertas;
            }
        }

    }
}