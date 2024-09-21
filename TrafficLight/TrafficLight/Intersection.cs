using System;
using System.Collections.Generic;
using System.Threading;

namespace TrafficLight
{
    public class Intersection
    {
        private List<TrafficLight> trafficLights;

        public Intersection()
        {
            trafficLights = new List<TrafficLight>
            {
                new TrafficLight("VehicleLight1", true),
                new TrafficLight("VehicleLight2", true),
                new TrafficLight("PedestrianLight1", false),
                new TrafficLight("PedestrianLight2", false)
            };

            // Подписка каждого светофора на события других светофоров
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

            foreach (var light in trafficLights)
            {
                light.Start(); // Запуск каждого светофора
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

            // Добавление очереди
            trafficLights[0].AddToQueue();
            trafficLights[0].AddToQueue();

            // Удаление из очереди
            trafficLights[1].RemoveFromQueue();
            trafficLights[2].RemoveFromQueue();
            trafficLights[3].RemoveFromQueue();

            // Генерируем события и считаем камерой кол-во объектов в очереди
            foreach (var light in trafficLights)
            {
                light.GenerateTrafficEvent();
                light.CameraCount();
            }
        }
    }
}
