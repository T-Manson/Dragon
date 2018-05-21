namespace Dragon.Framework.Infrastructure.Pinyin
{
    internal class StrokeUnitPredicate
	{
		private readonly int _expectedStrokeNum;

		internal StrokeUnitPredicate(int strokeNum)
		{
			_expectedStrokeNum = strokeNum;
		}

		internal bool Match(StrokeUnit strokeUnit)
		{
			return strokeUnit.StrokeNumber == _expectedStrokeNum;
		}
	}
}
