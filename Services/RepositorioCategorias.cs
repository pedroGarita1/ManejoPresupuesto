using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categorias categoria);
        Task BorrarCat(int id);
        Task<Categorias> Buscar(int id, int idUsuario);
        Task Crear(Categorias categorias);
        Task<IEnumerable<Categorias>> Obtener(int idUsuario);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(Categorias categorias)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categoria (idUsuario, IdTipoOperacion, Nombre) VALUES (@idUsuario, @IdTipoOperacion, @Nombre); SELECT SCOPE_IDENTITY()", new { categorias.IdUsuario, categorias.IdTipoOperacion, categorias.Nombre });
            categorias.Id = id;
        }
        public async Task<IEnumerable<Categorias>> Obtener(int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categorias>(@"SELECT * FROM Categoria Where IdUsuario = @IdUsuario", new { idUsuario });
        }
        public async Task<Categorias> Buscar(int id,int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categorias>(@"SELECT * FROM Categoria Where Id = @Id AND IdUsuario = @IdUsuario ", new { id, idUsuario });
        }
        public async Task Actualizar (Categorias categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categoria SET IdTipoOperacion = @IdTipoOperacion, Nombre = @Nombre WHERE Id = @Id", categoria);
        }
        public async Task BorrarCat (int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categoria WHERE Id = @Id", new { id });
        }
    }
}
