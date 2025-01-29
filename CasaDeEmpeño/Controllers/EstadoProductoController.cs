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
    public class EstadoProductoController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: EstadoProducto
        public ActionResult Index()
        {
            List<EstadoProducto> estadosProducto = GetEstadosProducto();
            return View(estadosProducto);
        }


        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(EstadoProducto estado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"INSERT INTO EstadoProducto VALUES ('{estado.Descripcion}', {(estado.Estatus ? 1 : 0)}) ";

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
                        return View(estado);
                    }
                }
                catch
                {
                    estado.MensajeError = "Ocurrió un error al crear el registro. Intente de nuevo, por favor.";
                    return View(estado);
                }
            } 
            else
            {
                return View(estado);
            }
        }


        public ActionResult Editar(int id)
        {
            EstadoProducto estadoProducto = GetEstadoProductoById(id);
            return (estadoProducto != null) ? View(estadoProducto) : View("Error");
        }

        [HttpPost]
        public ActionResult Editar(EstadoProducto estado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"UPDATE EstadoProducto " +
                        $"SET Descripcion = '{estado.Descripcion}', Estatus = {(estado.Estatus ? 1 : 0)} " +
                        $"WHERE Id = {estado.Id} ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    command.ExecuteNonQuery();
                    TempData["Editado"] = "Registro editado exitosamente";
                    return RedirectToAction("Index");
                }
                catch
                {
                    estado.MensajeError = "Ocurrió un error al editar el registro. Intente de nuevo, por favor.";
                    return View(estado);
                }
            }
            else
            {
                return View(estado);
            }
        }


        public ActionResult Borrar(int id)
        {
            EstadoProducto estadoProducto = GetEstadoProductoById(id);
            return (estadoProducto != null) ? View(estadoProducto) : View("Error");
        }

        [HttpDelete]
        public ActionResult Borrar(EstadoProducto estado)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"DELETE FROM EstadoProducto WHERE Id = {estado.Id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                int resultado = command.ExecuteNonQuery();
                TempData["Borrado"] = "Registro borrado exitosamente";
                return RedirectToAction("Index");
            }
            catch
            {
                estado.MensajeError = "Existen productos registrados que cuentan con este estado.";
                return View(estado);
            }
        }

        public ActionResult Ver(int id)
        {
            EstadoProducto estadoProducto = GetEstadoProductoById(id);
            return (estadoProducto != null) ? View(estadoProducto) : View("Error");
        }

        public List<EstadoProducto> GetEstadosProducto()
        {
            List<EstadoProducto> estadosProducto = new List<EstadoProducto>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM EstadoProducto ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    EstadoProducto estado = new EstadoProducto()
                    {
                        Id = reader.GetInt32(0),
                        Descripcion = reader.GetString(1),
                        Estatus = reader.GetBoolean(2)
                    };

                    estadosProducto.Add(estado);
                }
                return estadosProducto;
            }
            catch
            {
                return estadosProducto;
            }
        }

        public EstadoProducto GetEstadoProductoById(int id)
        {
            EstadoProducto estadoProducto = null;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"SELECT * FROM EstadoProducto WHERE Id = {id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    estadoProducto = new EstadoProducto()
                    {
                        Id = reader.GetInt32(0),
                        Descripcion = reader.GetString(1),
                        Estatus = reader.GetBoolean(2)
                    };

                }
                return estadoProducto;
            }
            catch
            {
                return estadoProducto;
            }
        }

    }
}