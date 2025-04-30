namespace _1BRC.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var station = new Station();
            station.AddValue(10.0m);
            station.AddValue(20.0m);
            station.AddValue(30.0m);

            Assert.Equal(10.0m,station.Min);
            Assert.Equal(30.0m,station.Max);
            Assert.Equal(20.0m, station.Mean);
        }
    }
}
