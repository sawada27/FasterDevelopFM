using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.File.AliOss
{
    /// <summary>
    /// 配置键
    /// </summary>
    internal static class ConfigKeys
    {

        public const string RootKey = "AliyunOss";

        public const string AccessKeyID = "AliyunOss:access-key:id";
        public const string AccessKeySecret = "AliyunOss:access-key:secret";

        public const string Synchronized = RootKey + ":synchronized";

        public static string Bucket(string bucket)
        {
            return $"{RootKey}:buckets:{bucket}";
        }
    }
}
