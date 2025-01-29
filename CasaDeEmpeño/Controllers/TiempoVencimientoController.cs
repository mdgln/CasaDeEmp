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
    public class TiempoVencimientoController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;

        // GET: TiempoVencimiento
        public ActionResult Editar()
        {
            try
            {
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

                TiempoVencimiento tiempoVencimiento = new TiempoVencimiento()
                {
                    Tiempo = $"{(int)tiempo.TotalHours:D2}:{tiempo.Minutes:D2}:{tiempo.Seconds:D2}"
                };

                command.Connection.Close();

                return View(tiempoVencimiento);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Editar(TiempoVencimiento tiempo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TimeSpan tiempoSpan;
                    if (TimeSpan.TryParse(tiempo.Tiempo, out tiempoSpan))
                    {
                        SqlConnection connection = new SqlConnection(connectionString);

                        string query = $"UPDATE SettingDevolucion " +
                            $"SET Tiempo = {(long)tiempoSpan.TotalSeconds}";

                        SqlCommand command = new SqlCommand(query, connection);

                        command.Connection.Open();

                        command.ExecuteNonQuery();

                        TempData["Exito"] = "Tiempo actualizado exitosamente";
                        return RedirectToAction("Editar");
                    }
                    else
                    {
                        ModelState.AddModelError("Tiempo", "Formato inválido. Use HH:mm:ss");
                    }
                }
                catch
                {
                    return View("Error");
                }
            }
            return View(tiempo);
        }
    }
}