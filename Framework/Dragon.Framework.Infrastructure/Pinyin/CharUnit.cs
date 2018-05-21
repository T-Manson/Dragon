using System.IO;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class CharUnit
	{
		internal char Char;

		internal byte StrokeNumber;

		internal byte PinyinCount;

		internal short[] PinyinIndexList;

		internal static CharUnit Deserialize(BinaryReader binaryReader)
		{
		    var charUnit = new CharUnit
		    {
		        Char = binaryReader.ReadChar(),
		        StrokeNumber = binaryReader.ReadByte(),
		        PinyinCount = binaryReader.ReadByte()
		    };
		    charUnit.PinyinIndexList = new short[charUnit.PinyinCount];
			for (var i = 0; i < charUnit.PinyinCount; i++)
			{
				charUnit.PinyinIndexList[i] = binaryReader.ReadInt16();
			}
			return charUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(Char);
			binaryWriter.Write(StrokeNumber);
			binaryWriter.Write(PinyinCount);
			for (var i = 0; i < PinyinCount; i++)
			{
				binaryWriter.Write(PinyinIndexList[i]);
			}
		}
	}
}
