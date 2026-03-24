using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaLoginDemo.Models;


namespace SistemaLoginDemo.Controllers
{
    public class TarefasController : Controller
    {
        private readonly IConfiguration _config;

        public TarefasController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            List<Tarefa> tarefas = new List<Tarefa>();

            string connStr = _config.GetConnectionString("DefaultConnection");

            int usuarioId = Convert.ToInt32(HttpContext.Session.GetInt32("UsuarioId"));

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM Tarefas WHERE UsuarioId = @UsuarioId";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tarefas.Add(new Tarefa
                        {
                            Id = (int)reader["Id"],
                            Titulo = reader["Titulo"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            Status = reader["Status"].ToString(),
                            UsuarioId = (int)reader["UsuarioId"]
                        });
                    }
                }
            }

            return View(tarefas);
        }
        
        
        public IActionResult Criar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            int usuarioId = Convert.ToInt32(HttpContext.Session.GetInt32("UsuarioId"));

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"INSERT INTO Tarefas 
                       (Titulo, Descricao, Status, UsuarioId) 
                       VALUES 
                       (@Titulo, @Descricao, 'Pendente', @UsuarioId)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Titulo", tarefa.Titulo);
                    cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Concluir(int id)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "UPDATE Tarefas SET Status = 'Concluída' WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Editar(int id)
        {
            Tarefa tarefa = new Tarefa();

            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM Tarefas WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    tarefa.Id = Convert.ToInt32(reader["Id"]);
                    tarefa.Titulo = reader["Titulo"].ToString();
                    tarefa.Descricao = reader["Descricao"].ToString();
                    tarefa.Status = reader["Status"].ToString();
                }
            }

            return View(tarefa);
        }
        [HttpPost]
        public IActionResult Editar(Tarefa tarefa)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE Tarefas 
                       SET Titulo=@Titulo,
                           Descricao=@Descricao,
                           Status=@Status
                       WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Titulo", tarefa.Titulo);
                cmd.Parameters.AddWithValue("@Descricao", tarefa.Descricao);
                cmd.Parameters.AddWithValue("@Status", tarefa.Status);
                cmd.Parameters.AddWithValue("@Id", tarefa.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Excluir(int id)
        {
            string connStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "DELETE FROM Tarefas WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}