using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public class WilliamsKlootResult
  {
    public CorrelationFormula ModelA { get; internal set; }
    public CorrelationFormula ModelB { get; internal set; }

    public double TStatistic { get; internal set; }
    public double PValue { get; internal set; }

    public bool IsSignificant { get; internal set; }
    public CorrelationFormula BetterModel
    {
      get { return this.TStatistic < 0 ? this.ModelA : this.ModelB; }
    }
  }
}
