using LEX.Domain.Interfaces;
using LEX.Persistence.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();


        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
        {
            var EntityType = typeof(TEntity);

            // لو موجود رجعه
            if (_repositories.TryGetValue(EntityType, out var repo))
            {
                return (IGenericRepository<TEntity, TKey>)repo;
            }

            // لو مش موجود اعمل واحد جديد
            var newRepo = new GenericRepository<TEntity, TKey>(_dbContext);

            _repositories[EntityType] = newRepo;

            return newRepo;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
