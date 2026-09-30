using System;
using Lab1.Core;

namespace Lab1.Instruments
{
    public class Violin : MusicalInstrument
    {
        public Violin() : base("Скрипка", "Струнно-смичковий інструмент")
        {
            Description = "Має чотири струни, грають за допомогою смичка.";
            HistoricalInfo = "Сучасний вигляд набула в Італії у 16 столітті.";
        }

        public Violin(string name, string characteristics) : base(name, characteristics) { }

        public Violin(string name, string characteristics, string description) : base(name, characteristics, description) { }

        public Violin(Violin other) : base(other) { }

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
            Console.WriteLine("Звук: Вжиу-вжиу (грає скрипка)");
        }
    }
}