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
    public class LoginController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: Login
        public ActionResult Login()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult Login(Usuario user)
        {

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);
                string contrasenaHash = HashHelper.GetMD5Hash(user.Password);

                string query = $"SELECT * FROM Usuarios WHERE Username = '{user.Username}' AND Password = '{contrasenaHash}' ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    user.MensajeError = "Usuario o contraseña incorrectos";
                    return View(user);
                }


                command.Connection.Close();

                return RedirectToAction("Index", "Producto");
            }
        }

        public ActionResult Crear()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult Crear(Usuario user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string contrasenaHash = HashHelper.GetMD5Hash(user.Password);

                string query = $"SELECT * FROM Usuarios WHERE Username = '{user.Username}' ";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user.MensajeError = "Ya existe el usuario ingresado. Intente con uno distinto.";
                    return View(user);
                }
                else
                {
                    command.Connection.Close();

                    query = $"INSERT INTO Usuarios VALUES ('{user.Username}', '{contrasenaHash}') ";

                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Connection.Open();

                    int result = sqlCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        sqlCommand.Connection.Close();
                        return RedirectToAction("Index", "Producto");
                    }
                    else
                    {
                        user.MensajeError = "Ocurrió un error al crear el usuario. Intente de nuevo, por favor.";
                        return View(user);
                    }
                }
            }
        }

    }
}