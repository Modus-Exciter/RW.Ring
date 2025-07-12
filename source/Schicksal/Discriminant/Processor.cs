using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung;
using Schicksal.Basic;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using System.Data;
using Notung.Services;


namespace Schicksal.Discriminant
{
  public class Processor : IRunBase
  {
    private readonly Parameterscs _parameters;
    public DataTable Results { get; private set; }
    public DecisionTree Tree { get; private set; }

    public Processor(Parameterscs parameters)
    {
      _parameters = parameters;
    }

    public void Run()
    {
      var rows = _parameters.Table.Select(_parameters.Filter ?? "true");

      var predictors = _parameters.Predictors.Names;
      var resultColumn = _parameters.ResponseColumn;

      // Построение рабочей таблицы
      var workingTable = new DataTable();
      foreach (var col in predictors) workingTable.Columns.Add(col);
      workingTable.Columns.Add(resultColumn);

      foreach (var row in rows)
      {
        var newRow = workingTable.NewRow();
        foreach (var col in predictors)
          newRow[col] = row[col];
        newRow[resultColumn] = row[resultColumn];
        workingTable.Rows.Add(newRow);
      }

      // Кодирование категориальных переменных
      var codification = new Codification(workingTable);
      var symbols = codification.Apply(workingTable);

      int[][] inputs = symbols.ToJagged(predictors.ToArray());
      int[] outputs = symbols.ToArray(resultColumn);

      var attributes = predictors.Select(p => new DecisionVariable(p, DecisionVariableKind.Continuous)).ToArray();

      Tree = new DecisionTree(attributes, classCount: codification[resultColumn].NumberOfSymbols);
      var teacher = new C45Learning(Tree);
      teacher.Learn(inputs, outputs);

      // Предсказания
      int[] predicted = inputs.Select(Tree.Decide).ToArray();

      // Построение таблицы результатов
      Results = new DataTable();
      Results.Columns.Add("TrueClass");
      Results.Columns.Add("PredictedClass");

      var decodeClass = codification[resultColumn];

      for (int i = 0; i < predicted.Length; i++)
      {
        Results.Rows.Add(decodeClass.Revert(outputs[i]), decodeClass.Revert(predicted[i]));
      }
    }
  }
}

