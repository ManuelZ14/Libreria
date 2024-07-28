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

       string cadena = "Server=127.0.0.1;Database=Libreria;User ID=root;Password=root;";
      

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

            if (oUSUARIO.Clave != oUSUARIO.ConfirmarClave)
            {
                ViewData["pMensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            
            

            using (MySqlConnection cn = new MySqlConnection(cadena))
            {
                MySqlCommand cmd = new MySqlCommand("sp_RegistrarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("pCorreo", oUSUARIO.Correo);
                cmd.Parameters.AddWithValue("pClave", oUSUARIO.Clave);
                cmd.Parameters.Add(new MySqlParameter("pRegistrado", MySqlDbType.Bit));
                cmd.Parameters["pRegistrado"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new MySqlParameter("pMensaje", MySqlDbType.VarChar, 255));
                cmd.Parameters["pMensaje"].Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["pRegistrado"].Value);

                mensaje = cmd.Parameters["pMensaje"].Value.ToString();

                
            }
            ViewData["pMensaje"] = mensaje;

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
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("pCorreo", oUSUARIO.Correo);
                cmd.Parameters.AddWithValue("pClave", oUSUARIO.Clave);
             

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
