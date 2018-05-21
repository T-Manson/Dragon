using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static partial class ExtensionMethods
    {
        public static StreamReader GetReader(this Stream stream)
        {
            return stream.GetReader(null);
        }

        public static StreamReader GetReader(this Stream stream, Encoding encoding)
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream does not support reading.");
            encoding = (encoding ?? Encoding.UTF8);
            return new StreamReader(stream, encoding);
        }

        public static StreamWriter GetWriter(this Stream stream)
        {
            return stream.GetWriter(null);
        }

        public static StreamWriter GetWriter(this Stream stream, Encoding encoding)
        {
            if (!stream.CanWrite)
                throw new InvalidOperationException("Stream does not support writing.");
            
            encoding = (encoding ?? Encoding.UTF8);
            return new StreamWriter(stream, encoding);
        }

        public static string ReadToEnd(this Stream stream)
        {
            return stream.ReadToEnd(null);
        }

        public static string ReadToEnd(this Stream stream, Encoding encoding)
        {
            using (var reader = stream.GetReader(encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] ReadBytesToEnd(this Stream stream, int bufferSize = 4049)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Reading buffer size must rather than 0 .");
            }

            var bytes = new List<byte>();
            var buffer = new byte[bufferSize];
            using (var reader = new BinaryReader(stream))
            {
                int byteCount;
                while ((byteCount = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var readBytes = new byte[byteCount];
                    Buffer.BlockCopy(buffer, 0, readBytes, 0, readBytes.Length);
                    bytes.AddRange(readBytes);
                }
            }
            return bytes.ToArray();
        }
    }
}
