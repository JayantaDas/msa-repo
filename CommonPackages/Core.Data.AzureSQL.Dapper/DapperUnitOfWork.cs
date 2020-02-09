using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Core.Data.AzureSQL.Dapper
{
    public class DapperUnitOfWork : IDapperUnitOfWork
    {
        #region Fields
        public Dictionary<Type, dynamic> Repositories { get; set; }
        
        public string ConnectionString { get; private set; }        

        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public IsolationLevel IsolationLevel { get; set; }
        #endregion

        #region Constructors
        public DapperUnitOfWork(IDbConnection connection, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            Connection = connection;
            IsolationLevel = isolationLevel;
            Repositories = new Dictionary<Type, dynamic>();

            try
            {
                Connection.Open();
                Transaction = Connection.BeginTransaction(isolationLevel);
            }
            catch (Exception ex)
            {
                //TO DO
                //Log(ex.ToString());  
                throw;
            }
        }

        public DapperUnitOfWork(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        : this(new SqlConnection(connectionString), isolationLevel) { }

        #endregion

        #region Methods

        public IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        {
            //TO DO
            //if (ServiceLocator.IsLocationProviderSet)
            //{
            //    return ServiceLocator.Current.GetInstance<IRepositoryBase<TEntity>>();
            //}

            if (Repositories == null)
                Repositories = new Dictionary<Type, dynamic>();

            if (Repositories.ContainsKey(typeof(TEntity)))
                return (IRepositoryBase<TEntity>)Repositories[typeof(TEntity)];

            var repositoryType = typeof(RepositoryBase<>);
            Repositories.Add(typeof(TEntity), Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), this));

            return Repositories[typeof(TEntity)];
        }

        public void Commit()
        {
            try
            {
                Transaction.Commit();
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                //Log(ex.ToString());   //TO DO
                throw;
            }
            finally
            {
                Reset();
            }
        }

        public void Rollback()
        {
            Transaction.Rollback();

            Reset();
        }

        public void Reset()
        {
            Transaction.Dispose();
            Transaction = Connection.BeginTransaction();
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
                    if (Transaction != null)
                    {
                        Transaction.Dispose();
                        Transaction = null;
                    }
                    if (Connection != null)
                    {
                        Connection.Dispose();
                        Connection = null;
                    }
                }
                _disposed = true;
            }
        }
        #endregion

    }
}
