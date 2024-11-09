using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace Task_Scheduler
{
    /// <summary>
    /// Interaction logic for All_Page.xaml
    /// </summary>
    public partial class All_Page : Page
    {
        public All_Page()
        {
            InitializeComponent();


            bool bandwidthRunning = false;
            bool memoryRunning = false;
            bool cpuRunning = false;

            if (!cpuRunning) {

                var cpuThread = new Thread(CPU_Track);
                cpuThread.Start();
            
            }

            if (!bandwidthRunning)
            {
                var bandwithThread = new Thread(Bandwith_Track);
                bandwithThread.Start();

            }
            if (!memoryRunning) {

                var memoryThread = new Thread(Memory_Track);
                memoryThread.Start();
            
            }
        }








        public void AppendToConsole_CPU(string text)
        {
            ConsoleTextBlock.Text += text + "\n";
            ConsoleScrollViewer.ScrollToEnd();
        }
        public void AppendToConsole_Bandwidth(string text)
        {
            ConsoleTextBlock2.Text += text + "\n";
            ConsoleScrollViewer2.ScrollToEnd();
        }
        public void AppendToConsole_Memory(string text)
        {
            ConsoleTextBlock3.Text += text + "\n";
            ConsoleScrollViewer3.ScrollToEnd();
        }

        private void ConsoleScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender == ConsoleScrollViewer)
                ConsoleScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            else if (sender == ConsoleScrollViewer2)
                ConsoleScrollViewer2.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            else if (sender == ConsoleScrollViewer3)
                ConsoleScrollViewer3.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

        }

        private void ConsoleScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == ConsoleScrollViewer)
                ConsoleScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            else if (sender == ConsoleScrollViewer2)
                ConsoleScrollViewer2.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            else if (sender == ConsoleScrollViewer3)
                ConsoleScrollViewer3.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

        }




        private async void CPU_Track()
        {
            PerformanceCounter cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter cpuIdleCounter = new PerformanceCounter("Processor", "% Idle Time", "_Total");
            PerformanceCounter cpuInterruptsCounter = new PerformanceCounter("Processor", "Interrupts/sec", "_Total");

            while (true)
            {
                float cpuUsage = cpuUsageCounter.NextValue();
                float cpuIdle = cpuIdleCounter.NextValue();
                float cpuInterrupts = cpuInterruptsCounter.NextValue();

                Console.WriteLine("CPU Usage: {0}% ||| CPU Idle: {1}% ||| CPU Interrupts/sec: {2}", cpuUsage, cpuIdle, cpuInterrupts);
                string appendString = $"Usage: {cpuUsage}\nIdle: {cpuIdle}\nInterrupt: {cpuInterrupts}";
                Dispatcher.Invoke(() => AppendToConsole_CPU(appendString));
                await Task.Run(() =>
                {


                    Thread.Sleep(1_000);


                });
            }
        }





        private async void Memory_Track()
        {

            PerformanceCounter availableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter committedBytesCounter = new PerformanceCounter("Memory", "Committed Bytes");

            while (true)
            {
                float availableMemory = availableMemoryCounter.NextValue();
                float committedMemory = committedBytesCounter.NextValue();

                float committedMemoryGB = committedMemory / (1024 * 1024 * 1024);


                Console.WriteLine("Available Memory: {0} MB || Committed Memory: {1} GB", availableMemory, committedMemoryGB);
                string appendString = $"Avail: {availableMemory} Mb/s\nCommit: {committedMemoryGB} GB/s";
                Dispatcher.Invoke(() => AppendToConsole_Memory(appendString));

                await Task.Run(() =>
                {


                    Thread.Sleep(1_000);


                });
            }
        }







        private async void Bandwith_Track()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            var oldBytesRec = 0L;
            var oldBytesSent = 0L;
            var startTime = DateTime.Now;

            while (true)
            {

                var newBytesRec = 0L;
                var newBytesSent = 0L;
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - startTime;

                foreach (var ni in interfaces)
                {

                    if (ni.Name.Contains("Ethernet") && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        var stats = ni.GetIPv4Statistics();
                        newBytesSent = stats.BytesSent;
                        newBytesRec = stats.BytesReceived;

                    }


                }

                double sendSpeed = 0;
                double recSpeed = 0;
                if (elapsedTime.TotalSeconds > 0)
                {

                    sendSpeed = (newBytesSent - oldBytesSent) * 8 / (elapsedTime.TotalSeconds * 1_000.0);
                    recSpeed = (newBytesRec - oldBytesRec) * 8 / (elapsedTime.TotalSeconds * 1_000.0);
                }





                //Console.WriteLine("Upload Speed: {0} Mb/s ||| Download Speed: {1} Mb/s", sendSpeed.ToString("0.00"), recSpeed.ToString("0.00"));
                string appendString = $"Up: {sendSpeed:0.00} KB/s\nDown: {recSpeed:0.00} KB/s";
                Dispatcher.Invoke(() => AppendToConsole_Bandwidth(appendString));
                oldBytesSent = newBytesSent;
                oldBytesRec = newBytesRec;
                startTime = DateTime.Now;
                await Task.Run(() =>
                {
                    
                    
                        Thread.Sleep(1_000); 
                                            
                    
                });
            }

        }
    }
}
