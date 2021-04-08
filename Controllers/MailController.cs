
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UsuariosApi.Models;
using Microsoft.AspNetCore.Authorization;
using mails;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace LoginService.Controllers
{
    [Route("api/Mail")]
    [ApiController]
    [Authorize]
    public class MailController : ControllerBase
    {

        private readonly LoginServiceContext _context;
        private readonly ILogger<MailController> _logger;
        private readonly IgoogleMailService _mailService;
        private readonly IConfiguration _configuration;
        public MailController(LoginServiceContext context, ILogger<MailController> logger, IgoogleMailService mailService, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mailService = mailService;
            _configuration = configuration;

        }

        [HttpPost("send")]
        [AllowAnonymous]
        public ActionResult send([FromBody] MailParts mailParts)
        {

            var appSettingsSection = _configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<UsuariosApi.Helpers.AppSettings>();



            if (mailParts.token == appSettings.mailSecret)
            {


                _mailService.customMail(mailParts.to, mailParts.cc, mailParts.subject, mailParts.body);

                return Ok();

            }
            else
            {
                return BadRequest();

            }

        }

        [HttpPost("sendInfo")]
        [AllowAnonymous]
        public ActionResult sendInfo([FromBody] SendInfoDto sendInfo)
        {

            var appSettingsSection = _configuration.GetSection("AppSettings");

            var appSettings = appSettingsSection.Get<UsuariosApi.Helpers.AppSettings>();

            string[] mailInfoTo = appSettings.mailInfoTo.Split(",");
            string[] mailInfoCC = appSettings.mailInfoCC.Split(",");

            List<string> cc = new List<string>(mailInfoTo);
            List<string> to = new List<string>(mailInfoCC);

            string body = "Hemos recibido un pedido de información de " + sendInfo.ApellidoNombre + " de la empresa " + sendInfo.Empresa + "(" + sendInfo.Empleados + " empleados). Email:" + sendInfo.Email + " Telefono:" + sendInfo.Telefono;

            _mailService.customMail(to, cc, "Pedido de Información", body);

            return Ok();

        }
    }
    public class MailParts
    {


        public List<string> to { get; set; }
        public List<string> cc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string token { get; set; }

    }

    public class SendInfoDto
    {

        public string ApellidoNombre { get; set; }
        public string Empresa { get; set; }
        public string Email { get; set; }
        public int Empleados { get; set; }
        public string Telefono { get; set; }


    }


}