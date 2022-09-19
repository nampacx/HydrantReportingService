using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter
{
    public static class GlobalSettings 
    {
        public static Settings Settings { get; set; }
    }

    public class Settings
    {
        public string SubmitReportUri { get; set; }

        public string RequestSasUriUri { get; set; }
    }
}
