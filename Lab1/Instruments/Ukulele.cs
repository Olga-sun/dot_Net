using System;
using System.Collections.Generic;
using System.Text;
using Lab1.Core;

namespace Lab1.Instruments
{
    internal class Ukulele : MusicalInstrument
    {
        public Ukulele() : base("Укулеле", "Гавайський чотириструнний щипковий інструмент")
        {
            Description = "Маленька гітара.";
            HistoricalInfo = "З'явилася на Гаваях у 19 столітті.";
        }

        public Ukulele(string name, string characteristics) : base(name, characteristics){ }

        public Ukulele(string name, string characteristics, string description) : base(name, characteristics, description)
        {
        }

        public Ukulele(Ukulele other) : base(other)
        {
        }

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
            Console.WriteLine("Укулеле грає музичну мелодію!");
        }
    }
}
