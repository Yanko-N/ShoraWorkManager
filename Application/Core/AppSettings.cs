using System.Text;

namespace Application.Core
{
    public class AppSettings
    {
        public class JWT
        {
            public required string Issuer { get; set; }
            public required string Audience { get; set; }
            public required string Key { get; set; }

            public byte[] GetSymmetricKey()
            {
                if (string.IsNullOrWhiteSpace(Key))
                {
                    return Array.Empty<byte>();
                }

                return Encoding.UTF8.GetBytes(Key);
            }
        }
    }
}
