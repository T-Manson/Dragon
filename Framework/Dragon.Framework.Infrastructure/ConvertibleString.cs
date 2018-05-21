using System;
using System.Globalization;

namespace Dragon.Framework.Infrastructure
{
    /// <summary>
    /// 可变的字符串
    /// </summary>
    public class ConvertibleString
    {
        private readonly string _value;

        public ConvertibleString(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }

        #region implicit

        #region primitive


        public static implicit operator string(ConvertibleString obj)
        {
            return obj._value;
        }

        public static implicit operator short(ConvertibleString obj)
        {
            short.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator ushort(ConvertibleString obj)
        {
            ushort.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator int(ConvertibleString obj)
        {
            int.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator uint(ConvertibleString obj)
        {
            uint.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator long(ConvertibleString obj)
        {
            long.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator ulong(ConvertibleString obj)
        {
            ulong.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator byte(ConvertibleString obj)
        {
            byte.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator sbyte(ConvertibleString obj)
        {
            sbyte.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator decimal(ConvertibleString obj)
        {
            decimal.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator double(ConvertibleString obj)
        {
            double.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator float(ConvertibleString obj)
        {
            float.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out var result);
            return result;
        }

        public static implicit operator char(ConvertibleString obj)
        {
            char.TryParse(obj._value, out var result);
            return result;
        }

        public static implicit operator bool(ConvertibleString obj)
        {
            bool.TryParse(obj._value, out var result);
            return result;
        }

        public static implicit operator DateTime(ConvertibleString obj)
        {
            DateTime.TryParse(obj._value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out DateTime result);
            return result;
        }

        public static implicit operator Guid(ConvertibleString obj)
        {
            Guid.TryParse(obj._value, out Guid result);
            return result;
        }

        public static implicit operator byte[](ConvertibleString obj)
        {
            if (obj._value == null)
            {
                return null;
            }
            else
            {
                return Convert.FromBase64String(obj._value);
            }
        }

        #endregion

        #region nullable

        public static implicit operator short?(ConvertibleString obj)
        {
            short result;
            if (short.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator ushort?(ConvertibleString obj)
        {
            ushort result;
            if (ushort.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator int?(ConvertibleString obj)
        {
            int result;
            if (int.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator uint?(ConvertibleString obj)
        {
            uint result;
            if (uint.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator long?(ConvertibleString obj)
        {
            long result;
            if (long.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator ulong?(ConvertibleString obj)
        {
            ulong result;
            if (ulong.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator byte?(ConvertibleString obj)
        {
            byte result;
            if (byte.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator sbyte?(ConvertibleString obj)
        {
            sbyte result;
            if (sbyte.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator decimal?(ConvertibleString obj)
        {
            decimal result;
            if (decimal.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator double?(ConvertibleString obj)
        {
            double result;
            if (double.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator float?(ConvertibleString obj)
        {
            float result;
            if (float.TryParse(obj._value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator char?(ConvertibleString obj)
        {
            char result;
            if (char.TryParse(obj._value, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator bool?(ConvertibleString obj)
        {
            bool result;
            if (bool.TryParse(obj._value, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator DateTime?(ConvertibleString obj)
        {
            DateTime result;
            if (DateTime.TryParse(obj._value, out result))
            {
                return result;
            }
            return null;
        }

        public static implicit operator Guid?(ConvertibleString obj)
        {
            Guid result;
            if (Guid.TryParse(obj._value, out result))
            {
                return result;
            }
            return null;
        }

        #endregion

        #endregion
    }
}
