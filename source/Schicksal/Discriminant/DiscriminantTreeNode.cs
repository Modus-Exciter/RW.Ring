using System.Collections.Generic;

namespace Schicksal.Discriminant
{
    public enum SplitType
    {
        Numeric,
        Categorical
    }

    public class DiscriminantTreeNode
    {
        public string FeatureName { get; set; } // имя фактора
        public double Znach { get; set; }       // пороговое значение
        public string ClassName { get; set; }   // класс в листе
        public DiscriminantTreeNode Left { get; set; }  // <= Znach
        public DiscriminantTreeNode Right { get; set; } // > Znach
        public Dictionary<string, DiscriminantTreeNode> Categories { get; set; } // для категориальных признаков
        public SplitType SplitType { get; set; } // тип разделения

        public bool End => this.ClassName != null;

        public override string ToString()
        {
            return this.End
                ? $" Класс: {this.ClassName} "
                : $"{this.FeatureName} ≤ {this.Znach:F2}";
        }
    }
}