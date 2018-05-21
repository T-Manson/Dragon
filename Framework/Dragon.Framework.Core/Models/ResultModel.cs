using System;

namespace Dragon.Framework.Core.Models
{
    /// <summary>
    /// 业务层返回封装类
    /// </summary>
    public abstract class ResultModel
    {
        /// <summary>
        /// 数据总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 是否出现异常
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 显示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string Extend { get; set; }

        /// <summary>
        /// 控制台输出提示
        /// </summary>
        public string ConsoleMsg { get; set; }
    }

    /// <summary>
    /// 业务层返回封装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultModel<T> : ResultModel
    {
        private readonly bool _hasException;

        private string _errorMessage;

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage
        {
            get => _hasException ? "操作出现异常" : _errorMessage;
            set => _errorMessage = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public ResultModel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public ResultModel(Exception ex)
        {
            HasError = true;
            _hasException = true;
            ErrorMessage = ex.Message;
            ConsoleMsg = ConsoleMsg = $"Message:{ ex.Message}  --->InnerException:{ex.InnerException?.Message ?? string.Empty}";
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Result { get; set; }
    }
}
