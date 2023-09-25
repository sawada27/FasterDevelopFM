using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FasterDevelopFM.Infrastructure
{
    public class StandardResponseBase<T>
    {
        public StandardResponseBase() { 
        

        }

        public StandardResponseBase(string errorMessage)
        {
            Code = 500;
        }

        public StandardResponseBase(int code,string message)
        {
            Code = code;
            Message = message;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 请求结果
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 跟踪ID
        /// </summary>
        public string TraceId { get; internal set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 错误列表
        /// </summary>
        public string[] Errors { get; set; }


        /// <summary>
        /// ToJson
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}