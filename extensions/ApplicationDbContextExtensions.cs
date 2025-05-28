using DETP.data;
using Microsoft.EntityFrameworkCore;

namespace DETP.extensions
{
    public static class ApplicationDbContextExtensions
    {
        public static string GetTableName<TEntity>(this ApplicationDbContext context) where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            return entityType.GetTableName();
        }
    }
}
