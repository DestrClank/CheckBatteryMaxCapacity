using System;
using OpenHardwareMonitor.Hardware;
using System.Resources;

class Program
{
    private readonly static ResourceManager rm = new ResourceManager("CheckBatteryMaxCapacity.Messages", typeof(Program).Assembly);
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine(rm.GetString("CollectingBatteryInfo"));

            // Initialisation du gestionnaire de matériel
            var computer = new Computer
            {
                IsBatteryEnabled = true // Activer la surveillance de la batterie
            };
            computer.Open(true);

            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Battery)
                {
                    hardware.Update(); // Met à jour les données

                    Console.WriteLine(string.Format(rm.GetString("NameOfBattery"), hardware.Name));
                    float? maxCapacity = null;
                    float? currentCapacity = null;

                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Energy)
                        {
                            if (sensor.Name.Contains("Designed Capacity"))
                                maxCapacity = sensor.Value;
                            else if (sensor.Name.Contains("Fully-Charged Capacity"))
                                currentCapacity = sensor.Value;
                        }
                    }

                    if (maxCapacity.HasValue && currentCapacity.HasValue)
                    {
                        float percentage = (currentCapacity.Value / maxCapacity.Value) * 100;
                        string percentagestring = percentage.ToString("0") + "%";
                        Console.WriteLine(string.Format(rm.GetString("MaximumCapacity"), hardware.Name, percentagestring));
                    }
                    else
                    {
                        Console.WriteLine(rm.GetString("CalculationError"));
                    }
                }
            }

            computer.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(rm.GetString("ErrorOccured"), ex.Message));
        }
    }
}
