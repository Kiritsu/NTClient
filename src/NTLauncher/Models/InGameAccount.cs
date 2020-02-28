using System.Diagnostics;

namespace NTLauncher.Models
{
    public class InGameAccount
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public Process Process { get; set; }
    }
}
