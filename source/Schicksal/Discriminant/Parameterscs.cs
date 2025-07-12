using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Schicksal.Basic;


namespace Schicksal.Discriminant
{
  class Parameterscs
  {
    public DataTable Table { get; }
    public string Filter { get; }
    public FactorInfo Predictors { get; }
    public string ResponseColumn { get; }
    public double Probability { get; }

    public Parameterscs(DataTable table, string filter, FactorInfo predictors, string responseColumn, double probability)
    {
      Table = table;
      Filter = filter;
      Predictors = predictors;
      ResponseColumn = responseColumn;
      Probability = probability;
    }
  }
}
