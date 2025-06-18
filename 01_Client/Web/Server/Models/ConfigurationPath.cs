using Infraestructura.Services;
using Microsoft.Extensions.Configuration;
using Server.Resources.Pages.Authentication.Login;

namespace Server.Models
{
    public class ConfigurationPath : IConfigurationPath
    {

        private readonly IConfiguration _conf;

        public ConfigurationPath(IConfiguration conf)
        {
            _conf = conf;
        }

        public string PathUriDoc()
        {
            return _conf.GetSection(Resource.pathdocuri).Value;
        }

        public string PathUriTemplate()
        {
            return _conf.GetSection(Resource.pathdoctemplate).Value;
        }

    }
}