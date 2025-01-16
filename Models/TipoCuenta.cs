using ManejoPresupuesto.Validate;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta //: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimerLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int IdUsuario { get; set; }
        public int Orden { get; set; }
        public DateTime Create_at { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Nombre != null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if(primeraLetra != primeraLetra.ToUpper())
        //        {
        //            // Nota: Si no se agrega el nombre del modelo se aplicara este error para todo el modelo
        //            //       Y se aplicara al validation-summary = ModelOnly
        //            yield return new ValidationResult("La primera letra debe ser Mayúscula", new[] { nameof(Nombre) });
        //        }
        //    }
        //}
    }
}
