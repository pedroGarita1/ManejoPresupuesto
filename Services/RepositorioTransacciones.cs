using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
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
    }
}
