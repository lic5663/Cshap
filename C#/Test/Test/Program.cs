using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemberLib;
using static System.Console;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Member member = new Member("홍길동", "대한민국");
            WriteLine("이름{0} 주소{1}", member.Name, member.Addr);

        }
    }
}

