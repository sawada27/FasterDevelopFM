using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Infrastructure
{
    /// <summary>
    /// 实体基类
    /// </summary>
    internal class EntityBase
    {
        /// <summary>
        /// 删除标识
        /// </summary>
        [Description("删除标识")]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        public long? CreateUser { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除人
        /// </summary>
        [Description("删除人")]
        public long? DeleteUser { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Description("更新时间")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        [Description("更新人")]
        public long? UpdateUser { get; set; }
    }

}
