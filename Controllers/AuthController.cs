using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaLoginDemo.Models;
using System.Data;

namespace SistemaLoginDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login, string senha)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("spLoginUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Senha", senha);

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // pega o ID do usuário vindo do banco
                    int usuarioId = Convert.ToInt32(reader["Id"]);
                    string nome = reader["Nome"].ToString();

                    // salva na sessão
                    HttpContext.Session.SetInt32("UsuarioId", usuarioId);
                    HttpContext.Session.SetString("UsuarioNome", nome);

                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Erro = "Login inválido";
            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(Usuario usuario)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("spCriarUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@Email", usuario.Email);
                cmd.Parameters.AddWithValue("@Login", usuario.Login);
                cmd.Parameters.AddWithValue("@Senha", usuario.Senha);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["sucesso"] = "Usuário criado com sucesso";

            return RedirectToAction("Login");
        }
    }
    
    
}