using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolAppCore.Models
{
    public class SchoolContenxt : DbContext
    {
        public SchoolContenxt (DbContextOptions<SchoolContenxt> options)
            : base(options)
        {
        }

        public DbSet<SchoolAppCore.Models.Course> Course { get; set; } = default!;
    }
}
