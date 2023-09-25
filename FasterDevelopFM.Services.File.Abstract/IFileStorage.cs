using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.File.Abstract
{
    /// <summary>
    /// 文件存储接口
    /// </summary>
    public interface IFileStorage
    {
        Task<FileBase> UploadFile(IFormFile file, string bucketName = "");

        Task<Stream> DownloadFile(string bucketName, string fileName);

        Task<FileBase> GetFileInfo( string bucketName , string fileName);
    }
}
