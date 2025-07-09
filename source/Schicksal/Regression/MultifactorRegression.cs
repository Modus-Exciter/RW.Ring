using Notung.Data;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using static Schicksal.SpecialFunctions;

namespace Schicksal.Regression
{
  public class MultifactorRegression
  {
    /// <summary>
    /// Выполняет многофакторный регрессионный анализ с фильтрацией данных из таблицы.
    /// </summary>
    /// <param name="table">Исходная таблица данных.</param>
    /// <param name="predictorColumnNames">Имена столбцов-предикторов.</param>
    /// <param name="responseColumnName">Имя столбца зависимой переменной.</param>
    /// <param name="generalFilter">Общий фильтр для данных.</param>
    /// <param name="p">Уровень значимости для статистических тестов.</param>
    /// <returns>Результат регрессионного анализа.</returns>
    public MultifactorRegressionResult Perform(
        DataTable table,
        IEnumerable<string> predictorColumnNames,
        string responseColumnName,
        string generalFilter,
        double p)
    {
      // Создаем единый список всех столбцов, которые нужны для регрессии
      // Это нужно, чтобы фильтр на NULL-значения применялся ко всем участвующим столбцам
      // и все выборки (Y и X) имели одинаковое количество строк.
      var allRelevantColumns = new List<string>(predictorColumnNames);
      allRelevantColumns.Add(responseColumnName);

      // Формируем единую строку фильтра, которая будет применена ко ВСЕМ выборкам.
      // Этот метод CreateFilterStringForSample гарантирует, что NULL-значения
      // будут исключены из всех необходимых столбцов.
      string combinedFilterString = CreateFilterStringForSample(
          table,
          allRelevantColumns.ToArray(),
          generalFilter
      );

      IPlainSample y_sample = new DataColumnSample(
          table.Columns[responseColumnName],
          combinedFilterString
      );

      List<IPlainSample> x_samples = new List<IPlainSample>();
      foreach (var predictorName in predictorColumnNames)
      {
        x_samples.Add(new DataColumnSample(
            table.Columns[predictorName],
            combinedFilterString
        ));
      }

      FactorInfo factorInfo = new FactorInfo(predictorColumnNames);
      return this.Perform(factorInfo, x_samples, y_sample, p);
    }

    /// <summary>
    /// Выполняет многофакторную параболическую регрессию для двух факторов.
    /// </summary>
    /// <param name="factors">Информация о факторах.</param>
    /// <param name="xSamples">Выборки предикторов.</param>
    /// <param name="ySample">Выборка зависимой переменной.</param>
    /// <param name="p">Уровень значимости для тестов.</param>
    /// <returns>Результат регрессионного анализа.</returns>
    public MultifactorRegressionResult PerformParabolic(FactorInfo factors, List<IPlainSample> xSamples, IPlainSample ySample, double p)
    {
      var yValues = ySample.ToArray();
      var x1Values = xSamples[0].ToArray();
      var x2Values = xSamples[1].ToArray();
      int n = yValues.Length;

      var x1Squared = x1Values.Select(v => v * v).ToArray();
      var x2Squared = x2Values.Select(v => v * v).ToArray();
      var x1x2Interaction = new double[n];
      for (int i = 0; i < n; i++)
      {
        x1x2Interaction[i] = x1Values[i] * x2Values[i];
      }

      var parabolic_x_samples_list = new List<double[]>
      {
        x1Values,
        x2Values,
        x1Squared,
        x2Squared,
        x1x2Interaction
      };

      var factorNames = factors.ToArray();
      var parabolicFactorNames = new List<string>
      {
        factorNames[0],
        factorNames[1],
        $"{factorNames[0]}²",
        $"{factorNames[1]}²",
        $"{factorNames[0]}*{factorNames[1]}"
      };

      int k = parabolicFactorNames.Count;
      if (n <= k + 1)
      {
        throw new ArgumentException($"Недостаточно наблюдений ({n}) для выполнения параболической регрессии с {k} производными факторами. Требуется минимум {k + 2} наблюдений.");
      }

      var xMatrix = new RectangleMatrix<double>(n, k + 1);
      for (int i = 0; i < n; i++)
      {
        xMatrix[i, 0] = 1.0;
        for (int j = 0; j < k; j++)
        {
          xMatrix[i, j + 1] = parabolic_x_samples_list[j][i];
        }
      }

      var result = this.CalculateRegressionMetrics(xMatrix, yValues, parabolicFactorNames, n, k, p);
      result.ModelType = "Parabolic";
      return result;
    }

