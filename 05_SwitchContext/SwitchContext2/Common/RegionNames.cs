using System;

namespace SwitchContext.Common
{
    static class RegionNames
    {
        public static string TabContentRegion { get; } = nameof(TabContentRegion);

        public static string GetImageContentRegionName(int count, int index)
        {
            if (count <= index) throw new ArgumentException();
            return $"ImageContentRegion{count}_{index}";
        }


    }
}
