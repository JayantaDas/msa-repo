using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public interface IRepositoryBase<T>
    {
        int Add(T entity);
        T GetById(int id);
        IEnumerable<T> Get();
        void Update(T entity);
        int Delete(T entity);
        int Delete(int id);

        IEnumerable<T> Query(string query, object[] param = null);

        int ExecuteStoredProcedure(string spName, object param = null);




    }
}
