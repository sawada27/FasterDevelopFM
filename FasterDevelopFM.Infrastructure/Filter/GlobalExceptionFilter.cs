using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FasterDevelopFM.Infrastructure.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public GlobalExceptionFilter(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                Exception exception = context.Exception;
                string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path;
                string parameter = context.HttpContext.Request.QueryString.ToString();
                string method = context.HttpContext.Request.Method.ToString();

               //写入日志
                Logger.LogError($"报错地址:{url},请求方式：{method},参数:{parameter},异常描述：{exception.Message},堆栈信息：{exception.StackTrace}");
                
                context.Result = new ObjectResult(new StandardResponseBase<string>
                {
                    Code = 500,
                    Message = exception.InnerException?.Message ??  exception.Message  // "服务器内部错误，请联系开发人员处理"
                });

            }
            context.ExceptionHandled = true;
        }
        /// <summary>
        /// 异步发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;

        }


    }

}
