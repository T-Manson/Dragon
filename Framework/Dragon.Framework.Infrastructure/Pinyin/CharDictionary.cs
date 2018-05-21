using Dragon.Framework.Infrastructure.Pinyin.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class CharDictionary
    {
        internal int Length;

        internal int Count;

        internal short Offset;

        internal readonly byte[] Reserved = new byte[24];

        internal List<CharUnit> CharUnitTable;

        internal readonly short EndMark = 32767;

        internal void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Length);
            binaryWriter.Write(Count);
            binaryWriter.Write(Offset);
            binaryWriter.Write(Reserved);
            for (var i = 0; i < Count; i++)
            {
                CharUnitTable[i].Serialize(binaryWriter);
            }
            binaryWriter.Write(EndMark);
        }

        internal static CharDictionary Deserialize(BinaryReader binaryReader)
        {
            var charDictionary = new CharDictionary();
            binaryReader.ReadInt32();
            charDictionary.Length = binaryReader.ReadInt32();
            charDictionary.Count = binaryReader.ReadInt32();
            charDictionary.Offset = binaryReader.ReadInt16();
            binaryReader.ReadBytes(24);
            charDictionary.CharUnitTable = new List<CharUnit>();
            for (var i = 0; i < charDictionary.Count; i++)
            {
                charDictionary.CharUnitTable.Add(CharUnit.Deserialize(binaryReader));
            }
            binaryReader.ReadInt16();
            return charDictionary;
        }

        internal CharUnit GetCharUnit(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), AssemblyResource.INDEX_OUT_OF_RANGE);
            }
            return CharUnitTable[index];
        }

        internal CharUnit GetCharUnit(char ch)
        {
            var @object = new CharUnitPredicate(ch);
            return CharUnitTable.Find(@object.Match);
        }
    }
}
