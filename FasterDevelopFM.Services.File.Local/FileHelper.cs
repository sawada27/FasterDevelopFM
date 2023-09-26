using FasterDevelopFM.Services.File.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;

namespace FasterDevelopFM.Services.File.Local
{
    public class LocalFileHelper : IFileStorage
    {

        public async Task<Stream> DownloadFile(string bucketName, string fileName)
        {
            try
            {
                string rootPath =  $"{Directory.GetCurrentDirectory()}/UploadFile/bucketName";
                string filePath = Path.Combine(rootPath, fileName);
                var stream = System.IO.File.OpenRead(filePath);
                string contentType = await GetFileContentTypeAsync(fileName);
                return stream;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public Task<FileBase> GetFileInfo(string bucketName, string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<FileBase> UploadFile(IFormFile file, string bucketName = "")
        {
            string fileName = file.FileName;
            string path = $"{Directory.GetCurrentDirectory()}/UploadFile/{bucketName}/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string extension = Path.GetExtension(fileName);
            string filePath = Path.Combine(path, fileName);
            var contentType = string.Empty;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 获取本地IP
                var hostName = Dns.GetHostName();
                var ipAddresses = Dns.GetHostAddresses(hostName);
                var localIP = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                return new FileBase
                {
                    FilePath = fileName,
                    BucketName = bucketName,
                    FileName = fileName,
                    FileSize = file.Length,
                    FileType = contentType ?? "",
                    Extension = extension,
                    UploadMode = "Local",
                    FileLocation = localIP?.ToString() ?? string.Empty
                };
            }
            catch (Exception e)
            {
                throw new Exception($"文件{fileName}上传失败", e);
            }
        }

        /// < summary>
        /// 获取文件ContentType
        /// < /summary>
        /// < param name="fileName">文件名称< /param>
        /// < returns>< /returns>
        public async static Task<string> GetFileContentTypeAsync(string fileName)
        {
            return await Task.Run(() =>
            {
                string suffix = Path.GetExtension(fileName);
                var provider = new FileExtensionContentTypeProvider();
                var contentType = provider.Mappings[suffix];
                return contentType;
            });
        }

    }
}