using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using UEditor.Core;
using WY.WebAPI.Filters;

namespace WY.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment CurrentEnvironment { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                 opt =>
                 {
                     if (!CurrentEnvironment.IsDevelopment())
                     {
                         //opt.UseCentralRoutePrefix(new RouteAttribute("WY_API/"));
                     }
                     //opt.UseCentralRoutePrefix(new RouteAttribute("WY_API/"));
                     //opt.UseCentralRoutePrefix(new RouteAttribute("api/[controller]/[action]"));

                 })
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
            {

                x.ValueLengthLimit = int.MaxValue;

                x.MultipartBodyLengthLimit = int.MaxValue;

                x.MultipartHeadersLengthLimit = int.MaxValue;


            });
            services.AddUEditorService("ueditor.json", true);
            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
      System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/ExcelModel")),
                RequestPath = "/WY_API/ExcelModel"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
       System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/ExcelModel/Templates")),
                RequestPath = "/WY_API/ExcelModel/Templates"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/Files/export")),
                RequestPath = "/WY_API/Files/export"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
               System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/UploadFiles/img")),
                RequestPath = "/WY_API/UploadFiles/img",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
               System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/UploadFiles/HouseImg")),
                RequestPath = "/WY_API/UploadFiles/HouseImg",
                //OnPrepareResponse = ctx =>
                //{
                //    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
                //}
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "WY_API/UploadFiles/notice")),
                RequestPath = "/WY_API/UploadFiles/notice",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=36000");
                }
            });
            // 添加全局异常捕获，方便记录日志
            app.UseMiddleware<ExceptionMiddleware>();
            #region 解决Ubuntu Nginx 代理不能获取IP问题
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
            #endregion
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });
            app.UseMvc();
        }
    }
}
