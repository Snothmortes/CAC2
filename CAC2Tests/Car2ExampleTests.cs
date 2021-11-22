namespace CAC1Tests
{
    using Car2;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class Car2ExampleTests
    {
        [Test]
        public void TestStartSpeed() {
            var car = new Car();

            car.EngineStart();

            Assert.AreEqual(0, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestFreeWheelSpeed() {
            var car = new Car();

            car.EngineStart();

            Enumerable.Range(0, 10).ToList().ForEach(_ => car.Accelerate(100));

            Assert.AreEqual(100, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");

            car.FreeWheel();
            car.FreeWheel();
            car.FreeWheel();

            Assert.AreEqual(97, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestAccelerateBy10() {
            var car = new Car();

            car.EngineStart();

            Enumerable.Range(0, 10).ToList().ForEach(_ => car.Accelerate(100));

            car.Accelerate(160);
            Assert.AreEqual(110, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
            car.Accelerate(160);
            Assert.AreEqual(120, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
            car.Accelerate(160);
            Assert.AreEqual(130, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
            car.Accelerate(160);
            Assert.AreEqual(140, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
            car.Accelerate(145);
            Assert.AreEqual(145, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestBraking() {
            var car = new Car();

            car.EngineStart();

            Enumerable.Range(0, 10).ToList().ForEach(_ => car.Accelerate(100));

            car.BrakeBy(20);

            Assert.AreEqual(90, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");

            car.BrakeBy(10);

            Assert.AreEqual(80, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestConsumptionSpeedUpTo30() {
            var car = new Car(1, 20);

            car.EngineStart();

            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);
            car.Accelerate(30);

            Assert.AreEqual(0.98, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestMaximumSpeed() {
            var car = new Car();

            car.EngineStart();

            Enumerable.Range(0, 30).ToList().ForEach(_ => car.Accelerate(260));

            Assert.AreEqual(250, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestMinimumSpeed() {
            var car = new Car();

            car.EngineStart();

            car.Accelerate(5);

            car.BrakeBy(10);

            Assert.AreEqual(0, car.drivingInformationDisplay.ActualSpeed);
        }

        [Test]
        public void TestCustomMaxAcceleration() {
            var car = new Car(1, 20);

            car.EngineStart();

            Assert.AreEqual(20, (car.drivingInformationDisplay as DrivingInformationDisplay)?.MaxAcceleration);
        }

        [Test]
        public void TestNoAccelerationWhenEngineNotRunning() {
            var car = new Car();

            car.Accelerate(5);

            Assert.AreEqual(0, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestAccelerateLowerThanActualSpeed() {
            var car = new Car();

            car.EngineStart();

            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(100);
            car.Accelerate(30);

            Assert.AreEqual(99, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestConsumptionAsRunIdleWhenFreeWheelingAt0() {
            var car = new Car(1);

            car.EngineStart();

            Enumerable.Range(0, 200).ToList().ForEach(_ => car.FreeWheel());

            Assert.AreEqual(0.93999999999999995d, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }

        [Test]
        public void TestAccelerateOnlyUntil250() {
            var car = new Car();

            car.EngineStart();

            Enumerable.Range(0, 270).ToList().ForEach(_ => car.Accelerate(270));

            Assert.AreEqual(250, car.drivingInformationDisplay.ActualSpeed, "Wrong actual speed!");
        }

        [Test]
        public void TestConsumptionLeadsToStopEngine() {
            var car = new Car(1, 20);

            car.EngineStart();

            Enumerable.Range(0, 338).ToList().ForEach(_ => car.Accelerate(250));

            Assert.IsFalse(car.EngineIsRunning, "Engine could not be running!");
        }

        [TestCase(13, 113, 17, 5)]
        public void Car2RandomTests(int maxAcceleration, int finalSpeed, int freeWheel, int brakeBy) {
            var car = new Car(20, maxAcceleration);

            car.EngineStart();

            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.Accelerate(250);
            car.BrakeBy(brakeBy);
            Enumerable.Range(0,freeWheel).ToList().ForEach(_ => car.FreeWheel());
            car.Accelerate(finalSpeed);

            Assert.AreEqual(19.98d, car.fuelTankDisplay.FillLevel, "Wrong fuel tank fill level!");
        }
    }
}
