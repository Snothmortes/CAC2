namespace Car2
{
    public interface IEngine
    {
        bool IsRunning { get; }
        void Consume(double liter);
        void Start();
        void Stop();
    }
}