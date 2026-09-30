using System;
using Lab1.Core;

namespace Lab1.Instruments
{
    public class Cello : MusicalInstrument
    {
        public Cello() : base("Віолончель", "Смичковий струнний інструмент басового регістру")
        {
            Description = "Значно більша за скрипку, під час гри інструмент спирають на підлогу шпилем.";
            HistoricalInfo = "З'явилася в першій половині 16 століття, витіснивши віолу да гамба.";
        }

        public Cello(string name, string characteristics) : base(name, characteristics) { }

        public Cello(string name, string characteristics, string description) : base(name, characteristics, description) { }

        public Cello(Cello other) : base(other) { }

        public override void Show()
        {
            Console.WriteLine($"Назва: {Name}");
        }

        public override void Desc()
        {
            Console.WriteLine($"Опис: {Description}");
        }

        public override void History()
        {
            Console.WriteLine($"Історична довідка: {HistoricalInfo}");
        }

        public override void Sound()
        {
            Console.WriteLine("Звук: Низьке гудіння (грає віолончель)");
        }
    }
}