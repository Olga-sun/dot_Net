using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1.Core
{
     public abstract class MusicalInstrument
    {
        public string? Name { get; set; }
        public string? Characteristics { get; set; }
        public string? Description { get; set; }
        public string? HistoricalInfo { get; set; }
        public MusicalInstrument()
        {
            Name = "Невідомий";
            Characteristics = "Невідомі";
        }
        public MusicalInstrument(string name, string characteristics)
        {
            Name= name;
            Characteristics = characteristics;
        }
        public MusicalInstrument(string name, string characteristics, string description)
        {
            Name = name;
            Characteristics = characteristics;
            Description = description;
        }
        public MusicalInstrument(MusicalInstrument other)
        {
            Name = other.Name;
            Characteristics = other.Characteristics;
            Description = other.Description;
            HistoricalInfo = other.HistoricalInfo;
        }
        public abstract void Sound();
        public abstract void Show();
        public abstract void Desc();
        public abstract void History();

        public void ShowInfo()
        {
            Console.WriteLine("----------------------------------------------");
            Show();
            Desc();
            History();
            Sound();
            Console.WriteLine($"Характеристики: {Characteristics}");
            Console.WriteLine("----------------------------------------------");
        }
    }
}
