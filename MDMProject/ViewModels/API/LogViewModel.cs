using System.Collections.Generic;

namespace MDMProject.WebAPI_Controllers
{
    public class LogViewModel
    {
        public string msg { get; set; }

        public string url { get; set; }

        public string line { get; set; }

        public string col { get; set; }

        public string stack { get; set; }

        public IDictionary<string, string> extra { get; set; }
    }
}