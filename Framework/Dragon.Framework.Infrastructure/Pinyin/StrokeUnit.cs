using System.IO;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class StrokeUnit
	{
		internal byte StrokeNumber;

		internal short CharCount;

		internal char[] CharList;

		internal static StrokeUnit Deserialize(BinaryReader binaryReader)
		{
		    var strokeUnit = new StrokeUnit
		    {
		        StrokeNumber = binaryReader.ReadByte(),
		        CharCount = binaryReader.ReadInt16()
		    };
		    strokeUnit.CharList = new char[strokeUnit.CharCount];
			for (var i = 0; i < strokeUnit.CharCount; i++)
			{
				strokeUnit.CharList[i] = binaryReader.ReadChar();
			}
			return strokeUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			if (CharCount == 0)
			{
				return;
			}
			binaryWriter.Write(StrokeNumber);
			binaryWriter.Write(CharCount);
			binaryWriter.Write(CharList);
		}
	}
}
