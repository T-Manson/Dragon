using System;

namespace Dragon.Framework.ApiCore.Models
{
    /// <summary>
    /// 基础查询实体
    /// </summary>
    public abstract class BaseSearchModel
    {
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool NeedPaging
        {
            get; set;
        }

        private int _pageIndex;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("pageIndex必须大于0");
                _pageIndex = value;
            }
        }

        private int _pageSize;
        /// <summary>
        /// 条数
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("pageSize必须大于0");
                _pageSize = value;
            }
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalSize
        {
            get; set;
        }

        /// <summary>
        /// 排序字段 对应配置的字段名
        /// </summary>
        public string SortField
        {
            get; set;
        }

        private string _sortOrder;
        /// <summary>
        /// 排序方式 升序：asc 降序： desc
        /// </summary>
        public string SortOrder
        {
            get => _sortOrder;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value != "asc" && value != "desc")
                    throw new ArgumentException("sortOrder值定义应为asc或desc");
                _sortOrder = value;
            }
        }
    }

    /// <summary>
    /// 基础查询实体(带有查询筛选)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSearchModel<T> : BaseSearchModel
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseSearchModel()
        {
            NeedPaging = true;
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        public T ViewModel
        {
            get; set;
        }
    }
}
