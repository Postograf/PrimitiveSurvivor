using Unity.Core;

public static class TimeDataExtensions
{
    public static bool ComeFor(this in TimeData mainTime, double period, double offset = 0)
    {
        if (period <= mainTime.DeltaTime)
            return true;

        if (mainTime.ElapsedTime <= mainTime.DeltaTime)
            return false;

        return (mainTime.ElapsedTime + offset) / period % 1 <= mainTime.DeltaTime / period;
    }
}