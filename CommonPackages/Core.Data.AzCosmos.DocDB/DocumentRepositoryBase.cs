using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Core.Data.AzureCosmos.DocumentDB
{
    public class DocumentRepositoryBase<T> : IDocumentRepositoryBase<T> where T : class, IDocument
    {
        protected readonly IDocumentUnitOfWork documentUOW;
        public string containerID;
        protected Container container;

        public DocumentRepositoryBase(IDocumentUnitOfWork unitOfWork)
        {
            documentUOW = unitOfWork;
            containerID = "Patient";
            container = unitOfWork.DBClient.GetDatabase(unitOfWork.DatabaseID).GetContainer(containerID);
        }       

        public async Task<T> UpdateItemAsync(T item)
        {
            try
            {
                return await container.UpsertItemAsync<T>(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetItemAsync(string id, string partionKey)
        {
            try
            {

                return await container.ReadItemAsync<T>(id, new PartitionKey(partionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
        }

        public async Task<List<T>> GetItemsAsync(string query, string partionkey, int maxCount)
        {
            FeedIterator<T> resultSet = container.GetItemQueryIterator<T>(new QueryDefinition(query),
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(partionkey),
                    MaxItemCount = maxCount
                });

            List<T> result = new List<T>();

            while (resultSet.HasMoreResults)
            {
                FeedResponse<T> response = await resultSet.ReadNextAsync();
                if (response.Resource.Count() > 0)
                {
                    result.AddRange(response.Resource);
                }
            }

            return result;
        }

        public async Task<T> DeleteItemAsync(string id, string partionKey)
        {
            return await container.DeleteItemAsync<T>(id, partitionKey: new PartitionKey(partionKey));
        }

        public async Task<T> CreateItemAsync(T item, string partitionKey)
        {
            try
            {
                return await container.CreateItemAsync<T>(item, partitionKey: new PartitionKey(partitionKey));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

        public async Task<int> DeleteBulkItemAsync(string query, string SPName, string partationKey)
        {
            return await container.Scripts.ExecuteStoredProcedureAsync<int>(SPName, partitionKey: new PartitionKey(partationKey), new dynamic[] { query });
        }

        public async Task<int> CreateBulkItemAsync(string SPName, string partationKey, List<T> items)
        {

            return await container.Scripts.ExecuteStoredProcedureAsync<int>(SPName, partitionKey: new PartitionKey(partationKey), new dynamic[] { items });
        }

        public async Task<T> UpdateItemAsync(string id, string partationKey, T item)
        {
            return await container.ReplaceItemAsync<T>(item, id, new PartitionKey(partationKey));
        }

    }
}
