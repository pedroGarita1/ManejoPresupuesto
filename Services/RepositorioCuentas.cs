using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int idUsuario);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int idUsuario);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string ConnectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuenta (IdTipoCuenta, Nombre, Descripcion, Balance) VALUES (@IdTipoCuenta, @Nombre, @Descripcion, @Balance); SELECT SCOPE_IDENTITY()", cuenta);
            cuenta.Id = id;
        }
        public async Task<IEnumerable<Cuenta>> Buscar(int idUsuario)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT c.Id, c.Nombre, c.Balance, c.Create_at, tc.Nombre AS TipoCuenta
                                                            FROM Cuenta AS c 
                                                            INNER JOIN Tipo_Cuenta AS tc ON tc.Id = c.IdTipoCuenta 
                                                            WHERE tc.IdUsuario = @IdUsuario ORDER BY tc.Orden", new {idUsuario});
        }
        public async Task<Cuenta> ObtenerPorId(int id, int idUsuario)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT c.Id, c.Nombre, c.Balance, c.Descripcion, tc.id
                                                            FROM Cuenta AS c 
                                                            INNER JOIN Tipo_Cuenta AS tc ON tc.Id = c.IdTipoCuenta 
                                                            WHERE tc.IdUsuario = @IdUsuario AND C.Id = @Id", new {idUsuario,id});
        }
        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(@"UPDATE Cuenta SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, IdTipoCuenta = @IdTipoCuenta WHERE Id = @Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(@"DELETE Cuenta WHERE Id = @Id", new {id});
        }
    }
}
