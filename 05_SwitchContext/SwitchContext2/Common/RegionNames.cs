using System.Linq;

namespace SwitchContext.Common
{
    public static class RegionNames
    {

        public static string ImageContentRegion1 { get; } = nameof(ImageContentRegion1);

        public static string[] ImageContentRegion2 { get; } =
            Enumerable.Range(0, 2).Select(x => $"ImageContentRegion2_{x + 1}").ToArray();

        public static string[] ImageContentRegion3 { get; } =
            Enumerable.Range(0, 3).Select(x => $"ImageContentRegion3_{x + 1}").ToArray();

        public static string ImageContentRegion2_1 { get; } = nameof(ImageContentRegion2_1);
        public static string ImageContentRegion2_2 { get; } = nameof(ImageContentRegion2_2);
        public static string ImageContentRegion3_1 { get; } = nameof(ImageContentRegion3_1);
        public static string ImageContentRegion3_2 { get; } = nameof(ImageContentRegion3_2);
        public static string ImageContentRegion3_3 { get; } = nameof(ImageContentRegion3_3);
    }
}
