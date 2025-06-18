using Aplicacion.DTOs.Segurity;
using Aplicacion.Features.Segurity.Commands;
using Aplicacion.Wrappers;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Test.Features.Segurity.Commands
{
    using static Testing;
    public class SegurityUserValidateCommandTest
    {
        [Test]
        public async Task ShoulValidateSegurityUser() {
            
            var command = new SegurityAuthenticateUserCommand 
            {
               Usuario = "admin",
               Password = "1826"
            };

            Response<SegUsuarioDto> result = await SendAsync(command);
            result.Data.Should().NotBeNull();
            result.Data.JwToken.Should().NotBeNullOrEmpty();            
        }
    }
}
