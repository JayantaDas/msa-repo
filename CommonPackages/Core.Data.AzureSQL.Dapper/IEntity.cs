using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.AzureSQL.Dapper
{
    public interface IEntity
    {
        int Id { get; set; }
    }
}
