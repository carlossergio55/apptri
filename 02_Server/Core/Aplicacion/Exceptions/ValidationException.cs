using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace Aplicacion.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; set; }
       
        public ValidationException() : base("Se han producido uno o más errores de validación")
        {            
            Errors = new List<string>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }
        }

    }
}
