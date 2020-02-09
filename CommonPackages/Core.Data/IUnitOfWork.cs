using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        string ConnectionString { get; }       

        void Commit();
    }
}
