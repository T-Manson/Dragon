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
    /// ��װ�˼������ĵĶ����ͱʻ��Ȼ�����Ϣ��
    /// </summary>
	public class ChineseChar
    {
        private static readonly CharDictionary CharDictionary;

        private static readonly PinyinDictionary PinyinDictionary;

        private static readonly StrokeDictionary StrokeDictionary;

        private static readonly HomophoneDictionary HomophoneDictionary;

        private readonly string[] _pinyinList = new string[8];
        /// <summary>
        /// ��ȡ����ַ���ƴ��������
        /// </summary>
		public short PinyinCount { get; }

        /// <summary>
        /// ��ȡ����ַ��ıʻ�����
        /// </summary>
        public short StrokeNumber { get; }

        /// <summary>
        /// ��ȡ����ַ��Ƿ��Ƕ����֡�
        /// </summary>
        public bool IsPolyphone { get; }

        /// <summary>
        /// ��ȡ����ַ���ƴ�����ü��ϳ��Ȳ��ܱ�ʾʵ��ƴ��������ʵ��ƴ��������ʹ��PinyinCount�ֶ�
        /// </summary>
        public ReadOnlyCollection<string> Pinyins => new ReadOnlyCollection<string>(_pinyinList);

        /// <summary>
        /// ��ȡ��������ַ���
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
        /// ChineseChar��Ĺ��캯����
        /// </summary>
        /// <param name="ch">ָ���ĺ����ַ���</param>
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
        /// ʶ���ַ��Ƿ���ָ���Ķ�����
        /// </summary>
        /// <param name="pinyin">ָ������Ҫ��ʶ���ƴ��</param>
        /// <returns>���ָ����ƴ���ַ�����ʵ���ַ���ƴ���������򷵻�ture�����򷵻�false�� </returns>
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
        /// ʶ��������ַ��Ƿ���ʵ���ַ���ͬ���֡� 
        /// </summary>
        /// <param name="ch">ָ����Ҫʶ����ַ���</param>
        /// <returns>���������ʵ���ַ���ͬ�����򷵻�ture�����򷵻�false��</returns>
		public bool IsHomophone(char ch)
        {
            return IsHomophone(ChineseCharacter, ch);
        }

        /// <summary>
        /// ʶ������������ַ��Ƿ���ͬ���֡�
        /// </summary>
        /// <param name="ch1">ָ����һ���ַ�</param>
        /// <param name="ch2">ָ���ڶ����ַ�</param>
        /// <returns>����������ַ���ͬ���ַ���ture�����򷵻�false��</returns>
		public static bool IsHomophone(char ch1, char ch2)
        {
            var charUnit = CharDictionary.GetCharUnit(ch1);
            var charUnit2 = CharDictionary.GetCharUnit(ch2);
            return ExistSameElement(charUnit.PinyinIndexList, charUnit2.PinyinIndexList);
        }

        /// <summary>
        /// ���������ַ���ʵ���ַ��ıʻ������бȽϡ�
        /// </summary>
        /// <param name="ch">��ʾ�������ַ�</param>
        /// <returns>˵���Ƚϲ����Ľ���� ��������ַ���ʵ���ַ��ıʻ�����ͬ������ֵΪ 0�� ���ʵ���ַ��ȸ����ַ��ıʻ��࣬����ֵΪ> 0�� ���ʵ���ַ��ȸ����ַ��ıʻ��٣�����ֵΪ &lt; 0�� </returns>
		public int CompareStrokeNumber(char ch)
        {
            var charUnit = CharDictionary.GetCharUnit(ch);
            return StrokeNumber - charUnit.StrokeNumber;
        }

        /// <summary>
        /// ��ȡ����ƴ��������ͬ���֡�
        /// </summary>
        /// <param name="pinyin">ָ��������</param>
        /// <returns>���ؾ�����ͬ��ָ��ƴ�����ַ����б� ���ƴ��������Чֵ�򷵻ؿա� </returns>
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
        /// ʶ�������ƴ���Ƿ���һ����Ч��ƴ���ַ�����
        /// </summary>
        /// <param name="pinyin">ָ����Ҫʶ����ַ�����</param>
        /// <returns>���ָ�����ַ�����һ����Ч��ƴ���ַ����򷵻�ture�����򷵻�false��</returns>
		public static bool IsValidPinyin(string pinyin)
        {
            if (pinyin == null)
            {
                throw new ArgumentNullException(nameof(pinyin));
            }
            return PinyinDictionary.GetPinYinUnitIndex(pinyin) >= 0;
        }

        /// <summary>
        /// ʶ��������ַ����Ƿ���һ����Ч�ĺ����ַ���
        /// </summary>
        /// <param name="ch">ָ����Ҫʶ����ַ���</param>
        /// <returns>���ָ�����ַ���һ����Ч�ĺ����ַ��򷵻�ture�����򷵻�false��</returns>
		public static bool IsValidChar(char ch)
        {
            var charUnit = CharDictionary.GetCharUnit(ch);
            return charUnit != null;
        }

        /// <summary>
        /// ʶ������ıʻ����Ƿ���һ����Ч�ıʻ�����
        /// </summary>
        /// <param name="strokeNumber">ָ����Ҫʶ��ıʻ�����</param>
        /// <returns>���ָ���ıʻ�����һ����Ч�ıʻ����򷵻�ture�����򷵻�false��</returns>
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
        /// ��������ָ��ƴ�����ַ�����
        /// </summary>
        /// <param name="pinyin">��ʾ��Ҫ��ʶ���ƴ���ַ�����</param>
        /// <returns>���ؾ���ָ��ƴ�����ַ����� ���ƴ��������Чֵ�򷵻�-1��</returns>
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
        /// ����ָ���ַ��ıʻ����� 
        /// </summary>
        /// <param name="ch">ָ����Ҫʶ����ַ���</param>
        /// <returns>����ָ���ַ��ıʻ����� ����ַ�������Чֵ�򷵻�-1�� </returns>
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
        /// ��������ָ���ʻ����������ַ�����
        /// </summary>
        /// <param name="strokeNumber">ָ����Ҫ��ʶ��ıʻ�����</param>
        /// <returns>���ؾ���ָ���ʻ������ַ��б� ����ʻ�������Чֵ���ؿա�</returns>
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
        /// ��������ָ���ʻ������ַ�������
        /// </summary>
        /// <param name="strokeNumber">��ʾ��Ҫ��ʶ��ıʻ�����</param>
        /// <returns>���ؾ���ָ���ʻ������ַ����� ����ʻ�������Чֵ����-1��</returns>
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
