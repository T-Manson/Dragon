using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Dragon.Framework.Infrastructure.Pinyin.Resources;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    /// <summary>
    /// 封装了简体中文的读音和笔画等基本信息。
    /// </summary>
	public class ChineseChar
    {
        private static readonly CharDictionary CharDictionary;

        private static readonly PinyinDictionary PinyinDictionary;

        private static readonly StrokeDictionary StrokeDictionary;

        private static readonly HomophoneDictionary HomophoneDictionary;

        private readonly string[] _pinyinList = new string[8];
        /// <summary>
        /// 获取这个字符的拼音个数。
        /// </summary>
		public short PinyinCount { get; }

        /// <summary>
        /// 获取这个字符的笔画数。
        /// </summary>
        public short StrokeNumber { get; }

        /// <summary>
        /// 获取这个字符是否是多音字。
        /// </summary>
        public bool IsPolyphone { get; }

        /// <summary>
        /// 获取这个字符的拼音。该集合长度不能表示实际拼音个数，实际拼音个数请使用PinyinCount字段
        /// </summary>
        public ReadOnlyCollection<string> Pinyins => new ReadOnlyCollection<string>(_pinyinList);

        /// <summary>
        /// 获取这个汉字字符。
        /// </summary>
        public char ChineseCharacter { get; }

        static ChineseChar()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var manifestResourceStream = executingAssembly.GetManifestResourceStream("Dragon.Framework.Infrastructure.Pinyin.Resources.PinyinDictionary.resources"))
            {
                using (var resourceReader = new ResourceReader(manifestResourceStream ?? Stream.Null))
                {
                    resourceReader.GetResourceData("PinyinDictionary", out _, out var buffer);
                    using (var binaryReader = new BinaryReader(new MemoryStream(buffer)))
                    {
                        PinyinDictionary = PinyinDictionary.Deserialize(binaryReader);
                    }
                }
            }
            using (var manifestResourceStream2 = executingAssembly.GetManifestResourceStream("Dragon.Framework.Infrastructure.Pinyin.Resources.CharDictionary.resources"))
            {
                using (var resourceReader2 = new ResourceReader(manifestResourceStream2 ?? Stream.Null))
                {
                    resourceReader2.GetResourceData("CharDictionary", out _, out var buffer);
                    using (var binaryReader2 = new BinaryReader(new MemoryStream(buffer)))
                    {
                        CharDictionary = CharDictionary.Deserialize(binaryReader2);
                    }
                }
            }
            using (var manifestResourceStream3 = executingAssembly.GetManifestResourceStream("Dragon.Framework.Infrastructure.Pinyin.Resources.HomophoneDictionary.resources"))
            {
                using (var resourceReader3 = new ResourceReader(manifestResourceStream3 ?? Stream.Null))
                {
                    resourceReader3.GetResourceData("HomophoneDictionary", out _, out var buffer);
                    using (var binaryReader3 = new BinaryReader(new MemoryStream(buffer)))
                    {
                        HomophoneDictionary = HomophoneDictionary.Deserialize(binaryReader3);
                    }
                }
            }
            using (var manifestResourceStream4 = executingAssembly.GetManifestResourceStream("Dragon.Framework.Infrastructure.Pinyin.Resources.StrokeDictionary.resources"))
            {
                using (var resourceReader4 = new ResourceReader(manifestResourceStream4 ?? Stream.Null))
                {
                    resourceReader4.GetResourceData("StrokeDictionary", out _, out var buffer);
                    using (var binaryReader4 = new BinaryReader(new MemoryStream(buffer)))
                    {
                        StrokeDictionary = StrokeDictionary.Deserialize(binaryReader4);
                    }
                }
            }
        }

        /// <summary>
        /// ChineseChar类的构造函数。
        /// </summary>
        /// <param name="ch">指定的汉字字符。</param>
		public ChineseChar(char ch)
        {
            if (!IsValidChar(ch))
            {
                throw new NotSupportedException(AssemblyResource.CHARACTER_NOT_SUPPORTED);
            }
            ChineseCharacter = ch;
            var charUnit = CharDictionary.GetCharUnit(ch);
            StrokeNumber = charUnit.StrokeNumber;
            PinyinCount = charUnit.PinyinCount;
            IsPolyphone = charUnit.PinyinCount > 1;
            for (var i = 0; i < PinyinCount; i++)
            {
                var pinYinUnitByIndex = PinyinDictionary.GetPinYinUnitByIndex(charUnit.PinyinIndexList[i]);
                _pinyinList[i] = pinYinUnitByIndex.Pinyin;
            }
        }

        /// <summary>
        /// 识别字符是否有指定的读音。
        /// </summary>
        /// <param name="pinyin">指定的需要被识别的拼音</param>
        /// <returns>如果指定的拼音字符串在实例字符的拼音集合中则返回ture，否则返回false。 </returns>
        public bool HasSound(string pinyin)
        {
            if (pinyin == null)
            {
                throw new ArgumentNullException(nameof(pinyin));
            }
            for (var i = 0; i < PinyinCount; i++)
            {
                if (string.Compare(Pinyins[i], pinyin, true, CultureInfo.CurrentCulture) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 识别给出的字符是否是实例字符的同音字。 
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>如果给出的实例字符是同音字则返回ture，否则返回false。</returns>
		public bool IsHomophone(char ch)
        {
            return IsHomophone(ChineseCharacter, ch);
        }

        /// <summary>
        /// 识别给出的两个字符是否是同音字。
        /// </summary>
        /// <param name="ch1">指出第一个字符</param>
        /// <param name="ch2">指出第二个字符</param>
        /// <returns>如果给出的字符是同音字返回ture，否则返回false。</returns>
		public static bool IsHomophone(char ch1, char ch2)
        {
            var charUnit = CharDictionary.GetCharUnit(ch1);
            var charUnit2 = CharDictionary.GetCharUnit(ch2);
            return ExistSameElement(charUnit.PinyinIndexList, charUnit2.PinyinIndexList);
        }

        /// <summary>
        /// 将给出的字符和实例字符的笔画数进行比较。
        /// </summary>
        /// <param name="ch">显示给出的字符</param>
        /// <returns>说明比较操作的结果。 如果给出字符和实例字符的笔画数相同，返回值为 0。 如果实例字符比给出字符的笔画多，返回值为> 0。 如果实例字符比给出字符的笔画少，返回值为 &lt; 0。 </returns>
		public int CompareStrokeNumber(char ch)
        {
            var charUnit = CharDictionary.GetCharUnit(ch);
            return StrokeNumber - charUnit.StrokeNumber;
        }

        /// <summary>
        /// 获取给定拼音的所有同音字。
        /// </summary>
        /// <param name="pinyin">指出读音。</param>
        /// <returns>返回具有相同的指定拼音的字符串列表。 如果拼音不是有效值则返回空。 </returns>
		public static char[] GetChars(string pinyin)
        {
            if (pinyin == null)
            {
                throw new ArgumentNullException(nameof(pinyin));
            }
            if (!IsValidPinyin(pinyin))
            {
                return new char[] { };
            }
            var homophoneUnit = HomophoneDictionary.GetHomophoneUnit(PinyinDictionary, pinyin);
            return homophoneUnit.HomophoneList;
        }

        /// <summary>
        /// 识别给出的拼音是否是一个有效的拼音字符串。
        /// </summary>
        /// <param name="pinyin">指出需要识别的字符串。</param>
        /// <returns>如果指定的字符串是一个有效的拼音字符串则返回ture，否则返回false。</returns>
		public static bool IsValidPinyin(string pinyin)
        {
            if (pinyin == null)
            {
                throw new ArgumentNullException(nameof(pinyin));
            }
            return PinyinDictionary.GetPinYinUnitIndex(pinyin) >= 0;
        }

        /// <summary>
        /// 识别给出的字符串是否是一个有效的汉字字符。
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>如果指定的字符是一个有效的汉字字符则返回ture，否则返回false。</returns>
		public static bool IsValidChar(char ch)
        {
            var charUnit = CharDictionary.GetCharUnit(ch);
            return charUnit != null;
        }

        /// <summary>
        /// 识别给出的笔画数是否是一个有效的笔画数。
        /// </summary>
        /// <param name="strokeNumber">指出需要识别的笔画数。</param>
        /// <returns>如果指定的笔画数是一个有效的笔画数则返回ture，否则返回false。</returns>
		public static bool IsValidStrokeNumber(short strokeNumber)
        {
            if (strokeNumber < 0 || strokeNumber > 48)
            {
                return false;
            }
            var strokeUnit = StrokeDictionary.GetStrokeUnit(strokeNumber);
            return strokeUnit != null;
        }

        /// <summary>
        /// 检索具有指定拼音的字符数。
        /// </summary>
        /// <param name="pinyin">显示需要被识别的拼音字符串。</param>
        /// <returns>返回具有指定拼音的字符数。 如果拼音不是有效值则返回-1。</returns>
		public static short GetHomophoneCount(string pinyin)
        {
            if (pinyin == null)
            {
                throw new ArgumentNullException(nameof(pinyin));
            }
            if (!IsValidPinyin(pinyin))
            {
                return -1;
            }
            return HomophoneDictionary.GetHomophoneUnit(PinyinDictionary, pinyin).Count;
        }

        /// <summary>
        /// 检索指定字符的笔画数。 
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>返回指定字符的笔画数。 如果字符不是有效值则返回-1。 </returns>
		public static short GetStrokeNumber(char ch)
        {
            if (!IsValidChar(ch))
            {
                return -1;
            }
            var charUnit = CharDictionary.GetCharUnit(ch);
            return charUnit.StrokeNumber;
        }

        /// <summary>
        /// 检索具有指定笔画数的所有字符串。
        /// </summary>
        /// <param name="strokeNumber">指出需要被识别的笔画数。</param>
        /// <returns>返回具有指定笔画数的字符列表。 如果笔画数是无效值返回空。</returns>
		public static char[] GetChars(short strokeNumber)
        {
            if (!IsValidStrokeNumber(strokeNumber))
            {
                return new char[] { };
            }
            var strokeUnit = StrokeDictionary.GetStrokeUnit(strokeNumber);
            return strokeUnit.CharList;
        }

        /// <summary>
        /// 检索具有指定笔画数的字符个数。
        /// </summary>
        /// <param name="strokeNumber">显示需要被识别的笔画数。</param>
        /// <returns>返回具有指定笔画数的字符数。 如果笔画数是无效值返回-1。</returns>
		public static short GetCharCount(short strokeNumber)
        {
            if (!IsValidStrokeNumber(strokeNumber))
            {
                return -1;
            }
            return StrokeDictionary.GetStrokeUnit(strokeNumber).CharCount;
        }

        private static bool ExistSameElement<T>(T[] array1, T[] array2) where T : IComparable
        {
            var num = 0;
            var num2 = 0;
            while (num < array1.Length && num2 < array2.Length)
            {
                if (array1[num].CompareTo(array2[num2]) < 0)
                {
                    num++;
                }
                else
                {
                    if (array1[num].CompareTo(array2[num2]) <= 0)
                    {
                        return true;
                    }
                    num2++;
                }
            }
            return false;
        }
    }
}
