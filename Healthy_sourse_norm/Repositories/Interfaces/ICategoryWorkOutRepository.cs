using Healthy_sourse_norm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthy_sourse_norm.Repositories.Interfaces
{
    public interface ICategoryWorkOutRepository : IGenericRepository<Category_WorkOut>
    {
        public Task<IEnumerable<Category_WorkOut>> GetCategoryByNameAsync(string name);
    }
}
