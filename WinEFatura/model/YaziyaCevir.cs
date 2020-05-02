using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//test
//test 2, tarayicidan eklendi.

namespace com.yp.efatura.model
{
public static class YaziyaCevir
    {
        #region Variable

        private static readonly string[] Birler = { "", "Bir", "İki", "Üç", "Dört", "Beş", "Altı", "Yedi", "Sekiz", "Dokuz" };
        private static readonly string[] Onlar = { "", "On", "Yirmi", "Otuz", "Kırk", "Elli", "Altmış", "Yetmiş", "Seksen", "Doksan" };
        private static readonly string[] Binler = { "", "Bin", "Milyon", "Milyar", "Trilyon" };

        #endregion

        private static string Binlik(int value)
        {
            return (value >= 100 ? (value >= 200 ? Birler[value / 100] : "") + "Yüz" : "") + (value % 100 >= 10 ? Onlar[value % 100 / 10] : "") + Birler[value % 10];
        }
        private static string YaziIle(long value)
        {
            var yazi = "";
            var subStr = "" + value;
            var length = subStr.Length;
            var basamak = (int)Math.Ceiling(((decimal)length) / 3);
            var maxBasamak = length % 3 > 0 ? length % 3 : 3;
            for (var i = basamak - 1; i >= 0; i--)
            {
                if (i != basamak - 1)
                {
                    var d = int.Parse(subStr.Substring((basamak - i - 2) * 3 + maxBasamak, 3));
                    yazi += Binlik(d) + (d > 0 ? Binler[i] : "");
                }
                else
                {
                    var degerStr = subStr.Substring(0, maxBasamak);
                    var degerInt = int.Parse(degerStr);
                    yazi += (i != 1 || degerInt != 1 ? Binlik(degerInt) : "") + Binler[i];
                }
            }
            if (value == 0)
                yazi = "Sıfır ";
            return yazi;
        }
        public static string YaziIleTutar(this decimal value)
        {
            var kurus = value - Math.Floor(value);
            return YaziIle((long)Math.Floor(value)) + " TL, " + YaziIle((long)Math.Floor(kurus * 100)) + " Krş.";
        }
        public static string YaziIleVade(this DateTime tarih)
        {
            var gunYaziIle = YaziIle(tarih.Day);
            var ayYaziIle = tarih.ToString("MMMM");
            var yilYaziIle = YaziIle(tarih.Year);
            return gunYaziIle + " " + ayYaziIle + " " + yilYaziIle;
        }
    }}
