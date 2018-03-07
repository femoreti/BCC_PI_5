using System;
namespace Normalizer
{
    public class LineDistance
    {
        public string Label { get; set; }
        public double Distance { get; set; }

        public LineDistance(string Label, double Distance)
        {
            this.Label = Label;
            this.Distance = Distance;
        }
    }
}
