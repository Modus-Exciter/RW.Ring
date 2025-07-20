using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public class WilliamsKlootResult
  {
    public CorrelationFormula ModelA { get; set; }
    public CorrelationFormula ModelB { get; set; }

    public double TStatistic { get; set; }
    public double PValue { get; set; }

    public bool IsSignificant { get; set; }
    public CorrelationFormula BetterModel { get; set; }
  }
}
