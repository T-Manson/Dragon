using System.IO;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class HomophoneUnit
    {
        internal short Count;

        internal char[] HomophoneList;

        internal static HomophoneUnit Deserialize(BinaryReader binaryReader)
        {
            var homophoneUnit = new HomophoneUnit { Count = binaryReader.ReadInt16() };
            homophoneUnit.HomophoneList = new char[homophoneUnit.Count];
            for (var i = 0; i < homophoneUnit.Count; i++)
            {
                homophoneUnit.HomophoneList[i] = binaryReader.ReadChar();
            }
            return homophoneUnit;
        }

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Count);
            for (var i = 0; i < Count; i++)
            {
                binaryWriter.Write(HomophoneList[i]);
            }
        }
    }
}
