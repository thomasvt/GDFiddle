namespace GDFiddle.Ecs
{
    public class PerformanceCounter
    {
        private readonly double[] _measurements;
        private int _cursor;

        public PerformanceCounter(string name, int measurementCount = 20)
        {
            Name = name;
            MeasurementCount = measurementCount;
            _measurements = new double[measurementCount];
            Clear();
        }

        public void AddMeasurement(double value)
        {
            _measurements[_cursor] = value;
            _cursor = (_cursor + 1) % MeasurementCount;
        }

        public double Value => _measurements.Average();

        public override string ToString()
        {
            var valueMs = Value * 1000f;
            var percentageOfFrame = valueMs * 100 / 16.6666;
            return $"{Name} = {valueMs:0.000} ms  ({percentageOfFrame:0.0}%)";
        }

        public void Clear()
        {
            for (var i = 0; i < MeasurementCount; i++)
                _measurements[i] = 0;
        }

        public string Name { get; }
        public int MeasurementCount { get; }

    }
}
