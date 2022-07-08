using Imagize.Core;

namespace ImagizeTests
{
    [TestClass]
    public class UriTests
    {
      
        [TestMethod]
        public void AllowedOriginsTests()
        {
            HttpTools httpTools = new HttpTools(new HttpClient());

            Assert.IsFalse(httpTools.IsValidOrigin("http://yahoo.com/page/anotherpage/image.jpeg",
                "http://microsoft.com|http://google.com|https://another.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://www.microsoft.com", 
                "http://www.microsoft.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://microsoft.com", 
                "http://microsoft.com|http://google.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://google.com", 
                "http://microsoft.com|http://google.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("https://google.com", 
                "http://microsoft.com|http://google.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://another.com", 
                "http://microsoft.com|http://google.com|https://another.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://microsoft.com/page/anotherpage/image.jpeg",
                "http://microsoft.com|http://google.com|https://another.com"));
            Assert.IsTrue(httpTools.IsValidOrigin("http://google.com/page/anotherpage/image.jpeg",
                "http://microsoft.com|http://google.com|https://another.com"));
            Assert.IsFalse(httpTools.IsValidOrigin("https://s3.eu-west-2.amazonaws.com/a-sub-domain/25.JPG",
                "http://microsoft.com|http://google.com|https://another.com|https://s3.eu-west-2.amazonaws.com/old.markcastle.com-images"));
            Assert.IsTrue(httpTools.IsValidOrigin("https://s3.eu-west-2.amazonaws.com/a-sub-domain/25.JPG",
                "http://microsoft.com|http://google.com|https://another.com|https://s3.eu-west-2.amazonaws.com/a-sub-domain"));

            // Using another allowed separator
            Assert.IsTrue(httpTools.IsValidOrigin("https://s3.eu-west-2.amazonaws.com/a-sub-domain/25.JPG",
                "http://microsoft.com,http://google.com,https://another.com,https://s3.eu-west-2.amazonaws.com/a-sub-domain"));

            // Using another allowed separator
            Assert.IsTrue(httpTools.IsValidOrigin("https://s3.eu-west-2.amazonaws.com/a-sub-domain/25.JPG",
                "http://microsoft.com~http://google.com~https://another.com~https://s3.eu-west-2.amazonaws.com/a-sub-domain"));

        }


        [TestMethod]
        public void AllowedFileTypesTest()
        {
            HttpTools httpTools = new HttpTools(new HttpClient());

            Assert.IsTrue(httpTools.IsValidFileType("http://yahoo.com/page/anotherpage/image.jpeg", "jpeg|png|gif"));
            Assert.IsFalse(httpTools.IsValidFileType("http://yahoo.com/page/anotherpage/image.jpeg", "png|gif"));

        }
    }
}