    /// <summary>
    /// Выполняет многофакторный линейный регрессионный анализ.
    /// </summary>
    /// <param name="factors">Информация о факторах.</param>
    /// <param name="xSamples">Выборки предикторов.</param>
    /// <param name="ySample">Выборка зависимой переменной.</param>
    /// <param name="p">Уровень значимости для тестов.</param>
    /// <returns>Результат регрессионного анализа.</returns>
    public MultifactorRegressionResult Perform(FactorInfo factors, List<IPlainSample> xSamples, IPlainSample ySample, double p)
    {
      int n = ySample.Count();
      int k = factors.Count;

      if (n == 0)
      {
        throw new ArgumentException("Выборка зависимой переменной пуста.");
      }
      if (k < 2)
      {
        throw new ArgumentException("Требуется минимум два фактора. Для анализа с одним фактором используйте модуль простой линейной регрессии.");
      }
      if (n <= k + 1)
      {
        throw new ArgumentException($"Недостаточно наблюдений ({n}). Требуется минимум {k + 2} наблюдений.");
      }

      var yValues = ySample.ToArray();
      var xValuesList = new List<double[]>();
      foreach (var sample in xSamples)
      {
        if (sample.Count() != n)
        {
          throw new InvalidOperationException($"Выборка предиктора имеет другое количество наблюдений ({sample.Count()}) по сравнению с зависимой переменной ({n}). Убедитесь, что фильтры согласованы.");
        }
        xValuesList.Add(sample.ToArray());
      }

      var xMatrix = new RectangleMatrix<double>(n, k + 1);
      for (int i = 0; i < n; i++)
      {
        xMatrix[i, 0] = 1.0;
        for (int j = 0; j < k; j++)
        {
          xMatrix[i, j + 1] = xValuesList[j][i];
        }
      }

      var factorNames = factors.ToList();
      var result = this.CalculateRegressionMetrics(xMatrix, yValues, factorNames, n, k, p);
      result.ModelType = "Linear";
      return result;
    }

