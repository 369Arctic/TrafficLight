using Xunit;
using System.Threading.Tasks;


namespace TrafficLightTests
{
    public class TrafficLightTests
    {
        [Fact]
        public async Task Test_TrafficLight_DecreaseQueueDuringGreenLight()
        {
            // Arrange
            var light = new TrafficLight.TrafficLight("TestLight", true);
            light.AddToQueue();
            light.AddToQueue();
            light.AddToQueue();

            // Act
            light.Start(); // Запускаем цикл светофора

            // Ждем завершения работы зеленого света
            await Task.Delay(12000); // Предположим, что зелёный свет 10 секунд

            // Assert
            Assert.Equal(0, light.queueSize); // Убедимся, что вся очередь прошла
        }
    }
}