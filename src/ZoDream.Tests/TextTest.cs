using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.TextNetwork;

namespace ZoDream.Tests
{
    [TestClass]
    public class TextTest
    {
        const string BaseFolder = "D:\\Desktop\\分词";
        //[TestMethod]
        public void TestDict()
        {
            var dict = new CharDictionary();
            dict.LoadFromFolderAsync("D:\\zodream\\af\\小说\\精校").GetAwaiter().GetResult();
            //dict.LoadFromTxtAsync(Path.Combine(BaseFolder, "test.txt")).GetAwaiter().GetResult();
            dict.SaveAsync(Path.Combine(BaseFolder, "dict.map")).GetAwaiter().GetResult();
            Assert.AreEqual(dict.ToChar(0), ' ');
        }
        [TestMethod]
        public void TestCode()
        {
            var str = "ａ a ｂ b ｃ c ｄ d ｅ e ｆ f ｇ g ｈ h ｉ i ｊ j ｋ k ｌ l ｍ m ｎ n ｏ o ｐ p ｑ q ｒ r ｓ s ｔ t ｕ u ｖ v ｗ w ｘ x ｙ y ｚ z Ａ A Ｂ B Ｃ C Ｄ D Ｅ E Ｆ F Ｇ G Ｈ H Ｉ I Ｊ J Ｋ K Ｌ L Ｍ M Ｎ N Ｏ O Ｐ P Ｑ Q Ｒ R Ｓ S Ｔ T Ｕ U Ｖ V Ｗ W Ｘ X Ｙ Y Ｚ Z １ 1 ２ 2 ３ 3 ４ 4 ５ 5 ６ 6 ７ 7 ８ 8 ９ 9 ０ 0 ｀ ` ” \" ’ ' “ \" ‘ ' ＿ _ － - ～ ~ ＝ = ＋ + ＼ \\ ｜ | ／ / （ ( ） ) ［ [ ］ ] 【 [ 】 ] ｛ { ｝ } ＜ < ＞ > ． . ， , ； ; ： : ！ ! ＾ ^ ％ % ＃ # ＠ @ ＄ $ ＆ & ？ ? ＊ * 。 .";
            var items = str.Split(' ').Select(i => i[0]).ToArray();
            var data = new Dictionary<char, char>();
            for (int i = 0; i < items.Length; i+= 2)
            {
                data.Add(items[i], items[i + 1]);
            }
            data = data.OrderBy(i => i.Value).ToDictionary();
            var begin = Convert.ToChar('〃' + 1);
            //for (int i = 0; i < 2; i++)
            //{
            //    data.Add(Convert.ToChar(begin + i), Convert.ToChar(begin + i));
            //}
            Assert.AreEqual(data.Count, 99);
        }

        [TestMethod]
        public void TestSub()
        {
            var text = "12345577";
            var res = text[0..(text.Length - 1)];
            Assert.AreEqual(text, res);
        }
    }
}
