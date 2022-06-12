//namespace Gyldendal.Jini.Services.Tests
//{
//    /// <summary>
//    /// Summary description for JustMockTest
//    /// </summary>
//    [TestClass]
//    public class JustMockTest
//    {
//        [TestMethod]
//        public void TestMethod()
//        {
//            var rapService = Mock.Create<RapService>();
//            Mock.Arrange(() => rapService.GetDigitalProducts<DigitalProduct>()).Returns(new List<DigitalProduct>()
//            {
//                new DigitalProduct() {Isbn = "9877878454847", Title = "Foo"},
//                new DigitalProduct() {Isbn = "9877878454848", Title = "Bar"},
//            });

//            var jiniService = Mock.Create<JiniManager>();
//            Mock.Arrange(() => jiniService.GetAllConfigurations()).Returns(new List<Data.SalesConfiguration>()
//            {
//                new Data.SalesConfiguration() {Isbn = "9877878454847",State = Enums.EnumState.Approved.ToInt()}
//            });

//            var pf = new ProductFacade(rapService, jiniService);
//            var result = pf.GetProducts();

//            Assert.IsTrue(result[0].HasConfiguration);
//            Assert.IsFalse(result[1].HasConfiguration);
//        }
//        [TestMethod]
//        public void TestGetDigitalProducts()
//        {
//            var rapService = Mock.Create<RapService>();
//            Mock.Arrange(() => rapService.GetDigitalProducts<DigitalProduct>()).CallOriginal();

//            var result = rapService.GetDigitalProducts<DigitalProduct>();
//            Assert.IsTrue(result.Count > 0);
//        }
//    }
//}
