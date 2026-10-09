using DumplingKitchen.Core;
using NUnit.Framework;

namespace DumplingKitchen.Tests.EditMode
{
    public sealed class RoundTimerTests
    {
        [Test]
        public void Tick_ReducesRemainingTime()
        {
            var timer = new RoundTimer();
            timer.Start(300f);

            timer.Tick(1.5f);

            Assert.That(timer.Remaining, Is.EqualTo(298.5f).Within(0.0001f));
            Assert.That(timer.IsRunning, Is.True);
        }

        [Test]
        public void Expired_FiresExactlyOnce_AndClampsToZero()
        {
            var timer = new RoundTimer();
            int expiredCount = 0;
            timer.Expired += () => expiredCount++;
            timer.Start(5f);

            timer.Tick(6f);
            timer.Tick(1f);

            Assert.That(expiredCount, Is.EqualTo(1));
            Assert.That(timer.Remaining, Is.EqualTo(0f));
            Assert.That(timer.IsRunning, Is.False);
        }

        [Test]
        public void Tick_DoesNothing_WhenStopped()
        {
            var timer = new RoundTimer();
            timer.Start(10f);
            timer.Stop();

            timer.Tick(3f);

            Assert.That(timer.Remaining, Is.EqualTo(10f));
        }

        [Test]
        public void Start_RejectsZeroDuration()
        {
            var timer = new RoundTimer();
            Assert.That(() => timer.Start(0f), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        [TestCase(300f, "5:00")]
        [TestCase(61f, "1:01")]
        [TestCase(59.2f, "1:00")]
        [TestCase(0.4f, "0:01")]
        [TestCase(0f, "0:00")]
        public void Format_ShowsMinutesAndSeconds_RoundingUp(float seconds, string expected)
        {
            Assert.That(RoundTimer.Format(seconds), Is.EqualTo(expected));
        }
    }
}
