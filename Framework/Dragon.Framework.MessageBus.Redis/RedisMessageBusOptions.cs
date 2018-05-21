namespace Dragon.Framework.MessageBus.Redis
{
    /// <summary>
    /// Redis配置类
    /// </summary>
    public class RedisMessageBusOptions
    {
        /// <summary>
        /// IP
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Authorization
        /// </summary> 
        public string Password { get; set; }

        /// <summary>
        /// DB
        /// </summary>
        public int Db { get; set; }
    }
}
