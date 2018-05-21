namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class CharUnitPredicate
	{
		private readonly char _expectedChar;

		internal CharUnitPredicate(char ch)
		{
			_expectedChar = ch;
		}

		internal bool Match(CharUnit charUnit)
		{
			return charUnit.Char == _expectedChar;
		}
	}
}
