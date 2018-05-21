using System;

namespace Dragon.Framework.ApiCore.Models
{
    /// <summary>
    /// 分页数据实体
    /// </summary>
    public class PagedListDataModel<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PagedListDataModel()
        {
            List = (T)Activator.CreateInstance(typeof(T));
            PageInfo = new PageInfo { Total = 0 };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="total">总数</param>
        public PagedListDataModel(T data, int total)
        {
            if (data != null)
                List = data;
            if (total >= 0)
                PageInfo = new PageInfo { Total = total };
        }

        /// <summary>
        /// 返回数据 
        /// </summary>
        public T List { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PageInfo { get; set; }
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 数据总条数
        /// </summary>
        public int Total { get; set; }
    }
}