    /// <summary>
    /// Вычисляет метрики регрессионного анализа.
    /// </summary>
    private MultifactorRegressionResult CalculateRegressionMetrics(RectangleMatrix<double> xMatrix, double[] yValues, List<string> factorNames, int n, int k, double p)
    {
      var yMatrix = new RectangleMatrix<double>(n, 1);
      for (int i = 0; i < n; i++)
      {
        yMatrix[i, 0] = yValues[i];
      }

      var culture = CultureInfo.InvariantCulture;
      var xTranspose = MatrixFunctions.Transpose(xMatrix);
      var xtx = MatrixFunctions.Multiply(xTranspose, xMatrix, culture);
      var xtxInverse = MatrixFunctions.Invert(xtx, culture);

      if (xtxInverse == null)
      {
        throw new Exception("Матрица факторов вырождена, возможна мультиколлинеарность.");
      }

      var xty = MatrixFunctions.Multiply(xTranspose, yMatrix, culture);
      var betaMatrix = MatrixFunctions.Multiply(xtxInverse, xty, culture);

      var coefficients = new double[k + 1];
      for (int i = 0; i < k + 1; i++)
      {
        coefficients[i] = betaMatrix[i, 0];
      }

      var yHatMatrix = MatrixFunctions.Multiply(xMatrix, betaMatrix, culture);
      var yHatValues = new double[n];
      for (int i = 0; i < n; i++)
      {
        yHatValues[i] = yHatMatrix[i, 0];
      }

      double ySum = 0;
      for (int i = 0; i < n; i++)
      {
        ySum += yValues[i];
      }
      double yMean = ySum / n;

      double sst = 0;
      for (int i = 0; i < n; i++)
      {
        double diff = yValues[i] - yMean;
        sst += diff * diff;
      }

      double sse = 0;
      for (int i = 0; i < n; i++)
      {
        double diff = yHatValues[i] - yMean;
        sse += diff * diff;
      }

      double ssr = 0;
      for (int i = 0; i < n; i++)
      {
        double diff = yValues[i] - yHatValues[i];
        ssr += diff * diff;
      }

      double rSquared = sst > 0 ? sse / sst : 0;
      double adjustedRSquared = (n - k - 1 > 0) ? 1 - ((1 - rSquared) * (n - 1) / (n - k - 1)) : double.NaN;

      int dfRegression = k;
      int dfResidual = n - k - 1;
      double mse = dfRegression > 0 ? sse / dfRegression : 0;
      double msr = dfResidual > 0 ? ssr / dfResidual : 0;
      double fStatistic = msr > 0 ? mse / msr : 0;
      double pValueFStatistic = dfRegression > 0 && dfResidual > 0 ? 1.0 - fdistribution(dfRegression, dfResidual, fStatistic) : 1.0;

      var standardErrors = new double[k + 1];
      var tStatistics = new double[k + 1];
      var pValuesTStatistics = new double[k + 1];
      var covarianceMatrix = new RectangleMatrix<double>(xtxInverse.RowCount, xtxInverse.ColumnCount);
      for (int row = 0; row < xtxInverse.RowCount; row++)
      {
        for (int col = 0; col < xtxInverse.ColumnCount; col++)
        {
          covarianceMatrix[row, col] = xtxInverse[row, col] * msr;
        }
      }

      for (int i = 0; i < k + 1; i++)
      {
        standardErrors[i] = covarianceMatrix[i, i] >= 0 ? Math.Sqrt(covarianceMatrix[i, i]) : 0;
        tStatistics[i] = standardErrors[i] > 0 ? coefficients[i] / standardErrors[i] : 0;
        pValuesTStatistics[i] = dfResidual > 0 ? (1 - SpecialFunctions.studenttdistribution(dfResidual, Math.Abs(tStatistics[i]))) * 2 : 1.0;
      }

      var factorNamesResult = new string[factorNames.Count + 1];
      factorNamesResult[0] = "Свободный член";
      for (int i = 0; i < factorNames.Count; i++)
      {
        factorNamesResult[i + 1] = factorNames[i];
      }

      return new MultifactorRegressionResult
      {
        Coefficients = coefficients,
        FactorNames = factorNamesResult,
        RSquared = rSquared,
        AdjustedRSquared = adjustedRSquared,
        FStatistic = fStatistic,
        PValueFStatistic = pValueFStatistic,
        StandardErrors = standardErrors,
        TStatistics = tStatistics,
        PValuesTStatistics = pValuesTStatistics,
        ResidualSumOfSquares = ssr,
        TotalSumOfSquares = sst,
        DegreesOfFreedomResidual = dfResidual,
        DegreesOfFreedomRegression = dfRegression
      };
    }

    /// <summary>
    /// Создает строку фильтра для исключения NULL-значений и применения общего фильтра.
    /// </summary>
    public static string CreateFilterStringForSample(DataTable table, string[] columnNames, string generalFilter)
    {
      var expressions = new List<string>();

      foreach (var colName in columnNames)
      {
        if (table.Columns.Contains(colName) && table.Columns[colName].AllowDBNull)
        {
          expressions.Add(string.Format("[{0}] is not null", colName));
        }
      }

      if (!string.IsNullOrEmpty(generalFilter))
      {
        expressions.Add(generalFilter);
      }

      if (expressions.Count > 0)
      {
        return string.Join(" and ", expressions);
      }
      return null;
    }
  }
}