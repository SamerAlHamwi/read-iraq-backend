using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ReadIraq.EntityFrameworkCore
{
    public static class ReadIraqDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ReadIraqDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ReadIraqDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
