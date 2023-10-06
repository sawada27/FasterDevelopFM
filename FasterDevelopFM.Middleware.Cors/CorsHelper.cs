using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FasterDevelopFM.Middleware.Cors
{
    public static class CorsHelper
    {



        /// <summary>
        /// 配置跨域
        /// </summary>
        public static void AddCorsHelper(this IServiceCollection services, IConfiguration config)
        {
          
            ////use app=> IApplicationBuilder 暂时没想好怎么写进拓展类合适
            //app.UseCors(builder => builder.AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());
        }

    }
}
