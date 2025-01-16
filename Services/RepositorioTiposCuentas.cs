using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string Nombre, int IdUsuario);
        Task<IEnumerable<TipoCuenta>> Obtener(int idUsuario);
        Task<TipoCuenta> ObtenerPorId(int id, int idUsuario);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("sp_TiposCuentas_Insertar", new { idUsuario= tipoCuenta.IdUsuario, nombre = tipoCuenta.Nombre }, commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string Nombre, int IdUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM Tipo_Cuenta WHERE Nombre = @Nombre AND IdUsuario = @IdUsuario;", new { Nombre, IdUsuario});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, IdUsuario, Nombre, Orden, Create_at FROM Tipo_Cuenta WHERE IdUsuario = @IdUsuario ORDER BY Orden", new {idUsuario});
        }
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Tipo_Cuenta SET Nombre = @Nombre WHERE Id = @Id", tipoCuenta);
        }
        public async Task<TipoCuenta> ObtenerPorId(int id, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, IdUsuario, Nombre, Orden, Create_at FROM Tipo_Cuenta WHERE Id = @Id AND IdUsuario = @IdUsuario", new {id, idUsuario});
        }
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Tipo_Cuenta WHERE Id = @Id", new { id });
        }
        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE Tipo_Cuenta SET ORDEN = @Orden Where Id = @Id";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
