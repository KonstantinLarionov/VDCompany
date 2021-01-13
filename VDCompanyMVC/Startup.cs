using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using VDCompanyMVC.Hubs;
using VDCompanyMVC.Models;
using VDCompanyMVC.Models.Entitys;

namespace VDCompanyMVC
{
    internal static class Extensions
    {
        public static int ToInt(this string str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
        }
        public static double ToDouble(this string str)
        {
            try
            {
                return Convert.ToDouble(str.Replace(".", ","));
            }
            catch
            {
                return 0;
            }
        }
        public static string ToStringDF(this double number)
        {
            try
            {
                return Convert.ToString(number).Replace(",", ".");
            }
            catch
            {
                return "";
            }
        }
        public static string ToStringIF(this int number)
        {
            try
            {
                var countV = number.ToString().Count();
                switch (countV)
                {
                    case 10:
                        return number.ToString("# ### ### ###");
                    case 9:
                        return number.ToString("### ### ###");
                    case 8:
                        return number.ToString("## ### ###");
                    case 7:
                        return number.ToString("# ### ###");
                    case 6:
                        return number.ToString("### ###");
                    case 5:
                        return number.ToString("## ###");
                    case 4:
                        return number.ToString("# ###");
                    default:
                        return number.ToString();
                }
            }
            catch
            {
                return "";
            }
        }
    }
    public class Startup
    {
        private static readonly StartContext db = new StartContext(new DbContextOptions<StartContext>());
        internal static Action<string> Logs = null;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Logs += Loger;
        }
        public void Loger(string message)
        {
            var main_path = Environment.CurrentDirectory + @"\Logs\" + DateTime.Now.Date.Day.ToString() + "." + DateTime.Now.Date.Month.ToString() + @"\";
            if (Directory.Exists(main_path))
            {
                var file_path = $"{main_path}LOGAT{DateTime.Now.Hour.ToString()}-{DateTime.Now.Minute.ToString()}.txt";

                File.AppendAllText(file_path, message);
            }
            else
            {
                Directory.CreateDirectory(main_path);
                Loger(message);
            }
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddDbContextPool<StartContext>(
               options => options.UseMySql(Conf.ConnectDb,
               mySqlOptions =>
                 {
                     mySqlOptions.ServerVersion(new Version(5, 6, 45), ServerType.MySql);
                 }
           ));
            services.AddDbContextPool<ChatContext>(
            options => options.UseMySql(Conf.DBChat,
                mySqlOptions =>
                {
                    mySqlOptions.ServerVersion(new Version(5, 6, 45), ServerType.MySql);
                }
        ));
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(100000);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<CaseHub>("/caseHub");
                endpoints.MapHub<MyHub>("/MyHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
