using Aplicacion.DTOs.Segurity;
using Aplicacion.Interfaces.Repositories;
using Aplicacion.Wrappers;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Segurity.Commands
{
    public class SegurityAuthenticateUserSystemCommand : IRequest<Response<SegUsuarioDto>>
    {
        public string Usuario { get; set; }
        public string IpAddress { get; set; }
    }

    public class SegurityAuthenticateUserSystemCommandHandler : IRequestHandler<SegurityAuthenticateUserSystemCommand, Response<SegUsuarioDto>>
    {
        private readonly ISeguritySystemRepository _repository;
        public SegurityAuthenticateUserSystemCommandHandler(ISeguritySystemRepository repositoryAsync)
        {
            _repository = repositoryAsync;
        }

        public async Task<Response<SegUsuarioDto>> Handle(SegurityAuthenticateUserSystemCommand request, CancellationToken cancellationToken)
        {
            var data = await _repository.ValidateUserSystemLoguin(request.Usuario, request.IpAddress);
            return new Response<SegUsuarioDto>(data);
        }
    }

    public class SegurityAuthenticateUserSystemCommandValidator : AbstractValidator<SegurityAuthenticateUserSystemCommand>
    {
        public SegurityAuthenticateUserSystemCommandValidator()
        {
            RuleFor(p => p.Usuario)
                .NotEmpty().WithMessage("{PropertyName} no pude ser vacio")
                .MaximumLength(120).WithMessage("{PropertyName} no debe exceder de {MaxLength} caracteres");

        }
    }

}
