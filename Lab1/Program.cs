using System;
using System.Text;
using Lab1.Core;
using Lab1.Instruments;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            MusicalInstrument[] myInstruments = new MusicalInstrument[]
            {
                new Ukulele(), 
                new Violin(),
                new Trombone(),
                new Cello()
            };

            Console.WriteLine("=== Демонстрація поліморфізму ===\n");
   
            foreach (MusicalInstrument instrument in myInstruments)
            {
                instrument.ShowInfo();
            }

            Console.WriteLine("=== Демонстрація конструктора копіювання ===");
            Violin originalViolin = new Violin("Електроскрипка", "Струнний електронний", "Сучасна версія скрипки");
            Violin copiedViolin = new Violin(originalViolin);
            copiedViolin.Name = "Електроскрипка (Копія)";

            copiedViolin.ShowInfo();

            Console.ReadLine(); 
        }
    }
}