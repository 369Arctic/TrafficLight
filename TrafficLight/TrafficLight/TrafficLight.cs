using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficLight
    {
        public string Id { get; private set; }
        public int queueSize; // размер очереди
        private int greenLightDuration; // Длительность зеленого света
        private int yellowLightDuration; // Длительность желтого света
        private string state; // текущее состояние светофора
        private bool isVehicleLight; // является ли светофор автомобильным
        private int intervalBetweenPasses = 1000; // интервал между изменениями состояния
        private Timer timer; // таймер для защиты от слишком частой смены состояний.
        private Random random = new Random(); 
        private Task queueIncreaseTask; // Задача для периодического увеличения очереди
        private CancellationTokenSource cancellationTokenSource; // Отмена задачи
        private object lockObject = new object(); // объект для синхронизации
        private DateTime lastDurationChange = DateTime.Now; // время последнего изменения длительности

        public event EventHandler<TrafficEventArgs> OnTrafficUpdate; // событие для обновления трафика

        public TrafficLight(string id, bool isVehicleLight)
        {
            Id = id;
            queueSize = 0;
            greenLightDuration = isVehicleLight ? 10 : 5;
            yellowLightDuration = 3;
            state = "Red";
            this.isVehicleLight = isVehicleLight;
            timer = new Timer(ChangeState, null, Timeout.Infinite, Timeout.Infinite);
            cancellationTokenSource = new CancellationTokenSource();
            queueIncreaseTask = Task.Run(() => IncreaseQueuePeriodically(cancellationTokenSource.Token));
        }

        public void Start()
        {
            Console.WriteLine($"{Id}: Светофор запущен");
            StartCycle();
        }

        // Периодическое увеличение очереди
        private async Task IncreaseQueuePeriodically(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(random.Next(5000, 15000));
                lock (lockObject)
                {
                    AddToQueue();
                }
                Console.WriteLine($"{Id}: Очередь увеличилась автоматически, теперь: {queueSize}");
            }
        }

        // Изменяем состояние светофора
        private void ChangeState(object stateInfo)
        {
            lock (lockObject)
            {
                if (state == "Red")
                {
                    state = "Green";
                    AdjustGreenLightDuration(); // Адаптируем длительность зеленого света
                    Console.WriteLine($"{Id}: Зеленый свет на {greenLightDuration} секунд");
                    timer.Change(intervalBetweenPasses, intervalBetweenPasses);
                    Task.Run(() => DecreaseQueueDuringGreenLight());
                }
                else if (state == "Green" && isVehicleLight)
                {
                    state = "Yellow";
                    Console.WriteLine($"{Id}: Желтый свет на {yellowLightDuration} секунд");
                    timer.Change(yellowLightDuration * 1000, Timeout.Infinite);
                }
                else
                {
                    state = "Red";
                    Console.WriteLine($"{Id}: Красный свет");
                    StartCycle();
                }
            }
        }

        // Корректировка длительности зеленого света
        private void AdjustGreenLightDuration()
        {
            // Проверяем, прошло ли достаточно времени с последнего изменения
            if ((DateTime.Now - lastDurationChange).TotalSeconds < 10)
            {
                Console.WriteLine($"{Id}: Недавно изменяли длительность зеленого света, пропускаем изменение.");
                return;
            }

            // Увеличиваем/уменьшаем длительность зеленого света в зависимости от очереди
            if (queueSize > 3)
            {
                greenLightDuration = Math.Min(greenLightDuration + 2, 30);
            }
            else if (queueSize < 3)
            {
                greenLightDuration = Math.Max(greenLightDuration - 2, 5);
            }

            lastDurationChange = DateTime.Now; // Обновляем время последнего изменения
            Console.WriteLine($"{Id}: Продолжительность зеленого света изменена на {greenLightDuration} секунд");
        }

        // Уменьшение очереди во время зеленого света
        private async Task DecreaseQueueDuringGreenLight()
        {
            int duration = greenLightDuration * 1000;
            int elapsed = 0;

            while (elapsed < duration && queueSize > 0)
            {
                lock (lockObject)
                {
                    RemoveFromQueue();
                }
                await Task.Delay(intervalBetweenPasses);
                elapsed += intervalBetweenPasses;
            }

            lock (lockObject)
            {
                state = "Yellow";
                timer.Change(yellowLightDuration * 1000, Timeout.Infinite);
            }
        }

        private void StartCycle()
        {
            timer.Change(5000, Timeout.Infinite);
        }


        // Генерация события трафика
        public void GenerateTrafficEvent()
        {
            OnTrafficUpdate?.Invoke(this, new TrafficEventArgs(Id, queueSize, state, greenLightDuration));
        }

        public void HandleTrafficEvent(object sender, TrafficEventArgs e)
        {
            lock (lockObject)
            {
                Console.WriteLine($"{Id} получил событие от {e.Id}: Очередь {e.QueueSize}, состояние {e.State}, длительность зелёного света {e.GreenLightDuration} секунд");

                // Если на этом светофоре очередь >= 3, то увеличиваем длительность зеленого света
                if (this.queueSize >= 3)
                {
                    greenLightDuration = Math.Min(greenLightDuration + 3, 30); // Ограничение на максимум 30 секунд
                    Console.WriteLine($"{Id}: Очередь значительная, увеличиваем длительность зелёного света до {greenLightDuration} секунд");
                }
                // Если очередь меньше 3, уменьшаем длительность зеленого света
                else if (this.queueSize < 3 && e.QueueSize == 0)
                {
                    greenLightDuration = Math.Max(greenLightDuration - 2, 5); // Ограничение на минимум 5 секунд
                    Console.WriteLine($"{Id}: Очередь небольшая, уменьшаем длительность зелёного света до {greenLightDuration} секунд");
                }
                else
                {
                    Console.WriteLine($"{Id}: Нет необходимости менять длительность зелёного света");
                }
            }
        }

        public void AddToQueue()
        {
            queueSize++;
            Console.WriteLine($"{Id}: Очередь увеличена, всего: {queueSize}");
        }

        public void RemoveFromQueue()
        {
            if (queueSize > 0)
                queueSize--;
            Console.WriteLine($"{Id}: Очередь уменьшена, осталось: {queueSize}");
            GenerateTrafficEvent();
        }

        public void CameraCount()
        {
            Console.WriteLine($"{Id}: Камера подсчитала {queueSize} объектов в очереди.");
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            queueIncreaseTask.Wait();
        }
    }
}
