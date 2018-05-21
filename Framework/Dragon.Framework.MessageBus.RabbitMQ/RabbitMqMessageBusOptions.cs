namespace Dragon.Framework.MessageBus.RabbitMQ
{
    /// <summary>
    /// RabbitMq配置类
    /// </summary>
    public class RabbitMqMessageBusOptions
    {
        /// <summary>
        /// Uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary> 
        public string Password { get; set; }

        /// <summary>
        /// Exchange
        /// </summary>
        public string Exchange { get; set; }
    }
}
