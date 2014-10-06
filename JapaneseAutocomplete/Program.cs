using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Suggestion;

namespace JapaneseAutocomplete
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            StreamReader sr = System.IO.File.OpenText("test.in");

            if (args.Length > 0 && args[0] == "b")
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                TestFile(sr, Console.Out, 10);
                watch.Stop();

                Console.Out.WriteLine();
                Console.Out.WriteLine(watch.Elapsed + " " + watch.ElapsedMilliseconds + " " + watch.ElapsedTicks);
            }
            else
            {
                TestFile(sr, Console.Out, 10);
            }

            //TextWriter file = File.CreateText("test.out");
            //TestFile(sr, file, int.MaxValue);
        }

        static void TestFile(StreamReader tr, TextWriter sw, int limit)
        {
            Vocabulary vocab = new Vocabulary(LoadFromFile(tr));

            int count = int.Parse(tr.ReadLine());

            while (count-- > 0 && !tr.EndOfStream)
            {
                string prefix = tr.ReadLine();

                string [] completions = vocab.GetComplitions(prefix, limit);
                foreach(string s in completions)
                {
                    sw.WriteLine(s);
                }

                if (completions.Count() > 0)
                    sw.WriteLine("");
            }
        }

        static Dictionary<string, int> LoadFromFile(StreamReader fs)
        {
            Dictionary<string, int> words = new Dictionary<string, int>();

            //  читаем N
            string c = fs.ReadLine();
            int count = int.Parse(c);
            //  читаем слова
            while (count > 0 && !fs.EndOfStream)
            {
                LoadWordFromLine(fs.ReadLine(), words);
                --count;
            }

            return words;
        }

        static void LoadWordFromLine(string line, Dictionary<string, int> words)
        {
            string[] tokens = line.Split(' ');

            //  так как во входном файле гарантированно корректная информация,
            //  то можно ничего не проверять. А вообще желательно проверить
            words.Add(tokens[0], int.Parse(tokens[1]));
        }
    }

}
