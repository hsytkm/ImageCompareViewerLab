using System.Collections.Generic;

namespace ThosoImage
{
    public class Gamut9d
    {
        // 9分割領域Gamut
        private readonly IReadOnlyList<Gamut> Gamuts;

        public Gamut9d(IReadOnlyList<Gamut> gamuts)
        {
            Gamuts = gamuts;
        }

        // Gamutを名前付きで返す
        public IEnumerable<(string Name, Gamut Gamut)> GetGamuts()
        {
            var names = new[]
            {
                "左上", "上", "右上",
                "左", "中央", "右",
                "左下", "下", "右下",
            };

            for (int i = 0; i < names.Length; i++)
            {
                yield return (names[i], Gamuts[i]);
            }
        }

        #region 全画面平均

        private Gamut _AllAreaGamut = null;
        public Gamut AllAreaGamut
        {
            get
            {
                if (_AllAreaGamut == null)
                {
                    double r = 0, g = 0, b = 0;
                    foreach (var gamut in Gamuts)
                    {
                        r += gamut.Rgb.R;
                        g += gamut.Rgb.G;
                        b += gamut.Rgb.B;
                    }
                    var count = Gamuts.Count;
                    _AllAreaGamut = new Gamut(r: r / count, g: g / count, b: b / count);
                }
                return _AllAreaGamut;
            }
        }

        #endregion

    }
}
