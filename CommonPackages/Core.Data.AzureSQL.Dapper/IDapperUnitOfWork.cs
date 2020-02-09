using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Core.Data;

namespace Core.Data.AzureSQL.Dapper
{
    public interface IDapperUnitOfWork : IUnitOfWork
    {
        //string ConnectionString { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        IsolationLevel IsolationLevel { get; }
        
        //bool Commit();
        void Rollback();

    }
}
