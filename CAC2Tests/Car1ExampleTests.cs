namespace CAC1Tests
{
    using Car2;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class Car1ExampleTests
    {
        [Test]
        public void TestMotorStartAndStop() {
            var car = new Car();

            Assert.IsFalse(car.EngineIsRunning, "engine could not be running.");

            car.EngineStart();

            Assert.IsTrue(car.EngineIsRunning, "engine should be running.");

            car.EngineStop();

            Assert.IsFalse(car.EngineIsRunning, "engine could not be running.");
        }

        [Test]
        public void TestFuelConsumptionOnIdle() {
            var car = new Car(1);

            car.EngineStart();

            Enumerable.Range(0, 3000).ToList().ForEach(_ => car.RunningIdle());

            Assert.AreEqual(0.10, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestFuelTankDisplayIsComplete() {
            var car = new Car(60);

            Assert.IsTrue(car.fuelTankDisplay.IsComplete, "Fuel tank must be complete!");
        }

        [Test]
        public void TestFuelTankDisplayIsOnReserve() {
            var car = new Car(4);

            Assert.IsTrue(car.fuelTankDisplay.IsOnReserve, "Fuel tank must be on reserve!");
        }

        [Test]
        public void TestRefuel() {
            var car = new Car(5);

            car.Refuel(40);

            Assert.AreEqual(45, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestRefuelOverMaximum() {
            var car = new Car(60);

            car.Refuel(15);

            Assert.AreEqual(60, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestNoNegativeFuelLevelAllowed() {
            var car = new Car(-1);

            Assert.AreEqual(0, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestFuelDisplay() {
            var car = new Car(35);

            Assert.AreEqual(35, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestEngineStopsCauseOfNoFuelExactly() {
            var car = new Car(0);

            car.EngineStart();

            Assert.IsFalse(car.EngineIsRunning);
        }

        [Test]
        public void TestNoConsumptionWhenEngineNotRunning() {
            var car = new Car(1);

            Enumerable.Range(0, 1000).ToList().ForEach(_ => car.RunningIdle());

            Assert.AreEqual(1, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestEngineStopsCauseOfNoFuelOver() {
            var car = new Car(1);

            car.EngineStart();
            Enumerable.Range(0, 5000).ToList().ForEach(_ => car.RunningIdle());

            Assert.IsFalse(car.EngineIsRunning);
        }

        [TestCase(10000, 2.0d)]
        [TestCase(3333, 4.0d)]
        [TestCase(7000, 2.9d)]
        public void Car1RandomTests(int range, double result) {
            var car = new Car(5);

            car.EngineStart();
            Enumerable.Range(0, range).ToList().ForEach(_ => car.RunningIdle());

            Assert.AreEqual(result, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestFuelTankDisplayIsNotComplete() {
            var car = new Car();

            Assert.IsFalse(car.fuelTankDisplay.IsComplete);
        }
    }
}