using Checkpoint05Crud.Models;
using Microsoft.Data.SqlClient;
using Checkpoint05Crud.Services;

namespace Checkpoint05Crud.Repositories
{
    public class ProdutoRepository
    {
        private readonly string _connectionString;

        public ProdutoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Inserir(Produto produto)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string sql = @"INSERT INTO Produtos (Nome, Preco, Estoque, Categoria)
                           VALUES (@Nome, @Preco, @Estoque, @Categoria)";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Nome", produto.Nome);
            command.Parameters.AddWithValue("@Preco", produto.Preco);
            command.Parameters.AddWithValue("@Estoque", produto.Estoque);
            command.Parameters.AddWithValue("@Categoria", produto.Categoria);

            connection.Open();
            command.ExecuteNonQuery();
            LogService.Registrar($"Produto inserido: {produto.Nome}");
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = new List<Produto>();

            using SqlConnection connection = new SqlConnection(_connectionString);

            string sql = "SELECT Id, Nome, Preco, Estoque, Categoria FROM Produtos";

            using SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Produto produto = new Produto
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Preco = reader.GetDecimal(2),
                    Estoque = reader.GetInt32(3),
                    Categoria = reader.GetString(4)
                };

                produtos.Add(produto);
            }
            LogService.Registrar("Produtos listados.");
            return produtos;
        }

        public Produto? BuscarPorId(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string sql = @"SELECT Id, Nome, Preco, Estoque, Categoria
                           FROM Produtos
                           WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Produto produto = new Produto
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Preco = reader.GetDecimal(2),
                    Estoque = reader.GetInt32(3),
                    Categoria = reader.GetString(4)
                };

                LogService.Registrar($"Produto ID {id} consultado.");

                return produto;
            }

            LogService.Registrar($"Busca realizada para ID {id}: não encontrado.");

            return null;
        }

        public void Atualizar(Produto produto)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string sql = @"UPDATE Produtos
                           SET Nome = @Nome,
                               Preco = @Preco,
                               Estoque = @Estoque,
                               Categoria = @Categoria
                           WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", produto.Id);
            command.Parameters.AddWithValue("@Nome", produto.Nome);
            command.Parameters.AddWithValue("@Preco", produto.Preco);
            command.Parameters.AddWithValue("@Estoque", produto.Estoque);
            command.Parameters.AddWithValue("@Categoria", produto.Categoria);

            connection.Open();
            command.ExecuteNonQuery();
            LogService.Registrar($"Produto ID {produto.Id} atualizado.");
        }

        public void Excluir(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string sql = "DELETE FROM Produtos WHERE Id = @Id";

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
            LogService.Registrar($"Produto ID {id} excluído.");
        }
    }
}