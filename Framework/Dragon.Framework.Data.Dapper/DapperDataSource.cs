using Dragon.Framework.Data.Dapper.Abstractions;
using System;

namespace Dragon.Framework.Data.Dapper
{
    /// <summary>
    /// Dapper数据源
    /// </summary>
    public class DapperDataSource : IEquatable<DapperDataSource>
    {
        /// <summary>
        /// 数据提供类
        /// </summary>
        public IDatabaseProvider DatabaseProvider { get; }

        /// <summary>
        /// 读连接串配置名称
        /// </summary>
        public string ReadingConnectionName { get; }

        /// <summary>
        /// 写连接串配置名称
        /// </summary>
        public string WritingConnectionName { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="readingConnectionName"></param>
        /// <param name="writingConnectionName"></param>
        internal DapperDataSource(IDatabaseProvider provider, string readingConnectionName, string writingConnectionName)
        {
            DatabaseProvider = provider;
            ReadingConnectionName = readingConnectionName;
            WritingConnectionName = writingConnectionName;
        }

        /// <summary>
        /// 数据源对象相等比较
        /// </summary>
        /// <param name="other">另一个数据源对象</param>
        /// <returns></returns>
        public bool Equals(DapperDataSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(DatabaseProvider, other.DatabaseProvider) && string.Equals(ReadingConnectionName, other.ReadingConnectionName) && string.Equals(WritingConnectionName, other.WritingConnectionName);
        }

        /// <summary>
        /// 对象相等比较
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DapperDataSource)obj);
        }

        /// <summary>
        /// 获取HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DatabaseProvider != null ? DatabaseProvider.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (ReadingConnectionName != null ? ReadingConnectionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (WritingConnectionName != null ? WritingConnectionName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
