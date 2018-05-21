using System.Collections.Generic;
using System.Linq;
using Dragon.Framework.Infrastructure.Pinyin;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// 拼音帮助类
    /// </summary>
    public static class PinyinHelper
    {
        /// <summary>
        /// 获取拼音首字母（出现多音字时将返回所有排列组合结果以英文逗号分隔）
        /// </summary>
        /// <param name="words">中文</param>
        /// <returns>英文逗号分隔的字符串</returns>
        public static string GetPinyinFirst(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
                return string.Empty;

            return string.Join(",", GetPinyinFirstList(words)).ToLower();
        }

        /// <summary>
        ///  获取拼音首字母集合
        /// </summary>
        /// <param name="words">中文</param>
        /// <returns>拼音首字母集合</returns>
        public static IEnumerable<string> GetPinyinFirstList(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
                return new List<string>();

            var list = GetPinyinCartesianProduct(words);
            return list.Select(l => string.Join("", l.Select(s => s.Substring(0, 1)))).Distinct();
        }

        /// <summary>
        /// 获取拼音（出现多音字时将返回所有排列组合结果以英文逗号分隔）
        /// </summary>
        /// <param name="words">中文</param>
        /// <returns>英文逗号分隔的字符串</returns>
        public static string GetPinyin(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
                return string.Empty;

            return string.Join(",", GetPinyinList(words)).ToLower();
        }

        /// <summary>
        /// 获取拼音集合
        /// </summary>
        /// <param name="words">中文</param>
        /// <returns>拼音集合</returns>
        public static IEnumerable<string> GetPinyinList(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
                return new List<string>();

            var list = GetPinyinCartesianProduct(words);
            return list.Select(l => string.Join("", l));
        }

        #region 公共方法

        /// <summary>
        /// 获取转换后的笛卡尔积结果
        /// </summary>
        /// <param name="words">需要转换的字符串</param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<string>> GetPinyinCartesianProduct(string words)
        {
            var list = new List<List<string>>();
            var array = words.Trim().ToCharArray();

            foreach (var item in array)
            {
                if (string.IsNullOrWhiteSpace(item.ToString()))
                    continue;

                if (!ChineseChar.IsValidChar(item))
                {
                    var self = new List<string> { item.ToString() };
                    list.Add(self);
                    continue;
                }

                var pinyin = new ChineseChar(item);
                var pinyins = pinyin.Pinyins.Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Substring(0, s.Length - 1))
                    .Distinct()
                    .ToList();

                list.Add(pinyins);
            }

            return CartesianProduct(list);
        }

        /// <summary>
        /// 笛卡尔积算法合并list算法
        /// </summary>
        private static List<List<T>> CartesianProduct<T>(List<List<T>> list)
        {
            var count = 1;
            list.ForEach(item => count *= item.Count);

            var lstResult = new List<List<T>>();
            for (var i = 0; i < count; ++i)
            {
                var lstTemp = new List<T>();
                var j = 1;
                var index = i;
                list.ForEach(item =>
                {
                    j *= item.Count;
                    lstTemp.Add(item[index / (count / j) % item.Count]);
                });
                lstResult.Add(lstTemp);
            }
            return lstResult;
        }

        #endregion
    }
}
