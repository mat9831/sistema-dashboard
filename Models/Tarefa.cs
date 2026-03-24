namespace SistemaLoginDemo.Models
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao {  get; set; }
        public string Status { get; set; }
        public int UsuarioId { get; set; }
    }
}
