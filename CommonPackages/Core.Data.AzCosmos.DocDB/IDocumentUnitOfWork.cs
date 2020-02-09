using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.AzureCosmos.DocumentDB
{
    public interface IDocumentUnitOfWork : IUnitOfWork
    {
        string DBEndpoint { get; }
        string DBKey { get; }
        string DatabaseID { get; }

        CosmosClient DBClient { get; }
        
    }
}
