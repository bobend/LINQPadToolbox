using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LINQPad;

namespace Bobend.LINQPadToolbox
{
    /// <summary>
    /// Dumps a progress bar and estimate remaining time, for tracking long running tasks. 
    /// </summary>
    public class TimerUtil
    {
        private readonly Stopwatch _stopwatch;
        private readonly long _total;
        private readonly int _maxMeasurement;
        private readonly int _updateInterval;
        private readonly Util.ProgressBar _progress;
        private readonly DumpContainer _counterDc, _speed, _timeLeft, _timeDone;
        private readonly Queue<long> _measurements = new Queue<long>();
        private readonly object monitor = new object();

        private long _lastIncrementTime = 0;

        public long Counter { get; private set; } = 0;

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        public double Speed { get; private set; } = double.NaN;

        public TimeSpan? TimeLeft { get; private set; } = null;

        public DateTime? EndTime { get; private set; } = null;

        public double Fraction { get; private set; } = 0;

        public TimerUtil(long total, int maxMeasurement = 1000, int updateInterval = 25)
        {
            _stopwatch = new Stopwatch();
            if (total <= 0)
            {
                throw new Exception("zero total");
            }
            _total = total.Dump("Total");
            _maxMeasurement = maxMeasurement;
            _updateInterval = updateInterval;
            _progress = new Util.ProgressBar().Dump("Work");
            _counterDc = new DumpContainer().Dump("Count");
            _speed = new DumpContainer("-- ms/item").Dump("Speed");
            _timeLeft = new DumpContainer("--").Dump("time left estimate");
            _timeDone = new DumpContainer("--").Dump("time done estimate");
        }

        public void CalcAndDisplay()
        {
            Speed = ((double)_measurements.Sum()) / _measurements.Count;
            TimeLeft = TimeSpan.FromMilliseconds((_total - Counter) * Speed);
            EndTime = DateTime.Now + TimeLeft.Value;
            Fraction = (double)Counter / _total;

            _progress.Fraction = Fraction;
            _speed.Content = $"{Speed:0.00} ms/item";
            _counterDc.Content = Counter;
            _timeLeft.Content = TimeLeft;
            _timeDone.Content = EndTime;
        }

        public void Start() => _stopwatch.Start();

        public long Increment()
        {
            lock (monitor)
            {
                var temp = _stopwatch.ElapsedMilliseconds;
                var elapsed = temp - _lastIncrementTime;
                _lastIncrementTime = temp;
                _measurements.Enqueue(elapsed);
                while (_measurements.Count() > _maxMeasurement)
                {
                    _measurements.Dequeue();
                }

                ++Counter;
                if (Counter % _updateInterval == 0)
                {
                    CalcAndDisplay();
                }

                return Counter;
            }
        }
    }
}
