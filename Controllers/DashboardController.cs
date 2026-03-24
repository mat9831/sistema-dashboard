using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SistemaLoginDemo.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _config;
        public DashboardController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            int usuarioId = Convert.ToInt32(HttpContext.Session.GetInt32("UsuarioId"));

            int total = 0;
            int pendentes = 0;
            int concluidas = 0;

            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
    SELECT
        COUNT(*) AS Total,
        ISNULL(SUM(CASE WHEN Status = 'Pendente' THEN 1 ELSE 0 END),0) AS Pendentes,
        ISNULL(SUM(CASE WHEN Status = 'Concluída' THEN 1 ELSE 0 END),0) AS Concluidas
    FROM Tarefas
    WHERE UsuarioId = @UsuarioId", conn);

                // ESTA LINHA É OBRIGATÓRIA
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    total = Convert.ToInt32(reader["Total"]);
                    pendentes = Convert.ToInt32(reader["Pendentes"]);
                    concluidas = Convert.ToInt32(reader["Concluidas"]);
                }
            }

            ViewBag.Total = total;
            ViewBag.Pendentes = pendentes;
            ViewBag.Concluidas = concluidas;

            return View();
        }
    }



}
