using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Infrastructure.Filter
{
    /// <summary>
    /// 身份认证 中间件实现
    /// </summary>
    public class AuthorizationFilter : IActionFilter
    {

        public AuthorizationFilter()
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var description =
                (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;

            var Controllername = description.ControllerName.ToLower();
            var Actionname = description.ActionName.ToLower();

            //匿名标识
            var authorize = description.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
            if (authorize != null)
            {
                return;
            }

            //todo:这里增加判断登录逻辑
            bool checkLogin = false;

            if (checkLogin == false)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new ObjectResult(new StandardResponseBase<string>
                {
                    Code = 401,
                    Message = "身份认证失败，请重新登陆"
                });
            }

            //todo：增加鉴权逻辑

            //var actionPermission = description.MethodInfo.GetCustomAttribute(typeof(ActionPermissionAttribute));
            //if (actionPermission != null)
            //{
            //    var actionPermissonattr =  actionPermission as ActionPermissionAttribute;
            //    if (_permissionService.GetPermissions(_authUtil.GetCurrentUser().User.Account).Result
            //            .HasPermission(actionPermissonattr.PermissionKey) == false)
            //    {
            //        context.HttpContext.Response.StatusCode = 401;
            //        context.Result = new JsonResult(new Response
            //        {
            //            Code = 403,
            //            Message = "你没有权限"
            //        });
            //    }
            //}

            //todo：可记录访问日志 拆分成单独的中间件或过滤器
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

            return;
        }
    }
}
