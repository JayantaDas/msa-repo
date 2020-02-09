using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.AzureCosmos.DocumentDB
{
    public class DocumentUnitOfWork : IDocumentUnitOfWork
    {
        #region Fields

        public string ConnectionString { get; private set; }
        public string DBEndpoint { get; private set; }
        public string DBKey { get; private set; }
        public string DatabaseID { get; private set; }
        public CosmosClient DBClient { get; private set; }

        #endregion

        #region Constructors
        public DocumentUnitOfWork()
        {
            //read from config
            ConnectionString = "AccountEndpoint=https://vca-dev-westus-pv-cosmos.documents.azure.com:443/;AccountKey=LFWKjilAu9rpQdnE86iDIbzp5IEHTSzpCozBx4Eh9c1dAxW5l1HXe4mz1wUQ4xdplEPCI5vLTcDTh1RYxsPaHw==;";
            DatabaseID = "ClientDB";
            
            DBClient = new CosmosClient(ConnectionString);            
        }

        //public DocumentUnitOfWork(CosmosClient cosmosClient)
        //{
        //    DBClient = cosmosClient;
        //}

        //public DocumentUnitOfWork(string connectionString)
        //: this(new CosmosClient(connectionString)) { }
        #endregion

        #region Methods
        public IDocumentRepositoryBase<TDocument> Repository<TDocument>() where TDocument : class, IDocument
        {
            var repositoryType = typeof(DocumentRepositoryBase<>);
            return (DocumentRepositoryBase<TDocument>)Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TDocument)), this);           
        }

        public void Commit()
        {
            throw new NotSupportedException("Transactions are not supported as of now.");
        }

        #endregion

        #region Dispose Methods
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //Dispose if any unmanaged resources
                    //if (DBClient  != null)
                    //{
                    //    DBClient.Dispose();
                    //    DBClient = null;
                    //}
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
