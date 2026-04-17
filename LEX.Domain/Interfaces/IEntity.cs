using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Interfaces
{
    public interface IEntity<TKey>
    {
        TKey User_Id { get; set; }
    }
}
