namespace Car2
{
    using System;
    public interface IFuelTankDisplay
    {
        bool IsComplete { get; }
        bool IsOnReserve { get; }
        double FillLevel { get; }
    }
}