using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class RefDataOptions
    {
        public string BindingName { get; set; }
        public string Operation { get; set; }
        public string Query { get; set; }
        public string ParameterName { get; set; }
    }
}
