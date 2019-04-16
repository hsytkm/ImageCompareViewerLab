namespace ImagePixelReadTournament.Common
{
    interface IPixelReader
    {
        string Name { get; }

        double GetAverageY();
    }
}
