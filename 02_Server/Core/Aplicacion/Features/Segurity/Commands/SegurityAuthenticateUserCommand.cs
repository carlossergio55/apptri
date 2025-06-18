using Aplicacion.DTOs.Segurity;
using Aplicacion.Interfaces.Repositories;
using Aplicacion.Wrappers;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Segurity.Commands
{
    public class SegurityAuthenticateUserCommand : IRequest<Response<SegUsuarioDto>>
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
    }

    public class SegurityAuthenticateUserCommandHandler : IRequestHandler<SegurityAuthenticateUserCommand, Response<SegUsuarioDto>>
    {
        private readonly ISegurityRepository _repository;
        public SegurityAuthenticateUserCommandHandler(ISegurityRepository repositoryAsync)
        {
            _repository = repositoryAsync;
        }

        public async Task<Response<SegUsuarioDto>> Handle(SegurityAuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var data = await _repository.ValidateUserLoguin(request.Usuario, request.Password, request.IpAddress);
            return new Response<SegUsuarioDto>(data);
        }
    }

    public class CreateAutenticateUserCommandValidator : AbstractValidator<SegurityAuthenticateUserCommand>
    {
        public CreateAutenticateUserCommandValidator()
        {
            RuleFor(p => p.Usuario)
                .NotEmpty().WithMessage("{PropertyName} no pude ser vacio")
                .MaximumLength(120).WithMessage("{PropertyName} no debe exceder de {MaxLength} caracteres");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} no pude ser vacio")
                .MinimumLength(4).WithMessage("{PropertyName} no debe ser menor de {MinLength} caracteres");
        }
    }

}
