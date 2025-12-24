using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence
{
    public class Config
    {

        public string UrlPresence { get; set; } = "https://dados.presence.com.br/api/PresenceWebApi.dll/api/v1/WebMethods/consulta";
        public string UrlPrecenceToken { get; set; } = "https://dados.presence.com.br/api/PresenceAuthorization.dll/api/v1/authorization/token";
            
    }
}
