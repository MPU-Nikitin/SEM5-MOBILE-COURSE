using System.Windows;
using System.Windows.Threading;

namespace app1
{
    public partial class MainWindow : Window
    {
        private string secretNumber;
        private const int NumberLength = 4; // Длина загаданного числа

        private DispatcherTimer attemptTimer; // Таймер для отсчёта времени попытки
        private DateTime attemptStartTime;    // Время начала текущей попытки

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();

            // Инициализация таймера
            attemptTimer = new DispatcherTimer();
            attemptTimer.Interval = TimeSpan.FromSeconds(1);
            attemptTimer.Tick += AttemptTimer_Tick;
            attemptTimer.Start();
        }

        /// Обработчик события Tick для таймера попытки.
        /// Обновляет TextBlock с отображением прошедшего времени.
        private void AttemptTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - attemptStartTime;
            TimerTextBlock.Text = elapsed.ToString(@"mm\:ss");
        }

        /// Сбрасывает время начала попытки для нового отсчёта.
        private void ResetAttemptTimer()
        {
            attemptStartTime = DateTime.Now;
        }

        /// <summary>
        /// Инициализирует новую игру, генерируя случайное число и сбрасывая таймер.
        /// </summary>
        private void StartNewGame()
        {
            secretNumber = GenerateSecretNumber();
            ResultTextBlock.Text = "Игра началась! Введите вариант:";
            GuessTextBox.Clear();
            ResetAttemptTimer();
        }

        /// <summary>
        /// Генерирует случайное число заданной длины без повторяющихся цифр.
        /// Первая цифра не равна 0.
        /// </summary>
        /// <returns>Сгенерированное число в виде строки.</returns>
        private string GenerateSecretNumber()
        {
            Random rnd = new Random();
            int[] digits = Enumerable.Range(0, 10).ToArray();
            // Перемешиваем массив цифр
            digits = digits.OrderBy(x => rnd.Next()).ToArray();

            // Если первая цифра равна 0, меняем её с первой ненулевой цифрой
            if (digits[0] == 0)
            {
                int nonZeroIndex = Array.FindIndex(digits, x => x != 0);
                (digits[0], digits[nonZeroIndex]) = (digits[nonZeroIndex], digits[0]);
            }

            return string.Join("", digits.Take(NumberLength));
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Проверить».
        /// Сравнивает введённое число с загаданным, считает быков и коров.
        /// После обработки попытки сбрасывает таймер для следующей попытки.
        /// </summary>
        private void CheckGuess_Click(object sender, RoutedEventArgs e)
        {
            string guess = GuessTextBox.Text.Trim();

            // Проверка корректности ввода: число нужной длины и только цифры
            if (guess.Length != NumberLength || !guess.All(char.IsDigit))
            {
                MessageBox.Show($"Пожалуйста, введите {NumberLength}-значное число.",
                                "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int bulls = 0, cows = 0;
            for (int i = 0; i < NumberLength; i++)
            {
                if (guess[i] == secretNumber[i])
                {
                    bulls++;
                }
                else if (secretNumber.Contains(guess[i]))
                {
                    cows++;
                }
            }

            TimeSpan attemptTime = DateTime.Now - attemptStartTime;

            ResultTextBlock.Text += $"\nПопытка: {guess} - Быки: {bulls}, Коровы: {cows} (время: {attemptTime.ToString(@"mm\:ss")})";

            // Если число угадано, выводим сообщение о победе и начинаем новую игру
            if (bulls == NumberLength)
            {
                MessageBox.Show($"Поздравляем! Вы угадали число: {secretNumber} за {attemptTime.ToString(@"mm\:ss")}",
                                "Победа", MessageBoxButton.OK, MessageBoxImage.Information);
                StartNewGame();
            }
            else
            {
                GuessTextBox.Clear();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Начать заново».
        /// Начинает новую игру.
        /// </summary>
        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }
}