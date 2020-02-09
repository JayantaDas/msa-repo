using Core.Cache;
using Common.Cache.Utility;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Core.Cache.AzureRedis
{
    public class RedisCache:ICacheHandler
    {
        #region private members
        private string connectionString = string.Empty;
        private ConnectionMultiplexer connection = null;
        private readonly Helper helper = new Helper();
        private readonly object lockObject = new object();
        private readonly ILogger<RedisCache> logger = null;
        #endregion

        #region constructors
        public RedisCache(string connectionString, ILogger<RedisCache> logger=null)
        {
            this.connectionString = connectionString;
            this.logger = logger;
        }
        #endregion

        #region private member functions
        private ConnectionMultiplexer GetConnection()
        {
            logger?.LogDebug("AzureRedisCache:GetConnection()", connectionString);

            try
            {
                if (connection == null || !connection.IsConnected)
                {
                    lock (lockObject)
                    {
                        if (connection == null || !connection.IsConnected)
                            connection = ConnectionMultiplexer.Connect(connectionString);
                    }
                }
            }
            catch (Exception e)
            {
                logger?.LogCritical(e, "AzureRedisCache:GetConnection()", connectionString);
            }

            return connection;
        }

        private IDatabase GetDataBase()
        {
            logger?.LogDebug("AzureRedisCache:GetDataBase()", connectionString);

            if (string.IsNullOrEmpty(connectionString))
            {
                logger?.LogError("AzureRedisCache:GetDataBase() connection string is NULL or Empty", "");
                return null;
            }

            IDatabase database = null;

            try
            {
                var connection = GetConnection();

                if (connection != null && connection.IsConnected)
                    database = connection.GetDatabase();
                
                return database;
            }
            catch (Exception e)
            {
                logger?.LogCritical(e, "AzureRedisCache:GetDataBase()", "");
            }

            return database;
        }
        #endregion

        #region public member functons
        public T GetData<T>(string key)
        {
            logger?.LogDebug("AzureRedisCache:GetData()", key);

            try
            {
                var database = GetDataBase();
                if (database != null)
                {
                    var data = database.StringGet(key);

                    return helper.Deserialize<T>(data);
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "AzureRedisCache:GetData()", key);
            }

            return default(T);
        }

        public bool SetData(string key, object data)
        {
            logger?.LogDebug("AzureRedisCache:SetData()", new object[] { (object)key, (object)data });

            bool setDataSuccessful = false;

            try
            {
                var database = GetDataBase();
                if (database != null)
                {
                    setDataSuccessful = database.StringSet(key, helper.Serialize(data));
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "AzureRedisCache:SetData()", new object[] { (object)key, (object)data });
            }

            return setDataSuccessful;
        }

        public bool Remove(string key)
        {
            logger?.LogDebug("AzureRedisCache:Remove()", "");

            bool removeDataSuccessful = false;

            try
            {
                if (string.IsNullOrEmpty(key))
                    return removeDataSuccessful;

                var database = GetDataBase();
                if (database != null)
                {
                    removeDataSuccessful = database.KeyDelete(key);
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "AzureRedisCache:Remove()", key);
            }

            return removeDataSuccessful;
        }

        public bool RemoveAll()
        {
            logger?.LogDebug("AzureRedisCache:RemoveAll()", "");

            bool removeAllDataSuccessful = true;

            try
            {
                var connection = GetConnection();
                if (connection != null)
                {
                    var endpoints = connection.GetEndPoints(true);
                    foreach (var endpoint in endpoints)
                    {
                        var server = connection.GetServer(endpoint);
                        server.FlushAllDatabases();
                    }
                }
                else
                {
                    removeAllDataSuccessful = false;
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, "AzureRedisCache:RemoveAll()", "");
                removeAllDataSuccessful = false;
            }

            return removeAllDataSuccessful;
        }
        #endregion
    }
}
