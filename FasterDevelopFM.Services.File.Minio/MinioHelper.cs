using FasterDevelopFM.Services.File.Abstract;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;

namespace FasterDevelopFM.Services.File.Minio
{


    public class MinioHelper : IFileStorage
    {
        static long MB = 1024 * 1024;
        private readonly MinioClient _minioClient;

        public MinioHelper(MinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<FileBase> UploadFile(IFormFile file, string bucketName = "")
        {
            var fileName = file.FileName;
            try
            {
                var folderYear = DateTime.Now.ToString("yyyy");
                var folderMonth = DateTime.Now.ToString("MM");
                var folderDay = DateTime.Now.ToString("dd");
                bucketName = !string.IsNullOrWhiteSpace(bucketName) ? bucketName : folderYear;
                // 找到bucket，如果不存在则创建
                var beArgs = new BucketExistsArgs().WithBucket((string)bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket((string)bucketName).WithObjectLock();
                    await _minioClient.MakeBucketAsync(mbArgs);
                }

                if (file != null && file.Length > 0 && file.Length < 100 * MB)
                {
                    var ext = Path.GetExtension(fileName).ToLower();
                    string newName = $"{folderYear}/{folderMonth}/{folderDay}/{fileName}{ext}";

                    //缩略图 暂定
                    //string thumbnailName = "";
                    //if (ext.Contains(".jpg") || ext.Contains(".jpeg") || ext.Contains(".png") || ext.Contains(".bmp") || ext.Contains(".gif"))
                    //{
                    //    thumbnailName = $"{folderMonth}/{folderDay}/{GenerateId.GenerateOrderNumber() + ext}";
                    //}

                    //todo: contentType获取待定  FileExtensionContentTypeProvider() or 帮助类 (未测试linux)
                    var contentType = string.Empty;

                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        var putObjectArgs = new PutObjectArgs().WithBucket(bucketName)
                        .WithObject(newName)
                    .WithStreamData(binaryReader.BaseStream)
                    .WithObjectSize(binaryReader.BaseStream.Length)
                    .WithContentType(contentType);
                        await _minioClient.PutObjectAsync(putObjectArgs);

                    }
                    return new FileBase
                    {
                        FilePath = newName,
                        BucketName = bucketName,
                        FileName = fileName,
                        FileSize = file.Length,
                        FileType = contentType ?? "",
                        Extension = Path.GetExtension(fileName),
                        UploadMode = "Minio"
                    };
                }
                else
                {
                    throw new Exception($"文件{fileName}上传失败，文件过大");
                }
            }
            catch (Exception e)
            {
                //抛出给外层捕获 可记录其他上传信息 本方法不做处理
                throw new Exception($"文件{fileName}上传失败，原因{e.Message}", e);
            }

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFile(string bucketName, string fileName)
        {
            var fileStream = new MemoryStream();
            var getObjectArgs = new GetObjectArgs()
        .WithBucket(bucketName)
        .WithObject(fileName)
        .WithCallbackStream(stream => stream.Dispose());
            await _minioClient.GetObjectAsync(getObjectArgs);
            fileStream.Position = 0;
            return fileStream;
        }


        public async Task<FileBase> GetFileInfo(string bucketName, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}