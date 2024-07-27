using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Libreria.Models;
using MySql.Data.MySqlClient;



namespace Libreria.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = "Server=Local;Database=Libreria;User ID=root;Password=root;";
      

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Registrar(USUARIO oUSUARIO)
        {
            bool registrado;
            string mensaje;

            if (oUSUARIO.Clave == oUSUARIO.ConfirmarClave)
            {

            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (MySqlConnection cn = new MySqlConnection(cadena))
            {
                MySqlCommand cmd = new MySqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUSUARIO.Correo);
                cmd.Parameters.AddWithValue("cLAVE", oUSUARIO.Clave);
                cmd.Parameters.Add("Registrado", MySqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", MySqlDbType.VarChar,255).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);

                mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                
            }
            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login","Acceso");
            }
            else
            {
                return View();

            }

        }


        [HttpPost]
        public ActionResult Login(USUARIO oUSUARIO)
        {

            oUSUARIO.Clave = (oUSUARIO.Clave);

            using (MySqlConnection cn = new MySqlConnection(cadena))
            {
                MySqlCommand cmd = new MySqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUSUARIO.Correo);
                cmd.Parameters.AddWithValue("cLAVE", oUSUARIO.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();


               oUSUARIO.Idusuario = Convert.ToInt32( cmd.ExecuteScalar().ToString());

            }

            if(oUSUARIO.Idusuario != 0)

            {
                Session["usuario"] = oUSUARIO;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "usuario no encontrado";

                return View();
            }
        }
    }
}
