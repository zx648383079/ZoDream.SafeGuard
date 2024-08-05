using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Plugins.Compress;

namespace ZoDream.Tests
{
    [TestClass]
    public class CompressTest
    {

        [TestMethod]
        public void TestByte()
        {
            Assert.AreEqual(InflateStream.Clamp(-361, 360), 359);
        }
    }
}
