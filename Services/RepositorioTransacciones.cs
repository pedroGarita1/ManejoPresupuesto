using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int idUsuario);
        Task<IEnumerable<Transaccion>> ObtenerPorIdUsuario(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("sp_Transacciones_Insertar",
                new
                {
                    transaccion.IdUsuario,
                    transaccion.IdCuenta,
                    transaccion.IdCategoria,
                    transaccion.Nota,
                    transaccion.Monto,
                    transaccion.FechaTransaccion
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }
        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int idCuentaAnterior)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("sp_Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.IdCuenta,
                    idCuentaAnterior,
                    transaccion.IdCategoria,
                    transaccion.Nota,
                    transaccion.Monto,
                    montoAnterior,
                    transaccion.FechaTransaccion
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("sp_Transacciones_Eliminar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task <IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre AS Cuenta, c.IdTipoOperacion
                FROM Transacciones t 
                INNER JOIN Categoria c ON c.Id = t.IdCategoria 
                INNER JOIN Cuenta cu ON cu.Id = t.IdCuenta 
                WHERE t.IdCuenta = @IdCuenta 
                AND t.IdUsuario = @IdUsuario 
                AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }
        public async Task<IEnumerable<Transaccion>> ObtenerPorIdUsuario(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre AS Cuenta, c.IdTipoOperacion
                FROM Transacciones t 
                INNER JOIN Categoria c ON c.Id = t.IdCategoria 
                INNER JOIN Cuenta cu ON cu.Id = t.IdCuenta 
                WHERE t.IdUsuario = @IdUsuario 
                AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY t.FechaTransaccion DESC", modelo);
        }
        public async Task<Transaccion> ObtenerPorId(int id, int idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT t.*, cat.IdTipoOperacion FROM Transacciones t INNER JOIN Categoria cat ON cat.Id = t.IdCategoria WHERE t.Id = @Id AND t.IdUsuario = @IdUsuario", new { id, idUsuario });
        }
    }
}
