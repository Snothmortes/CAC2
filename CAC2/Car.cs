namespace Car2
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class Car : ICar
    {
        // ReSharper disable once InconsistentNaming
        private readonly IEngine engine;
        // ReSharper disable once InconsistentNaming
        public IFuelTankDisplay fuelTankDisplay;
        // ReSharper disable once InconsistentNaming
        public IDrivingInformationDisplay drivingInformationDisplay;
        // ReSharper disable once InconsistentNaming
        private readonly IDrivingProcessor drivingProcessor;

        private readonly int _maxAcceleration = 10;

        public Car() {
            engine = new Engine(false);
            fuelTankDisplay = new FuelTankDisplay(engine);
            drivingProcessor = new DrivingProcessor(_maxAcceleration);
            drivingInformationDisplay = new DrivingInformationDisplay(drivingProcessor);
        }

        public Car(double fuelLevel) : this() => (engine as Engine)!.FuelTank.FillLevel = fuelLevel;
        public Car(double fuelLevel, int maxAcceleration) : this(fuelLevel) =>
            _maxAcceleration = maxAcceleration < 5
                ? 5
                : maxAcceleration > 20
                    ? 20
                    : maxAcceleration;

        public bool EngineIsRunning => engine.IsRunning;
        public void BrakeBy(int speed) => drivingProcessor.ReduceSpeed(speed);

        public void Accelerate(int speed) {
            Console.WriteLine($"Accelerate({speed}) => fuelTankDisplay: {(engine as Engine)!.FuelTank.FillLevel} => ActualSpeed: {drivingProcessor.ActualSpeed}");

            speed = speed > 250 ? 250 : speed;
            if ((engine as Engine)!.FuelTank.FillLevel == 0)
                EngineStop();
            if (!EngineIsRunning)
                return;
            if (speed < drivingInformationDisplay.ActualSpeed) {
                FreeWheel();
                Console.WriteLine($"Accelerate({speed}) => fuelTankDisplay: {(engine as Engine)!.FuelTank.FillLevel} => ActualSpeed: {drivingProcessor.ActualSpeed}");
                return;
            }

            drivingProcessor.IncreaseSpeedTo(speed);

            var consumeSwitch = new Dictionary<Func<int, bool>, Action>() {
            { x => x <= 60, () => (engine as Engine)!.Consume(0.002)},
            { x => x <= 100, () => (engine as Engine)!.Consume(0.0014)},
            { x => x <= 140, () => (engine as Engine)!.Consume(0.002)},
            { x => x <= 200, () => (engine as Engine)!.Consume(0.0025)},
            { x => x <= 250, () => (engine as Engine)!.Consume(0.0030)},
        };
            consumeSwitch.First(sw => sw.Key(drivingProcessor.ActualSpeed)).Value();
        }

        public void FreeWheel() {
            RunningIdle();
            BrakeBy(1);
        }

        public void EngineStart() {
            Console.WriteLine($"fuelLevel: {fuelTankDisplay.FillLevel}");
            Console.WriteLine($"maxAcceleration: {_maxAcceleration}");
            Console.WriteLine($"ActualSpeed: {drivingInformationDisplay.ActualSpeed}");
            engine.Start();
            (drivingProcessor as DrivingProcessor)!.MaxAcceleration = _maxAcceleration;
        }

        public void EngineStop() => engine.Stop();

        public void RunningIdle() {
            if (engine.IsRunning)
                (engine as Engine)!.Consume(0.0003);
            if (fuelTankDisplay.FillLevel == 0)
                EngineStop();
        }

        public void Refuel(double liters) => (engine as Engine)!.FuelTank.Refuel(liters);
    }

    public class Engine : IEngine
    {
        public FuelTank FuelTank { get; }

        public Engine(bool isRunning) {
            FuelTank = new FuelTank();
            IsRunning = isRunning;
        }

        public bool IsRunning { get; set; }

        public void Start() {
            if (FuelTank.FillLevel == 0)
                return;
            IsRunning = true;
        }

        public void Stop() => IsRunning = false;
        public void Consume(double liter) => FuelTank.Consume(liter);
    }

    public class FuelTank : IFuelTank, INotifyPropertyChanged
    {
        private double _fillLevel = 20;

        public double FillLevel {
            get => _fillLevel;
            set {
                _fillLevel = value < 0 ? 0 : value > 60 ? 60 : Math.Round(value, 10);
                OnPropertyChanged();
            }
        }

        public bool IsOnReserve => FillLevel < 5;
        public bool IsComplete => !(FillLevel < 60);

        public void Consume(double liters) => FillLevel -= liters;
        public void Refuel(double liters) => FillLevel = FillLevel + liters > 60 ? 60 : FillLevel + liters;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FuelTankDisplay : IFuelTankDisplay
    {
        private readonly Engine _engine;

        public FuelTankDisplay(IEngine engine) {
            _engine = engine as Engine;
            _engine!.FuelTank.PropertyChanged += (s, e)
                => FillLevel = Math.Round(_engine.FuelTank.FillLevel, 2);
        }

        public bool IsComplete => _engine.FuelTank.IsComplete;
        public bool IsOnReserve => _engine.FuelTank.IsOnReserve;
        public double FillLevel { get; private set; }
    }

    public class DrivingInformationDisplay : IDrivingInformationDisplay
    {
        public DrivingInformationDisplay(IDrivingProcessor drivingProcessor) {
            (drivingProcessor as DrivingProcessor)!.PropertyChanged += (s, e)
                => ActualSpeed = drivingProcessor.ActualSpeed;
            (drivingProcessor as DrivingProcessor)!.PropertyChanged += (s, e)
                => MaxAcceleration = ((DrivingProcessor)drivingProcessor).MaxAcceleration;
        }

        public int ActualSpeed { get; private set; }
        public int MaxAcceleration { get; private set; }
    }

    public class DrivingProcessor : IDrivingProcessor, INotifyPropertyChanged
    {
        private DrivingProcessor() {
        }

        public DrivingProcessor(int maxAcceleration) => MaxAcceleration = maxAcceleration;

        private int _maxAcceleration;
        public int MaxAcceleration {
            get => _maxAcceleration;
            set {
                _maxAcceleration = value;
                OnPropertyChanged();
            }
        }

        private int _actualSpeed = 0;
        public int ActualSpeed {
            get => _actualSpeed;
            set {
                _actualSpeed = value;
                OnPropertyChanged();
            }
        }

        public void IncreaseSpeedTo(int speed) {
            ActualSpeed = speed - ActualSpeed < MaxAcceleration
                    ? speed
                    : ActualSpeed + MaxAcceleration;
        }

        public void ReduceSpeed(int speed) =>
            ActualSpeed = ActualSpeed < 10
                ? ActualSpeed - speed < 0
                    ? 0
                    : ActualSpeed - speed
                : ActualSpeed - Math.Min(speed, 10);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}