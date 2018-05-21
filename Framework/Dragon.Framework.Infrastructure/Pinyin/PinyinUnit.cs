using System.IO;
using System.Text;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class PinyinUnit
	{
		internal string Pinyin;

		internal static PinyinUnit Deserialize(BinaryReader binaryReader)
		{
			var pinyinUnit = new PinyinUnit();
			var bytes = binaryReader.ReadBytes(7);
			pinyinUnit.Pinyin = Encoding.ASCII.GetString(bytes, 0, 7);
			var array = new char[1];
			var trimChars = array;
			pinyinUnit.Pinyin = pinyinUnit.Pinyin.TrimEnd(trimChars);
			return pinyinUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			var array = new byte[7];
			Encoding.ASCII.GetBytes(Pinyin, 0, Pinyin.Length, array, 0);
			binaryWriter.Write(array);
		}
	}
}
