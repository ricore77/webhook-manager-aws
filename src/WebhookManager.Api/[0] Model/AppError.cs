using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    public class AppErrorReponse
    {
        public string Error { get; set; }
        public string Status { get; set; }

        public string Timestamp { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }

    }
}
