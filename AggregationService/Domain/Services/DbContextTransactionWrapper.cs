namespace AggregationService.Domain.Services
{
    using System.Data.Entity;

    internal class DbContextTransactionWrapper : ITransaction
    {
        private readonly DbContextTransaction _transaction;

        public DbContextTransactionWrapper(DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit() => _transaction.Commit();

        public void Dispose() => _transaction.Dispose();
    }
}
