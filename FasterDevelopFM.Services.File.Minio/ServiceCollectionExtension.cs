using FasterDevelopFM.Services.File.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.File.Minio
{
    public static class ServiceCollectionExtension
    {
        public static void AddMinio(this IServiceCollection service,IConfiguration configuration )
        {
            //单机模式
            var minioEndpoint = configuration["Minio:Server"];
            var minioKey = configuration["Minio:AccessKey"];
            var minioSecret = configuration["Minio:Secret"];
            var minioClient = new MinioClient().WithEndpoint(minioEndpoint).WithCredentials(minioKey,minioSecret).Build();
            service.AddSingleton(minioClient);
            service.AddSingleton<IFileStorage,MinioHelper>();
            //builder.RegisterType<MinioFileStore>().As<IFileStore>();


        }



    }
}
