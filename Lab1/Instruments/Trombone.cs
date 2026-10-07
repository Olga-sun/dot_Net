using System;
using Lab1.Core;

namespace Lab1.Instruments
{
    public class Trombone : MusicalInstrument
    {
        public Trombone() : base("Тромбон", "Мідний духовий інструмент")
        {
            Description = "Має висувну U-подібну трубку (кулісу) для зміни висоти звуку.";
            HistoricalInfo = "Виник у 15 столітті, походить від старовинних кулісних труб.";
        }

        public Trombone(string name, string characteristics) : base(name, characteristics) { }

        public Trombone(string name, string characteristics, string description) : base(name, characteristics, description) { }

        public Trombone(Trombone other) : base(other) { }

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
            Console.WriteLine("Звук: Ту-ту-ту (грає тромбон)");
        }
    }
}