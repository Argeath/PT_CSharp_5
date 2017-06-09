using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PT_CSharp_Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long NewtonNominator(int N, int K) // Licznik
        {
            long result = 1;
            for (int i = N; i > N - K; i--)
                result *= i;

            return result;
        }

        long NewtonDenominator(int N, int K) // Mianownik
        {
            long result = 1;
            for (int i = 2; i <= K; i++)
                result *= i;

            return result;
        }

        void Zad1A(int N, int K)
        {
            //Tuple<int, int> nk = new Tuple<int, int>(N, K);

            Task<long> nominatorTask = Task.Factory.StartNew(() => NewtonNominator(N, K));
            Task<long> denominatorTask = Task.Factory.StartNew(() => NewtonDenominator(N, K));

            long nominator = nominatorTask.Result;
            long denominator = denominatorTask.Result;
            double newton = (double)nominator / denominator;

            TasksTextBox.Text = newton.ToString();
        }

        void Zad1B(int N, int K)
        {
            Func<int, int, long> opNominator = NewtonNominator;
            Func<int, int, long> opDenominator = NewtonDenominator;

            var resultNominator = opNominator.BeginInvoke(N, K, null, null);
            var resultDenominator = opDenominator.BeginInvoke(N, K, null, null);

            while (!resultNominator.IsCompleted && !resultDenominator.IsCompleted)
                Console.Write(".");


            long nominator = opNominator.EndInvoke(resultNominator);
            long denominator = opDenominator.EndInvoke(resultDenominator);
            double newton = (double)nominator / denominator;

            DelegatesTextBox.Text = newton.ToString();
        }

        Task<long> doNominator(int N, int K)
        {
            return Task.Factory.StartNew(() => NewtonNominator(N, K));
        }
        Task<long> doDenominator(int N, int K)
        {
            return Task.Factory.StartNew(() => NewtonDenominator(N, K));
        }

        async void Zad1C(int N, int K)
        {
            long nominator = await doNominator(N, K);
            long denominator = await doDenominator(N, K);
            double newton = (double)nominator / denominator;

            AsyncTextBox.Text = newton.ToString();
        }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TasksBtn_Click(object sender, RoutedEventArgs e)
        {
            int N = int.Parse(NTextBox.Text);
            int K = int.Parse(KTextBox.Text);
            Zad1A(N, K);
        }

        private void DelegatesBtn_Click(object sender, RoutedEventArgs e)
        {
            int N = int.Parse(NTextBox.Text);
            int K = int.Parse(KTextBox.Text);
            Zad1B(N, K);
        }

        private void AsyncBtn_Click(object sender, RoutedEventArgs e)
        {
            int N = int.Parse(NTextBox.Text);
            int K = int.Parse(KTextBox.Text);
            Zad1C(N, K);
        }

        private void Fib_Click(object sender, RoutedEventArgs e)
        {
            int n = int.Parse(FibITextBox.Text);
            FibResult.Text = "";

            BackgroundWorker w = new BackgroundWorker();
            w.DoWork += (s, args) =>
            {
                BackgroundWorker worker = s as BackgroundWorker;

                BigInteger prev = new BigInteger(0);
                BigInteger next = new BigInteger(1);
                BigInteger result = new BigInteger(0);

                for (int i = 0; i < n - 1; i++)
                {
                    result = prev + next;
                    prev = next;
                    next = result;

                    worker.ReportProgress((i*100)/n);
                    Thread.Sleep(20);
                }

                worker.ReportProgress(100);
                args.Result = result;
            };

            w.ProgressChanged += (s, args) =>
            {
                FibProgress.Value = args.ProgressPercentage;
            };

            w.RunWorkerCompleted += (s, args) =>
            {
                FibResult.Text = args.Result.ToString();
            };

            w.WorkerReportsProgress = true;
            w.RunWorkerAsync();
        }
    }
}
