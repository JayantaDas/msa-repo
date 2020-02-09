using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.AzureCosmos.DocumentDB
{
    public interface IDocument
    {
        public string Id { get; set; }
    }
}
