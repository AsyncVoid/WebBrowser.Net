using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WBN.Render;

namespace RenderingTests
{
    [TestClass]
    public class HtmlParserTests
    {
        [TestMethod]
        public void LoadTest()
        {
            Assert.AreEqual(new HtmlParser("<html></html>").Length, 13);
        }

        [TestMethod]
        public void RootElementParse()
        {
            Assert.AreEqual(new HtmlParser("<root></root>").Root.Name, "root");
        }

        [TestMethod]
        public void RootElementInnerHtmlParse()
        {
            Assert.AreEqual(new HtmlParser("<root>test</root>").Root.InnerHtml, "test");
        }

        [TestMethod]
        public void RootElementInnerAttributeParse()
        {
            Assert.AreEqual(new HtmlParser("<root test=fudge></root>").Root.InnerAttribute, "test=fudge");
        }
    }
}
