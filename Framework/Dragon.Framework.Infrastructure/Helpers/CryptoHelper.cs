using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 加密解密工具类
    /// </summary>
    public static class CryptoHelper
    {
        #region MD5

        /// <summary>
        /// 采用 MD5 32位加密数据
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="encoding">编码，为空使用 UTF-8 编码。</param>
        /// <param name="useBase64String">指示是否对结果使用BASE64编码。</param>
        /// <returns>加密后的数据</returns>
        public static string Encrypt32Md5(string data, Encoding encoding = null, bool useBase64String = true)
        {
            if (data.IsNullOrWhiteSpace()) return String.Empty;

            encoding = encoding ?? Encoding.UTF8;
            using (var md5 = MD5.Create())
            {
                var buffer = md5.ComputeHash(encoding.GetBytes(data));
                var result = useBase64String ? Convert.ToBase64String(buffer) : encoding.GetString(buffer);
                return result;
            }
        }

        #endregion

        #region RSA

        /// <summary>
        /// 采用 RSA 加密
        /// </summary>
        /// <param name="cerFilePath">公钥证书文件</param>
        /// <param name="secretData">加密的数据</param>
        /// <param name="isOaep">是否使用OAEP填充方式</param>
        /// <returns></returns>
        public static object RsaEncrypt(string cerFilePath, byte[] secretData, bool isOaep = false)
        {
            RSACryptoServiceProvider provider = (RSACryptoServiceProvider)GetPublicCertFromPfx(cerFilePath).PublicKey.Key;
            return provider.Encrypt(secretData, isOaep);
        }

        /// <summary>
        /// 采用 RSA 解密
        /// </summary>
        /// <param name="pfxFilePath">PFX证书文件</param>
        /// <param name="password">密码</param>
        /// <param name="secretData">加密的数据</param>
        /// <param name="isOaep">是否使用OAEP填充方式</param>
        /// <returns></returns>
        public static object RsaDecrypt(string pfxFilePath, string password, byte[] secretData, bool isOaep = false)
        {
            RSACryptoServiceProvider provider = (RSACryptoServiceProvider)GetPrivateCertFromPfx(pfxFilePath, password).PrivateKey;
            return provider.Decrypt(secretData, isOaep);
        }

        #region 公共方法

        /// <summary>
        /// 根据公钥证书文件获取公钥证书
        /// </summary>
        /// <param name="cerFilePath">公钥证书文件</param>
        /// <returns></returns>
        private static X509Certificate2 GetPublicCertFromPfx(string cerFilePath)
        {
            return new X509Certificate2(cerFilePath);
        }

        /// <summary>
        /// 根据PFX文件获取私钥证书
        /// </summary>
        /// <param name="pfxFilePath">PFX加密的证书文件</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static X509Certificate2 GetPrivateCertFromPfx(string pfxFilePath, string password)
        {
            return new X509Certificate2(pfxFilePath, password);
        }

        #endregion

        #endregion
    }
}
