namespace AggregationService.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    public abstract class Service<T> : IDisposable where T : class
    {
        private readonly DbContext _context;

        protected Service(DbContext context) => _context = context;

        protected T Create(T obj)
        {
            _context.Set<T>().Add(obj);
            _context.SaveChanges();

            return obj;
        }

        protected IEnumerable<T> CreateRange(IEnumerable<T> rng)
        {
            _context.Set<T>().AddRange(rng);
            _context.SaveChanges();

            return rng;
        }

        protected IEnumerable<U> CreateRange<U>(IEnumerable<U> rng) where U : class
        {
            _context.Set<U>().AddRange(rng);
            _context.SaveChanges();

            return rng;
        }

        protected DbSet<T> Read() => _context.Set<T>();

        protected DbSet<U> Read<U>() where U : class => _context.Set<U>();

        protected DbContextTransaction BeginTransaction() => _context.Database.BeginTransaction();

        protected void DeleteRange<U>(IEnumerable<U> rng) where U : class
        {
            _context.Set<U>().RemoveRange(rng);
            _context.SaveChanges();
        }

        protected void CommitChanges()
        {
            _context.SaveChanges();
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