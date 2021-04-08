namespace UsuariosApi.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string mailToken { get; set; }
        public string mailSecret { get; set; }

        public string mailInfoTo { get; set; }
        public string mailInfoCC { get; set; }
        public string mailInfoFrom { get; set; }

    }
}