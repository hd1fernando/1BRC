namespace _1BRC.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var station = new Station
            {
                Values = new List<float>
                {
                    10.0f, 20.0f, 30.0f
                },
                Name = "MyStation"
            };

            var min = station.Min();
            var max = station.Max();
            var mean = station.Mean();

            Assert.Equal(10.0f,min);
            Assert.Equal(30.0f,max);
            Assert.Equal(20.0f, mean);
        }
    }
}
