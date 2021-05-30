using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UsuariosApi.Entities;
using System.Collections.Generic;

namespace mails
{
    public interface IMailService
    {
        Task<responseMail> newUserMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario);
        Task<responseMail> newPasswordMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario);
    }
    public class responseMail
    {

        public bool procesado { get; set; }
        public responseError error { get; set; }

    }
    public class responseError
    {

        public int codigo { get; set; }
        public string descripcion { get; set; }
        public int origen_error { get; set; }

    }
    public class MailService : IMailService
    {
        //private const string BaseUrl = "https://www.comunidad.com.ar/apirestful/api/sendemail";
        private const string BaseUrl = "https://www.uocra.net/MailUocra/odata/SendMail";

        private const string linkCliente = "https://dimodo.ga/demos/recupero/index.html?ticket=";
        private const string linkVeterinario = "https://dimodo.ga/demos/recupero/index.html?ticket=";
        private const string linkAdministracion = "https://dimodo.ga/demos/recupero/index.html?ticket=";
        private readonly HttpClient _client;

        private readonly string _token;
        public MailService(HttpClient client, string token)
        {
            _client = client;
            _token = token;
        }
        public class mailDTO
        {
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> Mails { get; set; }

        }
        public class requestMail
        {
            public mailDTO MailDTO { get; set; }

        }


        public Task<responseMail> newUserMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario)
        {
            var request = new requestMail();

            request.MailDTO = new mailDTO();
            request.MailDTO.Subject = "Activación de cuenta AppUOCRA!!";
            request.MailDTO.Body = "Estimado/a " + nombreDestinatario + ".<br>Hemos recibido una solicitud de activación de tu cuenta de AppUOCRA.</br>Tu código de activación es " + ticket;
            request.MailDTO.Mails = new List<string>();
            request.MailDTO.Mails.Add(emailDestinatario);

            return sendMailAsync(request);
        }

        public Task<responseMail> newPasswordMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario)
        {
            var request = new requestMail();

            request.MailDTO = new mailDTO();
            request.MailDTO.Subject = "Recupero de contraseña AppUOCRA!!";
            request.MailDTO.Body = "Estimado/a " + nombreDestinatario + ".<br>Hemos recibido una solicitud para el cambio de tu contraseña de usuario de AppUOCRA, por favor haz <a href='https://app.uocra.org/?vista=claveCambio&ticket=" + ticket + "'>click aquí</a> para realizar el cambio.";
            request.MailDTO.Mails = new List<string>();
            request.MailDTO.Mails.Add(emailDestinatario);

            return sendMailAsync(request);

        }

        private async Task<responseMail> sendMailAsync(requestMail request)
        {
            var content = JsonConvert.SerializeObject(request);

            var httpResponse = await _client.PostAsync(BaseUrl, new StringContent(content, Encoding.Default, "application/json"));

            if (!httpResponse.IsSuccessStatusCode)
            {
                // ver que pasa si no puede enviar el mail
                //throw new Exception("Cannot add a todo task");
            }

            var createdTask = JsonConvert.DeserializeObject<responseMail>(await httpResponse.Content.ReadAsStringAsync());

            return createdTask;
        }

        private string getLink(string perfil)
        {
            string link = linkAdministracion;
            if (perfil.ToUpper().Contains(Roles.Veterinario.ToUpper())) link = linkVeterinario;
            if (perfil.ToUpper().Contains(Roles.Cliente.ToUpper())) link = linkCliente;
            return link;
        }

    }
}