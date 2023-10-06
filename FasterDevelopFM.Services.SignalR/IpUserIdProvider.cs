using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Services.WebSocket
{


    public class IpUserIdProvider : IUserIdProvider
    {
        public IpUserIdProvider()
        {
          
        }

        public string GetUserId(HubConnectionContext connection)
        {
            //todo:应基于数据库固定数据，比如用户名 比如用户Id作为识别signal发送用户的标识
            return $"{connection.ConnectionId}"; 
           
        }
    }
}
