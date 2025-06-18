using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Behaviours
{
    public class ValidationBehaivours<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaivours(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationresult = await Task.WhenAll(_validators.Select(f => f.ValidateAsync(context, cancellationToken)));
                var failures = validationresult.SelectMany(x => x.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                    throw new Exceptions.ValidationException(failures);
            }

            return await next();
        }
    }
}
