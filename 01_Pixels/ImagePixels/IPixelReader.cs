namespace ImagePixels
{
    interface IPixelReader
    {
        string Name { get; }

        double GetAverageY();
    }
}
