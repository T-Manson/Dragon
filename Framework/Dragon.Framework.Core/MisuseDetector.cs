using Dragon.Framework.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dragon.Framework.Core
{
    /// <summary>
    /// 用于防止资源滥用的类。
    /// </summary>
    public class MisuseDetector
    {
        /// <summary>
        /// 需要检测的类型
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly Lazy<ILogger> _loggerLazy;

        /// <summary>
        /// 允许新实例的个数
        /// </summary>
        private readonly int _maxInstanceNum;

        /// <summary>
        /// 存活中的实例数量
        /// </summary>
        private long _activeInstanceNum;

        /// <summary>
        /// 是否已经记录日志
        /// </summary>
        private int _logged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">需要检测的类型</param>
        /// <param name="loggerFactory">日志工厂</param>
        /// <param name="maxInstanceNum">允许新实例的个数</param>
        public MisuseDetector(Type type, ILoggerFactory loggerFactory, int maxInstanceNum)
            : this(type, loggerFactory?.CreateLogger<MisuseDetector>(), maxInstanceNum)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">需要检测的类型</param>
        /// <param name="logger">日志</param>
        /// <param name="maxInstanceNum">允许新实例的个数</param>
        public MisuseDetector(Type type, ILogger logger, int maxInstanceNum)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            if (maxInstanceNum < 1)
                throw new ArgumentException("maxInstanceNum数量必须大于1");

            _type = type;
            _maxInstanceNum = maxInstanceNum;
            _loggerLazy = new Lazy<ILogger>(() => logger ?? NullLogger<MisuseDetector>.Instance);
        }

        /// <summary>
        /// 增加资源 存活中的实例数量。
        /// </summary>
        public void Increase()
        {
            Task.Run(() =>
            {
                if (Interlocked.Increment(ref _activeInstanceNum) > _maxInstanceNum)
                {
                    if (_loggerLazy.Value.IsEnabled(LogLevel.Warning) && Interlocked.CompareExchange(ref _logged, 0, 1) == 0)
                    {
                        _loggerLazy.Value.LogWarning(
                                $"检测到创建过多的 {_type.Name} 类型实例（实例警告上限：{_maxInstanceNum}）, {_type.Name} 是稀缺资源，请考虑减少它的实例（当前实例数量：{_activeInstanceNum}）。");
                    }
                }
            });
        }

        /// <summary>
        /// 减少资源 存活中的实例数量。
        /// </summary>
        public void Decrease()
        {
            Task.Run(() => Interlocked.Decrement(ref _activeInstanceNum));
        }
    }

}
