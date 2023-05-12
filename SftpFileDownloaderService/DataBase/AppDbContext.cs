using Microsoft.EntityFrameworkCore;
using SftpFileDownloaderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SftpFileDownloaderService.database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        public DbSet<FileModel> YourModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your models and relationships here
        }
    }
}
