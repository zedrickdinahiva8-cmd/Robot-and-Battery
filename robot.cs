using System;
using System.Collections.Generic;
 
namespace RobotBatteryApp
{
        public interface IBattery
    {
        int RemainingEnergy { get; }
        void Consume(int amount);
        void Recharge(int amount);
    }
 
    public class LithiumBattery : IBattery
    {
        private const int MaxEnergy = 100;
 
        private int _remainingEnergy;
 
        public LithiumBattery(int startingEnergy = MaxEnergy)
        {
            _remainingEnergy = Math.Clamp(startingEnergy, 0, MaxEnergy);
        }
 
        public int RemainingEnergy => _remainingEnergy;
 
        public void Consume(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Max(0, _remainingEnergy - amount); // never below 0
        }
 
        public void Recharge(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Min(MaxEnergy, _remainingEnergy + amount); // never above 100
        }
    }
 
    public class SolarBattery : IBattery
    {
        private const int MaxEnergy = 100;
        private int _remainingEnergy;
 
        public SolarBattery(int startingEnergy = MaxEnergy)
        {
            _remainingEnergy = Math.Clamp(startingEnergy, 0, MaxEnergy);
        }
 
        public int RemainingEnergy => _remainingEnergy;
 
        public void Consume(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Max(0, _remainingEnergy - amount);
        }
 
        public void Recharge(int amount)
        {
            if (amount < 0) return;
            _remainingEnergy = Math.Min(MaxEnergy, _remainingEnergy + amount);
        }
    }
 
    public abstract class Robot
    {
        private readonly IBattery _battery;
 
        public string Name { get; }
 
        public int RemainingEnergy => _battery.RemainingEnergy;
 
        protected Robot(string name, IBattery battery)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _battery = battery ?? throw new ArgumentNullException(nameof(battery));
        }
 
        public void Recharge(int amount)
        {
            _battery.Recharge(amount);
        }
 
        protected void UseEnergy(int amount)
        {
            _battery.Consume(amount);
        }
 
        public abstract void PerformTask();
    }
 
    public class CleaningRobot : Robot
    {
        public CleaningRobot(string name, IBattery battery) : base(name, battery) { }
 
        public override void PerformTask()
        {
            Console.WriteLine($"{Name} is cleaning the floor...");
            UseEnergy(10);
            Console.WriteLine($"  Energy left: {RemainingEnergy}");
        }
    }
 
    public class DeliveryRobot : Robot
    {
        public DeliveryRobot(string name, IBattery battery) : base(name, battery) { }
 
        public override void PerformTask()
        {
            Console.WriteLine($"{Name} is delivering a package...");
            UseEnergy(25);
            Console.WriteLine($"  Energy left: {RemainingEnergy}");
        }
    }
 
    public class Program
    {
        public static void Main(string[] args)
        {
            var cleaner = new CleaningRobot("CleanBot", new LithiumBattery());
            var courier = new DeliveryRobot("DeliverBot", new SolarBattery());
 
            Console.WriteLine("=== Individual calls ===");
            cleaner.PerformTask();
            courier.PerformTask();
 
            Console.WriteLine("\n=== Recharge ===");
            cleaner.Recharge(50);  
            Console.WriteLine($"{cleaner.Name} after recharge: {cleaner.RemainingEnergy}");
            courier.Recharge(10);
            Console.WriteLine($"{courier.Name} after recharge: {courier.RemainingEnergy}");
 
            Console.WriteLine("\n=== Polymorphism via List<Robot> ===");
            List<Robot> robots = new List<Robot> { cleaner, courier };
 
            foreach (Robot robot in robots)
            {
                robot.PerformTask(); 
            }
 
            Console.WriteLine("\n=== Drain test (energy never below 0) ===");
            for (int i = 0; i < 5; i++)
            {
                courier.PerformTask();
            }
        }
    }
}
