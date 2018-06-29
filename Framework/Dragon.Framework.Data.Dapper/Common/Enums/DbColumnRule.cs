using System;

namespace Dragon.Framework.Data.Dapper.Common.Enums
{
    /// <summary>
    /// DB字段规则（位枚举）
    /// </summary>
    [Flags]
    public enum DbColumnRule : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 忽略
        /// </summary>
        Ignore = 0x02,

        /// <summary>
        /// 主键
        /// </summary>
        Key = 0x04,

        /// <summary>
        /// 自增
        /// </summary>
        AutoGeneration = 0x08
    }
}
