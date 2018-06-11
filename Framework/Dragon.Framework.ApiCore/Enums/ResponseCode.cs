namespace Dragon.Framework.ApiCore.Enums
{
    /// <summary>
    /// 返回码
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// OK
        /// </summary>
        OK = 200,

        /// <summary>
        /// NotLogin
        /// </summary>
        NotLogin = 401,

        /// <summary>
        /// NotPermission
        /// </summary>
        NoPermission = 403,

        /// <summary>
        /// Failure
        /// </summary>
        Failure = 500,

        /// <summary>
        /// DBFailure
        /// </summary>
        DBFailure = 701,

        /// <summary>
        /// RPCFailure
        /// </summary>
        RPCFailure = 702,

        /// <summary>
        /// ArgumentFailure
        /// </summary>
        ArgumentFailure = 703
    }
}