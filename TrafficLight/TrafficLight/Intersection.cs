using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class Intersection
    {
        private List<TrafficLight> trafficLights;

        public Intersection()
        {
            trafficLights = new List<TrafficLight>
            {
                new TrafficLight("VehicleLight1"),
                new TrafficLight("VehicleLight2"),
                new TrafficLight("PedestrianLight1"),
                new TrafficLight("PedestrianLight2")
            };

            foreach (var light in trafficLights)
            {
                foreach (var otherLight in trafficLights)
                {
                    if (light != otherLight)
                    {
                        light.OnTrafficUpdate += otherLight.HandleTrafficEvent;
                    }
                }
            }
        }

        public void StartSimulation()
        {
            Console.WriteLine("Запуск симуляции перекрестка...");

            // Запускаем все светофоры
            foreach (var light in trafficLights)
            {
                light.Start();
            }

            while (true)
            {
                SimulateTraffic();
                Thread.Sleep(5000); // Обновляем трафик каждые 5 секунд
            }
        }

        private void SimulateTraffic()
        {
            Console.WriteLine("Обновляем трафик на перекрестке...");

            // На одном светофоре добавляем очередь
            trafficLights[0].AddToQueue();  
            trafficLights[0].AddToQueue();  

            // Остальные светофоры без очереди
            trafficLights[1].RemoveFromQueue();  // Убираем автомобили у VehicleLight2
            trafficLights[2].RemoveFromQueue();  // Убираем пешеходов у PedestrianLight1
            trafficLights[3].RemoveFromQueue();  // Убираем пешеходов у PedestrianLight2

            foreach (var light in trafficLights)
            {
                light.GenerateTrafficEvent();
            }
        }
    }
}
