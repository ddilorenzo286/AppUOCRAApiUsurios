using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UsuariosApi.Entities;

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
        private const string BaseUrl = "https://www.comunidad.com.ar/apirestful/api/sendemail";
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
        public class requestMail
        {

            public string token { get; set; }
            public string email_destinatario { get; set; }
            public string nombre_destinatario { get; set; }
            public string parametro_1 { get; set; }
            public string parametro_2 { get; set; }
            public string cuerpo { get; set; }
            public int id_tipo_comunicacion { get; set; }

        }


        public Task<responseMail> newUserMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario)
        {
            var request = new requestMail
            {
                id_tipo_comunicacion = 3,
                parametro_1 = getLink(perfil) + ticket,
                token = _token,
                email_destinatario = emailDestinatario,
                nombre_destinatario = nombreDestinatario
            };

            return sendMailAsync(request);
        }

        public Task<responseMail> newPasswordMail(string perfil, string ticket, string emailDestinatario, string nombreDestinatario)
        {

            var request = new requestMail
            {
                id_tipo_comunicacion = 2,
                parametro_1 = getLink(perfil) + ticket,
                token = _token,
                email_destinatario = emailDestinatario,
                nombre_destinatario = nombreDestinatario
            };

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