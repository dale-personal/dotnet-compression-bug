namespace dotnet7
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MyLib.ExtensionMethods.LoadResource();
        }
    }
}