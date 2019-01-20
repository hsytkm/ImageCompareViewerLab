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
                    double sumaver = 0, sumaveg = 0, sumaveb = 0;
                    double sumrmsr = 0, sumrmsg = 0, sumrmsb = 0, sumrmsy = 0;
                    foreach (var gamut in Gamuts)
                    {
                        sumaver += gamut.Rgb.R;
                        sumaveg += gamut.Rgb.G;
                        sumaveb += gamut.Rgb.B;
                        sumrmsr += gamut.Rms.R;
                        sumrmsg += gamut.Rms.G;
                        sumrmsb += gamut.Rms.B;
                        sumrmsy += gamut.Rms.Y;
                    }
                    var count = Gamuts.Count;
                    _AllAreaGamut = new Gamut(
                        ((sumaver / count), (sumaveg / count), (sumaveb / count)),
                        ((sumrmsr / count), (sumrmsg / count), (sumrmsb / count), (sumrmsy / count)));
                }
                return _AllAreaGamut;
            }
        }

        #endregion

    }
}
