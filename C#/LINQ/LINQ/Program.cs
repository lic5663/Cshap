using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace LINQ
{
    public class Profile
    {
        public string Name { get; set; }
        public int Height { get; set; }
    }
    public class Class
    {
        public string Name { get; set; }
        public int[] Score { get; set; }
    }
    class Person
    {
        public string Sex { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 9, 2, 6, 4, 5, 3, 7, 8, 1, 10 };
            var result = from n in numbers
                         where n % 2 == 0
                         orderby n
                         select n;
            foreach (int n in result)
                WriteLine($"짝수 {n}");

            Profile[] arrProfile =
            {
                new Profile() {Name="김철수",Height =186 },
                new Profile() {Name="바끄네",Height =158 },
                new Profile() {Name="채순시",Height =172 },
                new Profile() {Name="김기출",Height =178 },
                new Profile() {Name="유병우",Height =171 }
            };

            var profiles = from profile in arrProfile
                           where profile.Height < 175
                           orderby profile.Height
                           select new
                           {
                               Name = profile.Name,
                               InchHeight = profile.Height * 0.393
                           };
            foreach (var profile in profiles)
                WriteLine($"{profile.Name}, {profile.InchHeight}");

            Class[] arrClass =
            {
                new Class() {Name ="장미반", Score = new int[] {99,80,70,24}},
                new Class() {Name ="백합반", Score = new int[] {60,45,87,72}},
                new Class() {Name ="개나리반", Score = new int[] {92,30,25,94}},
                new Class() {Name ="갈대반", Score = new int[] {90,88,0,17}}
            };
            var classes = from c in arrClass
                          from s in c.Score
                          where s < 60
                          orderby s
                          select new { c.Name, Lowest = s };
            foreach (var c in classes)
                WriteLine($"낙제 : {c.Name} ({c.Lowest})");

            Person[] peopleArr =
            {
                new Person() {Sex = "여자", Name = "성나정"},
                new Person() {Sex = "남자", Name = "쓰레기"},
                new Person() {Sex = "여자", Name = "조윤진"},
                new Person() {Sex = "남자", Name = "삼천포"}
            };
            var group = from person in peopleArr
                        group person by person.Sex == "남자" into data
                        select new { SexCheck = data.Key, People = data };
            foreach (var element in group)
            {
                if (element.SexCheck)
                {
                    WriteLine("<남자 리스트>");
                    foreach (var person in element.People)
                        WriteLine($"이름 : {person.Name}");
                }
                else
                {
                    WriteLine("<여자 리스트>");
                    foreach (var person in element.People)
                        WriteLine($"이름 : {person.Name}");
                }
            }

        }
    }
}
