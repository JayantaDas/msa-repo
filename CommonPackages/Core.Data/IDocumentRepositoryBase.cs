using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IDocumentRepositoryBase<T>
    {
        Task<T> CreateItemAsync(T item, string partitionKey);
        Task<T> GetItemAsync(string id, string partitionKey);
        Task<List<T>> GetItemsAsync(string query, string partitionKey, int maxCount);
        Task<T> UpdateItemAsync(T item);
        Task<T> UpdateItemAsync(string id, string partitionKey, T item);
        Task<T> DeleteItemAsync(string id, string partitionKey);
        Task<int> DeleteBulkItemAsync(string query, string SPName, string partationKey);
        Task<int> CreateBulkItemAsync(string SPName, string partationKey, List<T> items);
    }
}
