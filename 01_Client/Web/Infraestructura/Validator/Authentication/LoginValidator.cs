using FluentValidation;
using Infraestructura.Models.Authentication;
using Microsoft.Extensions.Localization;

namespace Infraestructura.Validator.Authentication
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator(IStringLocalizer<Resources.Pages.Autentication.Resource> localizer)
        {
            RuleFor(r => r.UserName)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(x => localizer["Usuario es requerido!"]);

            RuleFor(r => r.Password)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(x => localizer["Password es requerido!"]);
        }
    }
}
