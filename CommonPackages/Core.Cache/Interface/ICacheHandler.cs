using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache.Interface
{
    public interface ICacheHandler
    {
        T GetData<T>(string key);
        void UpdateData(string key, object data);
    }

}
