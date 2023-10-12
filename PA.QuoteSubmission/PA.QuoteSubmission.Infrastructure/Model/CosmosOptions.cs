using Azure.Core;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Data.Cosmos.model
{
    public class CosmosOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
        public String ContainerName { get; set; }
        public CosmosClientOptions? ClientOptions { get; set; }
    }
}
