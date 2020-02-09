using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data;
using System.Data;

namespace Core.Data.AzureSQL.Dapper
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {
        #region Fields

        protected readonly IDapperUnitOfWork dapperUOW;

        #endregion

        #region Constructors

        public RepositoryBase(IDapperUnitOfWork dapperUnitOfWork)
        {
            dapperUOW = dapperUnitOfWork;
        }

        #endregion

        #region IRepositoryBase Methods

        public virtual int Add(T entity)
        {
            // To be implemented in the data layer of micro service.
            throw new NotImplementedException();
        }

        public virtual T GetById(int id)
        {
            return dapperUOW.Connection.Query<T>(
                $"select * from {typeof(T).Name} where Id = @Id",
                param: new { Id = id },
                transaction: dapperUOW.Transaction)
                .FirstOrDefault();
        }

        public virtual IEnumerable<T> Get()
        {
            return dapperUOW.Connection.Query<T>(
                $"select * from {typeof(T).Name}",
                transaction: dapperUOW.Transaction)
                .ToList();
        }

        public virtual void Update(T entity)
        {
            // To be implemented in the data layer of micro service.
            throw new NotImplementedException();
        }

        public virtual int Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Delete Entity Empty");

            return Delete(entity.Id);
        }

        public virtual int Delete(int id)
        {
            return dapperUOW.Connection.Execute(
                $"delete from {typeof(T).Name} where Id = @Id",
                param: new { Id = id },
                transaction: dapperUOW.Transaction);
        }

        public virtual IEnumerable<T> Query(string query, object[] param = null)
        {
            return dapperUOW.Connection.Query<T>(
                query,
                param,
                transaction: dapperUOW.Transaction)
                .ToList();
        }

        public virtual int ExecuteStoredProcedure(string spName, object param = null)
        {
            //return dapperUOW.Connection.Execute(
            //    spName,
            //    param,
            //    commandType: System.Data.CommandType.StoredProcedure,
            //    transaction: dapperUOW.Transaction);

            if (spName == null || param == null)
                throw new ArgumentNullException("Sql Entity");

            DynamicParameters _params = new DynamicParameters();
            _params.AddDynamicParams(param);
            _params.Add("@Id", DbType.Int32, direction: ParameterDirection.Output);
            var result = dapperUOW.Connection.Execute(spName, _params, transaction: dapperUOW.Transaction, null, commandType: CommandType.StoredProcedure);
            var retVal = _params.Get<int>("Id");
            return retVal;
        }

        #endregion
    }
}
