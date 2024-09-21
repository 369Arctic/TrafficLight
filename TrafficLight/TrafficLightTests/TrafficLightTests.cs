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
            light.Start(); // ��������� ���� ���������

            // ���� ���������� ������ �������� �����
            await Task.Delay(12000); // �����������, ��� ������ ���� 10 ������

            // Assert
            Assert.Equal(0, light.queueSize); // ��������, ��� ��� ������� ������
        }
    }
}