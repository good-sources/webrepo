namespace AggregationService.Auth
{
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Text;

    public static class AuthenticationService
    {
        public static bool ValidateCredentials(string username, string password)
        {
            string expectedUser = ConfigurationManager.AppSettings["auth:username"];
            string expectedHash = ConfigurationManager.AppSettings["auth:passwordHash"];

            if (username != expectedUser)
            {
                return false;
            }

            string inputHash = ComputeSha256(password);
            return inputHash == expectedHash;
        }

        private static string ComputeSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
