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
    public class TipoProductoController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: TipoProducto
        public ActionResult Index()
        {
            List<TipoProducto> tiposProducto = GetTiposProducto();
            return View(tiposProducto);
        }
        
        public ActionResult Agregar()
        {
            return View(new TipoProducto());
        }

        [HttpPost]
        public ActionResult Agregar(TipoProducto tipo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"INSERT INTO TipoProducto VALUES ('{tipo.Descripcion}', {(tipo.Estatus ? 1 : 0)}) ";

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
                        return View(tipo);
                    }
                }
                catch
                {
                    tipo.MensajeError = "Ocurrió un error al agregar el registro. Intente de nuevo, por favor.";
                    return View(tipo);
                }
            }
            else
            {
                return View(tipo);
            }
        }

        public ActionResult Editar(int id)
        {
            TipoProducto tipo = GetTipoProductoById(id);
            return (tipo != null) ? View(tipo) : View("Error");
        }

        [HttpPost]
        public ActionResult Editar(TipoProducto tipo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = $"UPDATE TipoProducto " +
                        $"SET Descripcion = '{tipo.Descripcion}', Estatus = {(tipo.Estatus ? 1 : 0)} " +
                        $"WHERE Id = {tipo.Id} ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Connection.Open();

                    command.ExecuteNonQuery();
                    TempData["Editado"] = "Registro editado exitosamente";
                    return RedirectToAction("Index");
                }
                catch
                {
                    tipo.MensajeError = "Ocurrió un error al editar el registro. Intente de nuevo, por favor.";
                    return View(tipo);
                }
            }
            else
            {
                return View(tipo);
            }
        }

        public ActionResult Borrar(int id)
        {
            TipoProducto tipo = GetTipoProductoById(id);
            return (tipo != null) ? View(tipo) : View("Error");
        }

        [HttpDelete]
        public ActionResult Borrar(TipoProducto tipo)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"DELETE FROM TipoProducto WHERE Id = {tipo.Id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                command.ExecuteReader();
                TempData["Borrado"] = "Registro borrado exitosamente";
                return RedirectToAction("Index");
            }
            catch
            {
                tipo.MensajeError = "Existen productos registrados con este tipo.";
                return View(tipo);
            }
        }

        public ActionResult Ver(int id)
        {
            TipoProducto tipo = GetTipoProductoById(id);
            return (tipo != null) ? View(tipo) : View("Error");
        }


        public List<TipoProducto> GetTiposProducto()
        {
            List<TipoProducto> tiposProducto = new List<TipoProducto>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM TipoProducto ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TipoProducto tipo = new TipoProducto()
                    {
                        Id = reader.GetInt32(0),
                        Descripcion = reader.GetString(1),
                        Estatus = reader.GetBoolean(2)
                    };

                    tiposProducto.Add(tipo);
                }
                return tiposProducto;
            }
            catch
            {
                return tiposProducto;
            }
        }

        public TipoProducto GetTipoProductoById(int id)
        {
            TipoProducto tipo = null;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = $"SELECT * FROM TipoProducto WHERE Id = {id} ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tipo = new TipoProducto()
                    {
                        Id = reader.GetInt32(0),
                        Descripcion = reader.GetString(1),
                        Estatus = reader.GetBoolean(2)
                    };
                }
                return tipo;
            }
            catch
            {
                return tipo;
            }
        }

    }
}