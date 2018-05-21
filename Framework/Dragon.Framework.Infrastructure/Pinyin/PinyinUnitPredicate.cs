using System.Globalization;

namespace Dragon.Framework.Infrastructure.Pinyin
{
	internal class PinyinUnitPredicate
	{
		private readonly string _expectedPinyin;

		internal PinyinUnitPredicate(string pinyin)
		{
			_expectedPinyin = pinyin;
		}

		internal bool Match(PinyinUnit pinyinUnit)
		{
			return string.Compare(pinyinUnit.Pinyin, _expectedPinyin, true, CultureInfo.CurrentCulture) == 0;
		}
	}
}
