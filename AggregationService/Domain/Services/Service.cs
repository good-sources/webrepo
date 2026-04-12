namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public abstract class Service<T> : IDisposable where T : class
    {
        private readonly DbContext _context;

        protected Service(DbContext context) => _context = context;

        protected async Task<T> CreateAsync(T obj)
        {
            _context.Set<T>().Add(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        protected async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> rng)
        {
            _context.Set<T>().AddRange(rng);
            await _context.SaveChangesAsync();

            return rng;
        }

        protected async Task<IEnumerable<U>> CreateRangeAsync<U>(IEnumerable<U> rng) where U : class
        {
            _context.Set<U>().AddRange(rng);
            await _context.SaveChangesAsync();

            return rng;
        }

        protected DbSet<T> Read() => _context.Set<T>();

        protected DbSet<U> Read<U>() where U : class => _context.Set<U>();

        protected virtual ITransaction BeginTransaction() => new DbContextTransactionWrapper(_context.Database.BeginTransaction());

        protected async Task DeleteRangeAsync<U>(IEnumerable<U> rng) where U : class
        {
            _context.Set<U>().RemoveRange(rng);
            await _context.SaveChangesAsync();
        }

        protected async Task CommitChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
