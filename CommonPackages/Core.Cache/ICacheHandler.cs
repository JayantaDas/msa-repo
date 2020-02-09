using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Cache
{
    public interface ICacheHandler
    {
        T GetData<T>(string key);
        bool SetData(string key, object data);
        bool Remove(string key);
        bool RemoveAll();
    }
}
