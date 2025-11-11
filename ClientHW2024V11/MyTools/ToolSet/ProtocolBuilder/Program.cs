using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Args number:{0}", args.Length);
            //for (int i = 0; i < args.Length; ++i)
            //{
            //    Console.WriteLine("Args{0} {1}", i, args[i]);
            //}

            if (args.Length < 3)
            {
                Console.WriteLine("参数不正确 格式:[input] -lua|-cs [out]");
                return;
            }

            String exe = System.AppDomain.CurrentDomain.BaseDirectory;
            String infile = exe + args[0];
            string outfile = exe + args[2];
            Console.WriteLine("Input file:{0}\nScript:{1}\nOutput file:{2}", infile, args[1], outfile);

            Builder b = new Builder();
            string script = args[1].ToLower();
            if (b.Load(infile))
            {
                if (script.CompareTo("-lua") == 0)
                {
                    b.BuildDefineScriptLua(outfile);
                }
                else if (script.CompareTo("-cs") == 0)
                {
                    b.BuildDefineScriptCS(outfile);
                }
            }
            else
            {
                Console.ReadKey();
            }

            //Console.ReadKey();      //调试用
        }
    }
}
