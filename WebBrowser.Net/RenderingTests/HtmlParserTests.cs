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
            Assert.AreEqual(new HtmlParser("<root test=fudge></root>").Root.InnerAttributes, "test=fudge");
        }

        [TestMethod]
        public void RootElementAttributeNoStringParse()
        {
            var x = new HtmlParser("<root test=fudge name=\"lol\"></root>");
            Assert.AreEqual(x.Root.Attributes["test"], "fudge");
        }

        [TestMethod]
        public void RootElementAttributeStringParse()
        {
            var x = new HtmlParser("<root test=fudge name=\"lol\"></root>");
            Assert.AreEqual(x.Root.Attributes["name"], "lol");
        }

        [TestMethod]
        public void NestedElementParse()
        {
            var x = new HtmlParser("<root test=fudge name=\"lol\"><head></head></root>");
            Assert.AreEqual(x.Root.Children[0].Name, "head");
        }

        [TestMethod]
        public void NestedElementCountParse()
        {
            var x = new HtmlParser("<root test=fudge name=\"lol\"><head></head><body></body></root>");
            Assert.AreEqual(x.Root.Children.Count, 2);
        }

    }
}
