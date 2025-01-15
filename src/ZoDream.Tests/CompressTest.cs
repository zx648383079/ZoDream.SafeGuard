using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Tests
{
    [TestClass]
    public class CompressTest
    {

        [TestMethod]
        public void TestByte()
        {
            Assert.AreEqual((25 >> 0) & 0xff, 25);
            // Assert.AreEqual(InflateStream.Clamp(-361, 360), 359);
        }

        [TestMethod]
        public void TestPath()
        {
            var folder = "D:\\Desktop";
            folder = Path.GetFullPath(folder);
            var data = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            foreach (var file in data)
            {
                var t = Path.GetRelativePath(folder, file);
                Assert.IsNotNull(t);
            }
        }
    }
}
