using System;
using System.Collections.Generic;
using System.Linq;
using Notung;
using Notung.Data;
using Notung.Logging;
using Notung.Threading;
using Schicksal.Basic;
using Schicksal.Properties;

namespace Schicksal.Anova
{
  /// <summary>
  /// Первичные результаты дисперсионного анализа
  /// </summary>
  public interface IPrimaryAnovaResults
  {
    /// <summary>
    /// Алгоритм расчёта остаточной дисперсии
    /// </summary>
    IResudualsCalculator ResudualsCalculator { get; }

    /// <summary>
    /// Преобразователь нормированных данных в ненормированные и обратно
    /// </summary>
    IValueTransform ValueTransform { get; }

    /// <summary>
    /// Результаты теста по критерию Фишера
    /// </summary>
    FisherTestResult[] FisherTestResults { get; }

    /// <summary>
    /// Анализируемый набор данных
    /// </summary>
    IDividedSample<GroupKey> DataSet { get; }

    /// <summary>
    /// Параметры анализа
    /// </summary>
    AnovaParameters Parameters { get; }
  }

  /// <summary>
  /// Задача дисперсионного анализа таблицы
  /// </summary>
  public class AnovaCalculator : RunBase, IProgressIndicator, IPrimaryAnovaResults, IServiceProvider
  {
    private readonly AnovaParameters m_parameters;
    private IResudualsCalculator m_residuals_calculator;
    private IValueTransform m_transform;
    private IDividedSample<GroupKey> m_data_set;

    private static readonly ILog _log = LogManager.GetLogger(typeof(AnovaCalculator));

    /// <summary>
    /// Инициализация задачи дисперсионного анализа таблицы
    /// </summary>
    /// <param name="parameters">Набор параметров - таблица с опциями</param>
    public AnovaCalculator(AnovaParameters parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_parameters = parameters;
    }

    /// <summary>
    /// Объект вычисления остатков, требуемый для анализа
    /// </summary>
    public IResudualsCalculator ResudualsCalculator
    {
      get { return m_residuals_calculator; }
    }

    /// <summary>
    /// Преобразователь нормированных данных в ненормированные
    /// </summary>
    public IValueTransform ValueTransform
    {
      get { return m_transform; }
    }

    /// <summary>
    /// Параметры анализа
    /// </summary>
    public AnovaParameters Parameters
    {
      get { return m_parameters; }
    }

    /// <summary>
    /// Анализируемый набор данных
    /// </summary>
    public IDividedSample<GroupKey> DataSet
    {
      get { return m_data_set; }
    }

    /// <summary>
    /// Результаты теста по критерию Фишера
    /// </summary>
    public FisherTestResult[] FisherTestResults { get; private set; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    public override void Run()
    {
      _log.Info("ANOVA started");
      
      this.ReportProgress(Resources.PREPARING_DATA);
      this.Initialize();

      m_residuals_calculator.Start(m_parameters, m_data_set, this);

      var between = new List<FactorVariance>();
      var within = new List<SampleVariance>();

      int totals = 1 << m_parameters.Predictors.Count;
      int current = 0;

      _log.Info("Calculating between variance");

      foreach (var p in m_residuals_calculator.GetSupportedFactors())
      {
        var parameters = new PredictedResponseParameters(m_parameters.Table,
          m_parameters.Filter, p, m_parameters.Response);

        var sample = GroupKey.Repack(m_data_set, p);
        var ms_b = FisherTest.MSb(sample);

        this.AddPredictorResult(between, within, p, ms_b);

        _log.DebugFormat("Factor: {0}, data set: {1}, equal subsamples size: {2}, between variance: {3}",
          p, sample, sample is IEqualSubSamples, ms_b);

        current++;

        this.ReportProgress(current * 100 / totals, p.ToString());
      }

      if (m_residuals_calculator.SingleWihinVariance)
      {
        var ms_w = m_residuals_calculator.GetWithinVariance(m_parameters.Predictors, this);

        _log.Info("Calculating interactions");
        _log.DebugFormat("Within variance: {0}", ms_w);

        foreach (var item in between)
          within.Add(ms_w);

        FisherTest.FactorInteraction(between, this.ProcessInteractionError);
      }

      this.FisherTestResults = this.ConvertResult(between, within);

      _log.Info("ANOVA completed");
    }

    void IProgressIndicator.ReportProgress(int percentage, string state)
    {
      base.ReportProgress(percentage, state);
    }

    private void ProcessInteractionError(FactorInfo predictor)
    {
      if (predictor.Count == 1)
        this.Infolog.Add(string.Format("No main effect information for factor {0}", predictor), InfoLevel.Warning);
      else
        this.Infolog.Add(string.Format("Factor {0} has wrong sample count", predictor), InfoLevel.Warning);
    }

    private void AddPredictorResult(List<FactorVariance> between, List<SampleVariance> within, FactorInfo p, SampleVariance ms_b)
    {
      between.Add(new FactorVariance(p, ms_b));

      if (!m_residuals_calculator.SingleWihinVariance)
        within.Add(m_residuals_calculator.GetWithinVariance(p, this));
    }

    private void Initialize()
    {
      var table = new TableDividedSample(m_parameters, m_parameters.Conjugation);

      m_transform = m_parameters.Normalizer.Prepare(m_parameters.Normalizer.Normalize(table));
      m_data_set = SampleRepack.Wrap(new ArrayDividedSample<GroupKey>(m_transform.Normalize(table), table.GetKey));

      _log.DebugFormat("Normalization value transform: {0}", m_transform);
      _log.DebugFormat("Data set: {0}, equal subsamples size: {1}", m_data_set, m_data_set is IEqualSubSamples);

      if (table.Sum(g => g.Count) > table.Count)
      {
        if (string.IsNullOrEmpty(m_parameters.Conjugation) || !(m_data_set is IEqualSubSamples))
          m_residuals_calculator = new IndenepdentResudualsCalculator();
        else
          m_residuals_calculator = new ConjugatedResudualsCalculator();
      }
      else
        m_residuals_calculator = new UnrepeatedResudualsCalculator();

      if (m_residuals_calculator is IndenepdentResudualsCalculator && !string.IsNullOrEmpty(m_parameters.Conjugation))
        this.Infolog.Add(Resources.UNABLE_CONJUGATION, InfoLevel.Warning);

      _log.DebugFormat("Residuals caclulation method: {0}", m_residuals_calculator);
    }

    private FisherTestResult[] ConvertResult(List<FactorVariance> between, List<SampleVariance> within)
    {
      var ret = new FisherTestResult[between.Count];

      for (int i = 0; i < ret.Length; i++)
      {
        ret[i] = new FisherTestResult
        {
          Factor = between[i].Factor,
          F = between[i].Variance.MeanSquare / within[i].MeanSquare,
          SSw = within[i].SumOfSquares,
          Kdf = (uint)between[i].Variance.DegreesOfFreedom,
          Ndf = (uint)within[i].DegreesOfFreedom,
          FCritical = FisherTest.GetCriticalValue
            (
              m_parameters.Probability,
              (uint)between[i].Variance.DegreesOfFreedom,
              (uint)within[i].DegreesOfFreedom
            ),
          P = FisherTest.GetProbability(between[i].Variance, within[i])
        };
      }

      return ret;
    }
  }
}