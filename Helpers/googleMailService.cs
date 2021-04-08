using System;
using UsuariosApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace mails
{
    public interface IgoogleMailService
    {
        void logonMail(Usuario usuario);
        void passwordMail(Usuario usuario);
        void customMail(List<string> to, List<string> cc, string subject, string body);
    }
    public class googleResponseMail
    {

        public bool procesado { get; set; }
        public responseError error { get; set; }

    }
    public class googleResponseError
    {

        public int codigo { get; set; }
        public string descripcion { get; set; }
        public int origen_error { get; set; }

    }
    public class GoogleMailService : IgoogleMailService
    {
        public GoogleMailService()
        {

        }
        public class requestMail
        {
            public Usuario Usuario { get; set; }


            public string subject { get; set; }
            public string body { get; set; }
            public string link { get; set; }


        }

        public void customMail(List<string> to, List<string> cc, string subject, string body)
        {
            var mailMessage = new System.Net.Mail.MailMessage();
            to.ForEach(t =>
            {
                mailMessage.To.Add(t);
            });
            cc.ForEach(c =>
            {
                mailMessage.CC.Add(c);
            });
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            sendMailAsync(mailMessage);
        }


        public void logonMail(Usuario usuario)
        {
            var link = "http://localhost:8080/?vista=claveCambio&ticket=" + usuario.Ticket;


            var mailMessage = new System.Net.Mail.MailMessage();

            mailMessage.To.Add(usuario.Email);

            mailMessage.Subject = "Activación de cuenta AppUOCRA";
            mailMessage.Body = "Estimado/a " + usuario.Apellido.Trim() + "," + usuario.Nombre.Trim() + "<br>Hemos recibido una solicitud para el alta de tu cuenta de AppUOCRA, por favor haz <a href='" + link + "'>click aqui</a> para validarla.";
            mailMessage.IsBodyHtml = true;

            sendMailAsync(mailMessage);
        }



        public void passwordMail(Usuario usuario)
        {
            var link = "http://localhost:8080/?vista=claveCambio&ticket=" + usuario.Ticket;

            var mailMessage = new System.Net.Mail.MailMessage();

            mailMessage.To.Add(usuario.Email);

            mailMessage.Subject = "Recupero de contraseña AppUOCRA";
            mailMessage.Body = "Estimado/a " + usuario.Apellido.Trim() + "," + usuario.Nombre.Trim() + "<br>Hemos recibido una solicitud para el cambio de tu contraseña de usuario de AppUOCRA, por favor haz <a href='" + link + "'>click aqui</a> para realizar el cambio.";
            mailMessage.IsBodyHtml = true;
            sendMailAsync(mailMessage);

        }

        static string[] Scopes = { GmailService.Scope.GmailSend };
        static string ApplicationName = "Gmail API .NET Quickstart";
        private void sendMailAsync(System.Net.Mail.MailMessage mailMessage)
        {
            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

            var gmailMessage = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Encode(mimeMessage.ToString())
            };


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Google.Apis.Gmail.v1.UsersResource.MessagesResource.SendRequest request = service.Users.Messages.Send(gmailMessage, "me");

            request.Execute();
        }

        public string Encode(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            return System.Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

    }
}