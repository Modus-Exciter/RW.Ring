using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schicksal;

namespace Schicksal.Regression
{
  public class WilliamsKlootResult
  {
    public CorrelationFormula Model { get; internal set; }
    public RegressionDependency DependencyA { get; internal set; }
    public RegressionDependency DependencyB { get; internal set; } 

    public double TStatistic { get; internal set; }
    public double PValue { get; internal set; }

    public bool IsSignificant { get; internal set; }
    public RegressionDependency BetterModel
    {
      get { return this.TStatistic > 0 ? this.DependencyA: this.DependencyB; }
    }
  }
}
