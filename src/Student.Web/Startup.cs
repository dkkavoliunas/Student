using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Student.Repository;
using Microsoft.EntityFrameworkCore;

namespace Student.Web
{
    public class Startup
    {
       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StudentContext>(
                  options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDb;Database=TheStudentDb;Trusted_Connection=true;MultipleActiveResultSets=true;", b => b.MigrationsAssembly("Student.Web")));

            services.AddMvc();
           
        }

       
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
