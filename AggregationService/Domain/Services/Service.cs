namespace AggregationService.Domain.Services
{
    using System;    
    using System.Data.Entity;
    using System.Collections.Generic;

    public abstract class Service<T> where T : class
    {
        private DbContext _context;

        protected Service(DbContext context) => _context = context;

        protected T Create(T obj)
        {
            try
            {
                _context.Set<T>().Add(obj);
                _context.SaveChanges();

                return obj;
            }
            catch
            {
                throw;
            }
        }

        protected IEnumerable<T> CreateRange(IEnumerable<T> rng)
        {
            try
            {
                _context.Set<T>().AddRange(rng);
                _context.SaveChanges();

                return rng;
            }
            catch
            {
                throw;
            }
        }

        protected IEnumerable<U> CreateRange<U>(IEnumerable<U> rng) where U : class
        {
            try
            {
                _context.Set<U>().AddRange(rng);
                _context.SaveChanges();

                return rng;
            }
            catch
            {
                throw;
            }
        }

        protected DbSet<T> Read() => _context.Set<T>();

        protected DbSet<U> Read<U>() where U : class => _context.Set<U>();

        /*protected void Update(T obj)
        {
            _context.Set<T>().Attach(obj);
            _context.SaveChanges();
        }*/

        /*protected void Update<U>(U obj) where U :class
        {
            _context.Set<U>().Attach(obj);
            _context.SaveChanges();
        }*/

        protected void DeleteRange<U>(IEnumerable<U> rng) where U : class
        {
            try
            {
                _context.Set<U>().RemoveRange(rng);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        protected void CommitChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }
}