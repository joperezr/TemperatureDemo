using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.CharacterLcd;
using Iot.Units;
using System;
using System.Device.I2c;
using System.Threading;
using System.Threading.Tasks;


namespace TemperatureDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("You are modified");

            using Lcd2004 lcdDisplay = new Lcd2004(22, 17, new int[] { 25, 24, 23, 18});
            using Bmp280 tempSensor = new Bmp280(I2cDevice.Create(new I2cConnectionSettings(1, 0x77)));
            tempSensor.SetTemperatureSampling(Sampling.UltraHighResolution);
            tempSensor.SetPowerMode(Bmx280PowerMode.Normal);
            using CancellationTokenSource cts = new CancellationTokenSource();
            Task workTask = Task.Run(() => DoWorkAsync(lcdDisplay, tempSensor, cts.Token), cts.Token);
            Console.WriteLine("Press Enter to stop the program...");
            // Run until user adds input
            Console.ReadLine();
            
            cts.Cancel();
            workTask.Wait(5000);
            lcdDisplay.Clear();
        }

        static async Task DoWorkAsync(Lcd2004 lcd, Bmp280 sensor, CancellationToken token)
        {
            string currentTemp = "Current temp:";
            while (!token.IsCancellationRequested)
            {
                lcd.Clear();
                try
                {
                    Temperature temp = await sensor.ReadTemperatureAsync();
                    lcd.Write(currentTemp);
                    lcd.SetCursorPosition(0, 1);
                    lcd.Write($"{temp.Celsius.ToString("0.00")} degrees");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when attempting to read the Bme280 sensor:");
                    Console.WriteLine($"  {e.Message}");
                    Console.WriteLine($"  {e.StackTrace}");
                }
                Thread.Sleep(1000);
            }
        }
    }
}
