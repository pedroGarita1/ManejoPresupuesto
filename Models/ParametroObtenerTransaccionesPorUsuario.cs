﻿namespace ManejoPresupuesto.Models
{
    public class ParametroObtenerTransaccionesPorUsuario
    {
        public int IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
