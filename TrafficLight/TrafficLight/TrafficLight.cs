using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficLight
    {
        public string Id { get; private set; }
        private int queueSize; 
        private int greenLightDuration;
        private string state; 
        private Timer timer;

        // Событие для передачи информации между светофорами
        public event EventHandler<TrafficEventArgs> OnTrafficUpdate;

        public TrafficLight(string id)
        {
            Id = id;
            queueSize = 0;
            greenLightDuration = 10; // Изначально 10 секунд зелёного света
            state = "Red"; // Начинаем с красного света
            timer = new Timer(ChangeState, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            Console.WriteLine($"{Id}: Светофор запущен");
            StartCycle();
        }

        // Изменение состояния светофора
        private void ChangeState(object stateInfo)
        {
            if (state == "Red")
            {
                state = "Green";
                Console.WriteLine($"{Id}: Зеленый свет на {greenLightDuration} секунд");
                timer.Change(greenLightDuration * 1000, Timeout.Infinite);
            }
            else if (state == "Green")
            {
                state = "Yellow";
                Console.WriteLine($"{Id}: Желтый свет на 3 секунды");
                timer.Change(3000, Timeout.Infinite);
            }
            else
            {
                state = "Red";
                Console.WriteLine($"{Id}: Красный свет");
                StartCycle();
            }
        }

        // Начало нового цикла светофора
        private void StartCycle()
        {
            timer.Change(5000, Timeout.Infinite); 
        }

        // Генерация события с текущим состоянием светофора
        public void GenerateTrafficEvent()
        {
            OnTrafficUpdate?.Invoke(this, new TrafficEventArgs(Id, queueSize, state, greenLightDuration));
        }

        // Обработка событий от других светофоров
        public void HandleTrafficEvent(object sender, TrafficEventArgs e)
        {
            Console.WriteLine($"{Id} получил событие от {e.Id}: Очередь {e.QueueSize}, состояние {e.State}, длительность зелёного света {e.GreenLightDuration} секунд");

            if (e.QueueSize > this.queueSize)
            {
                // Уменьшаем время зелёного света, если у другого светофора очередь больше
                greenLightDuration = Math.Max(greenLightDuration - 2, 5);
            }
            else
            {
                // Увеличиваем время зелёного света, если очередь меньше
                greenLightDuration = Math.Min(greenLightDuration + 2, 30);
            }

            Console.WriteLine($"{Id}: Новая длительность зелёного света: {greenLightDuration} секунд");
        }

        // Добавление автомобиля/пешехода в очередь
        public void AddToQueue()
        {
            queueSize++;
            Console.WriteLine($"{Id}: Очередь увеличена, всего: {queueSize}");
        }

        // Уменьшение очереди
        public void RemoveFromQueue()
        {
            if (queueSize > 0) queueSize--;
            Console.WriteLine($"{Id}: Очередь уменьшена, осталось: {queueSize}");
        }
    }
}
