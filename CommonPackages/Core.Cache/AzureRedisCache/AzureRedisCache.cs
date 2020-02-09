using Common.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Common.Cache.Utility;

namespace Common.Cache.AzureRedisCache
{
    class AzureRedisCache:ICacheHandler
    {
        private string connectionString = string.Empty;
        private readonly HelperClass helperClass = new HelperClass();

        public AzureRedisCache(string connString)
        {
            this.connectionString = connString;
        }
        public T GetData<T>(string key)
        {
            try
            {
                var database = GetDataBase();
                if (database != null)
                {
                    var data = database.StringGet(key);
                    return helperClass.Deserialize<T>(data);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return default(T);
        }

        public void UpdateData(string key, object data)
        {
            if (data == null)
            {
                return;
            }
            try
            {
                var database = GetDataBase();
                if (database != null)
                {
                    database.StringSet(key, helperClass.Serialize(data));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private IDatabase GetDataBase()
        {
            if (string.IsNullOrEmpty(connectionString))
                return null;

            IDatabase database = null;

            try
            {
                //string connectionString = "TVtesting.redis.cache.windows.net:6380,password=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX,ssl=True,abortConnect=False";
                var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

                if (connectionMultiplexer.IsConnected)
                    database = connectionMultiplexer.GetDatabase();

                return database;
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return database;
        }
    }
}
