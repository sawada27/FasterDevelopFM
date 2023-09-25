namespace FasterDevelopFM.Services.File.Abstract
{
    /// <summary>
    /// 文件
    /// </summary>
    public class FileBase
    {
        /// <summary>
        /// 构造
        /// </summary>
        public FileBase()
        {
            this.FileName = string.Empty;
            this.FilePath = string.Empty;
            this.Description = string.Empty;
            this.FileType = string.Empty;
            this.Extension = string.Empty;
            this.SortCode = 0;
            this.CreateUserName = string.Empty;
            this.CreateTime = DateTime.Now;
        }

        public FilePeriod FilePeriod { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
	    /// 文件路径
	    /// </summary>
        public string FilePath { get; set; }
        /// <summary>
	    /// 描述
	    /// </summary>
        public string Description { get; set; }
        /// <summary>
	    /// 文件类型
	    /// </summary>
        public string FileType { get; set; }
        /// <summary>
	    /// 文件大小
	    /// </summary>
        public long FileSize { get; set; }
        /// <summary>
	    /// 扩展名称
	    /// </summary>
        public string Extension { get; set; }
        /// <summary>
	    /// 是否可用
	    /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
	    /// 排序
	    /// </summary>
        public int SortCode { get; set; }
        /// <summary>
	    /// 删除标识
	    /// </summary>
        public int IsDelete { get; set; } = 0;
        /// <summary>
        /// 上传人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
	    /// 上传人姓名
	    /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
	    /// 上传时间
	    /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 上传方式Minio/本地/其他
        /// </summary>
        public string UploadMode { get; set; }

        /// <summary>
        /// 桶
        /// </summary>
        public string BucketName { get; set; }

    }


    public enum FilePeriod
    {
        Forever = 0,

        OneYear = 1,

        HalfYear = 2,

        ThreeMonth = 3,

        OneMonth = 4,


        HalfMonth = 5,

        SevenDays = 6,

        OneDay = 7
    }
}