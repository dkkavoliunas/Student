using Microsoft.EntityFrameworkCore;
using Student.Models;

namespace Student.Repository
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Course> Courses { set; get; }
        public DbSet<User> Users { set; get; }
    }
}
