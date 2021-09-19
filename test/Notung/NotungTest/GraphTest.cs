using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Data;
using Notung.Loader;

namespace NotungTest
{
  [TestClass]
  public class GraphTest
  {
    [TestMethod]
    public void UnweightedListAddArcOriented()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, true);

      Assert.IsTrue(graph.IsOriented);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsTrue(graph.AddArc(4, 2));

      Assert.IsTrue(graph.HasArc(0, 3));
      Assert.IsTrue(graph.HasArc(0, 4));
      Assert.IsTrue(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
      Assert.IsFalse(graph.HasArc(3, 0));
      Assert.IsFalse(graph.HasArc(4, 0));

      Assert.AreEqual(1, graph.IncomingCount(3));
      Assert.AreEqual(0, graph.IncomingCount(5));
      Assert.AreEqual(1, graph.IncomingCount(2));
      Assert.AreEqual(2, graph.IncomingCount(4));

      Assert.AreEqual(2, graph.OutgoingCount(0));
      Assert.AreEqual(0, graph.OutgoingCount(3));
      Assert.AreEqual(1, graph.OutgoingCount(2));
      Assert.AreEqual(1, graph.OutgoingCount(4));
    }

    [TestMethod]
    public void UnweightedListAddArcUnoriented()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, false);

      Assert.IsFalse(graph.IsOriented);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsFalse(graph.AddArc(4, 2));

      Assert.IsTrue(graph.HasArc(0, 3));
      Assert.IsTrue(graph.HasArc(0, 4));
      Assert.IsTrue(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
      Assert.IsTrue(graph.HasArc(3, 0));
      Assert.IsTrue(graph.HasArc(4, 0));

      Assert.AreEqual(1, graph.IncomingCount(3));
      Assert.AreEqual(0, graph.IncomingCount(5));
      Assert.AreEqual(1, graph.IncomingCount(2));
      Assert.AreEqual(2, graph.IncomingCount(4));

      Assert.AreEqual(2, graph.OutgoingCount(0));
      Assert.AreEqual(1, graph.OutgoingCount(3));
      Assert.AreEqual(1, graph.OutgoingCount(2));
      Assert.AreEqual(2, graph.OutgoingCount(4));
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumber()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, true);

      graph.AddArc(3, 6);
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumberReverse()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, true);

      graph.AddArc(-1, 2);
    }

    [TestMethod]
    public void RemoveArcOrientedList()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, true);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsTrue(graph.AddArc(4, 2));

      Assert.IsFalse(graph.RemoveArc(2, 5));
      Assert.IsTrue(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
    }

    [TestMethod]
    public void RemoveArcUnorientedList()
    {
      IUnweightedGraph graph = new UnweightedListGraph(6, false);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsFalse(graph.AddArc(4, 2));

      Assert.IsFalse(graph.RemoveArc(2, 5));
      Assert.IsTrue(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.HasArc(2, 4));
      Assert.IsFalse(graph.HasArc(4, 2));
    }

    [TestMethod]
    public void KanSort()
    {
      IUnweightedGraph graph = new UnweightedListGraph(10, true);

      graph.AddArc(0, 5);
      graph.AddArc(0, 7);
      graph.AddArc(1, 6);
      graph.AddArc(2, 3);
      graph.AddArc(5, 4);
      graph.AddArc(7, 4);
      graph.AddArc(7, 6);
      graph.AddArc(8, 2);
      graph.AddArc(9, 7);
      graph.AddArc(9, 1);
      graph.AddArc(9, 8);

      HashSet<int> checker = new HashSet<int>();
      var res = TopologicalSort.Kahn(graph);

      Assert.AreEqual(graph.PeakCount, res.Length);

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    #region Matrix

    [TestMethod]
    public void UnweightedMatrixAddArcOriented()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, true);

      Assert.IsTrue(graph.IsOriented);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsTrue(graph.AddArc(4, 2));

      Assert.IsTrue(graph.HasArc(0, 3));
      Assert.IsTrue(graph.HasArc(0, 4));
      Assert.IsTrue(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
      Assert.IsFalse(graph.HasArc(3, 0));
      Assert.IsFalse(graph.HasArc(4, 0));

      Assert.AreEqual(1, graph.IncomingCount(3));
      Assert.AreEqual(0, graph.IncomingCount(5));
      Assert.AreEqual(1, graph.IncomingCount(2));
      Assert.AreEqual(2, graph.IncomingCount(4));

      Assert.AreEqual(2, graph.OutgoingCount(0));
      Assert.AreEqual(0, graph.OutgoingCount(3));
      Assert.AreEqual(1, graph.OutgoingCount(2));
      Assert.AreEqual(1, graph.OutgoingCount(4));
    }

    [TestMethod]
    public void UnweightedMatrixAddArcUnoriented()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, false);

      Assert.IsFalse(graph.IsOriented);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsFalse(graph.AddArc(4, 2));

      Assert.IsTrue(graph.HasArc(0, 3));
      Assert.IsTrue(graph.HasArc(0, 4));
      Assert.IsTrue(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
      Assert.IsTrue(graph.HasArc(3, 0));
      Assert.IsTrue(graph.HasArc(4, 0));

      Assert.AreEqual(1, graph.IncomingCount(3));
      Assert.AreEqual(0, graph.IncomingCount(5));
      Assert.AreEqual(1, graph.IncomingCount(2));
      Assert.AreEqual(2, graph.IncomingCount(4));

      Assert.AreEqual(2, graph.OutgoingCount(0));
      Assert.AreEqual(1, graph.OutgoingCount(3));
      Assert.AreEqual(1, graph.OutgoingCount(2));
      Assert.AreEqual(2, graph.OutgoingCount(4));
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumberMatrix()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, true);

      graph.AddArc(3, 6);
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumberReverseMatrix()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, true);

      graph.AddArc(6, 2);
    }

    [TestMethod]
    public void RemoveArcOrientedMatrix()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, true);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsTrue(graph.AddArc(4, 2));

      Assert.IsFalse(graph.RemoveArc(2, 5));
      Assert.IsTrue(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.HasArc(2, 4));
      Assert.IsTrue(graph.HasArc(4, 2));
    }

    [TestMethod]
    public void RemoveArcUnorientedMatrix()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(6, false);

      Assert.IsTrue(graph.AddArc(0, 3));
      Assert.IsTrue(graph.AddArc(0, 4));
      Assert.IsTrue(graph.AddArc(2, 4));
      Assert.IsFalse(graph.AddArc(4, 2));

      Assert.IsFalse(graph.RemoveArc(2, 5));
      Assert.IsTrue(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.RemoveArc(2, 4));
      Assert.IsFalse(graph.HasArc(2, 4));
      Assert.IsFalse(graph.HasArc(4, 2));
    }

    [TestMethod]
    public void KanSortMatrix()
    {
      IUnweightedGraph graph = new UnweightedMatrixGraph(10, true);

      graph.AddArc(0, 5);
      graph.AddArc(0, 7);
      graph.AddArc(1, 6);
      graph.AddArc(2, 3);
      graph.AddArc(5, 4);
      graph.AddArc(7, 4);
      graph.AddArc(7, 6);
      graph.AddArc(8, 2);
      graph.AddArc(9, 7);
      graph.AddArc(9, 1);
      graph.AddArc(9, 8);

      HashSet<int> checker = new HashSet<int>();
      var res = TopologicalSort.Kahn(graph);

      Assert.AreEqual(graph.PeakCount, res.Length);

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    #endregion

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void KanSortError()
    {
      IUnweightedGraph graph = new UnweightedListGraph(10, true);

      graph.AddArc(0, 5);
      graph.AddArc(0, 7);
      graph.AddArc(1, 6);
      graph.AddArc(2, 3);
      graph.AddArc(5, 4);
      graph.AddArc(7, 4);
      graph.AddArc(7, 6);
      graph.AddArc(8, 2);
      graph.AddArc(9, 7);
      graph.AddArc(9, 1);
      graph.AddArc(9, 8);
      graph.AddArc(3, 9);

      TopologicalSort.Kahn(graph);
    }


    [TestMethod]
    public void TarjanSort()
    {
      IUnweightedGraph graph = new UnweightedListGraph(10, true);

      graph.AddArc(0, 5);
      graph.AddArc(0, 7);
      graph.AddArc(1, 6);
      graph.AddArc(2, 3);
      graph.AddArc(5, 4);
      graph.AddArc(7, 4);
      graph.AddArc(7, 6);
      graph.AddArc(8, 2);
      graph.AddArc(9, 7);
      graph.AddArc(9, 1);
      graph.AddArc(9, 8);

      HashSet<int> checker = new HashSet<int>();
      var res = TopologicalSort.Tarjan(graph);

      Assert.AreEqual(graph.PeakCount, res.Length);

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TarjanSortError()
    {
      IUnweightedGraph graph = new UnweightedListGraph(10, true);

      graph.AddArc(0, 5);
      graph.AddArc(0, 7);
      graph.AddArc(1, 6);
      graph.AddArc(2, 3);
      graph.AddArc(5, 4);
      graph.AddArc(7, 4);
      graph.AddArc(7, 6);
      graph.AddArc(8, 2);
      graph.AddArc(9, 7);
      graph.AddArc(9, 1);
      graph.AddArc(9, 8);
      graph.AddArc(3, 9);

      TopologicalSort.Tarjan(graph);
    }


    [TestMethod]
    public void ConvertAndSortKahn()
    {
      CharItem[] items = new CharItem[]
      {
        new CharItem('A', new char[] { 'E', 'C', 'D'}),
        new CharItem('B', new char[] { 'C', 'G'}),
        new CharItem('C', new char[0]),
        new CharItem('D', new char[] { 'F', 'G' }),
        new CharItem('E', new char[0]),
        new CharItem('F', new char[0]),
        new CharItem('G', new char[] { 'C' }),
      };

      var graph = items.ToUnweightedGraph();
      var res = TopologicalSort.Kahn(graph);

      Assert.AreEqual(graph.PeakCount, res.Length);
      HashSet<int> checker = new HashSet<int>();

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    [TestMethod]
    public void ConvertAndSortTarjan()
    {
      CharItem[] items = new CharItem[]
      {
        new CharItem('A', new char[] { 'E', 'C', 'D'}),
        new CharItem('B', new char[] { 'C', 'G'}),
        new CharItem('C', new char[0]),
        new CharItem('D', new char[] { 'F', 'G' }),
        new CharItem('E', new char[0]),
        new CharItem('F', new char[0]),
        new CharItem('G', new char[] { 'C' }),
      };

      var graph = items.ToUnweightedGraph();
      var res = TopologicalSort.Tarjan(graph);

      Assert.AreEqual(graph.PeakCount, res.Length);
      HashSet<int> checker = new HashSet<int>();

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    [TestMethod]
    public void ConvertToGraph()
    {
      CharItem[] items = new CharItem[]
      {
        new CharItem('A', new char[] { 'E', 'C', 'D'}),
        new CharItem('B', new char[] { 'C', 'G'}),
        new CharItem('C', new char[0]),
        new CharItem('D', new char[] { 'F', 'G' }),
        new CharItem('E', new char[0]),
        new CharItem('F', new char[0]),
        new CharItem('G', new char[] { 'C' }),
      };

      var graph = items.ToUnweightedGraph();

      Assert.IsFalse(graph.HasArc(0, 1));
      Assert.IsFalse(graph.HasArc(0, 2));
      Assert.IsFalse(graph.HasArc(0, 3));
      Assert.IsFalse(graph.HasArc(0, 4));
      Assert.IsFalse(graph.HasArc(0, 5));
      Assert.IsFalse(graph.HasArc(0, 6));

      Assert.IsFalse(graph.HasArc(1, 0));
      Assert.IsFalse(graph.HasArc(1, 2));
      Assert.IsFalse(graph.HasArc(1, 3));
      Assert.IsFalse(graph.HasArc(1, 4));
      Assert.IsFalse(graph.HasArc(1, 5));
      Assert.IsFalse(graph.HasArc(1, 6));

      Assert.IsTrue(graph.HasArc(2, 0));
      Assert.IsTrue(graph.HasArc(2, 1));
      Assert.IsFalse(graph.HasArc(2, 3));
      Assert.IsFalse(graph.HasArc(2, 4));
      Assert.IsFalse(graph.HasArc(2, 5));
      Assert.IsTrue(graph.HasArc(2, 6));

      Assert.IsTrue(graph.HasArc(3, 0));
      Assert.IsFalse(graph.HasArc(3, 1));
      Assert.IsFalse(graph.HasArc(3, 2));
      Assert.IsFalse(graph.HasArc(3, 4));
      Assert.IsFalse(graph.HasArc(3, 5));
      Assert.IsFalse(graph.HasArc(3, 6));

      Assert.IsTrue(graph.HasArc(4, 0));
      Assert.IsFalse(graph.HasArc(4, 1));
      Assert.IsFalse(graph.HasArc(4, 2));
      Assert.IsFalse(graph.HasArc(4, 3));
      Assert.IsFalse(graph.HasArc(4, 5));
      Assert.IsFalse(graph.HasArc(4, 6));

      Assert.IsFalse(graph.HasArc(5, 0));
      Assert.IsFalse(graph.HasArc(5, 1));
      Assert.IsFalse(graph.HasArc(5, 2));
      Assert.IsTrue(graph.HasArc(5, 3));
      Assert.IsFalse(graph.HasArc(5, 4));
      Assert.IsFalse(graph.HasArc(5, 6));

      Assert.IsFalse(graph.HasArc(6, 0));
      Assert.IsTrue(graph.HasArc(6, 1));
      Assert.IsFalse(graph.HasArc(6, 2));
      Assert.IsTrue(graph.HasArc(6, 3));
      Assert.IsFalse(graph.HasArc(6, 4));
      Assert.IsFalse(graph.HasArc(6, 5));
    }

    [TestMethod]
    public void FixSequence()
    {
      var items = new List<CharItem>
      {
        new CharItem('A', new char[] { 'E', 'C', 'D'}),
        new CharItem('B', new char[] { 'C', 'G', 'R'}),
        new CharItem('C', new char[0]),
        new CharItem('B', new char[] { 'C', 'G'}),
        new CharItem('D', new char[] { 'F', 'G' }),
        new CharItem('E', new char[0]),
        new CharItem('B', new char[] { 'C', 'G', 'A'}),
        new CharItem('F', new char[0]),
        new CharItem('G', new char[] { 'C' }),
      };

      items.Fix<char, CharItem>();
      Assert.AreEqual(7, items.Count);
      Assert.AreEqual('A', items[0].Key);
      Assert.AreEqual('B', items[1].Key);
      Assert.AreEqual('C', items[2].Key);
      Assert.AreEqual('D', items[3].Key);
      Assert.AreEqual('E', items[4].Key);
      Assert.AreEqual('F', items[5].Key);
      Assert.AreEqual('G', items[6].Key);

      Assert.AreEqual(2, items[1].Dependencies.Count);
      Assert.AreEqual('C', items[1].Dependencies.First());
      Assert.AreEqual('G', items[1].Dependencies.Skip(1).First());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void FixFailure()
    {
      var items = new List<CharItem>
      {
        new CharItem('A', new char[] { 'E', 'C', 'D'}),
        new CharItem('B', new char[] { 'C', 'G', 'R'}),
        new CharItem('C', new char[0]),
        new CharItem('B', new char[] { 'C', 'S'}),
        new CharItem('D', new char[] { 'F', 'G' }),
        new CharItem('E', new char[0]),
        new CharItem('B', new char[] { 'C', 'G', 'M'}),
        new CharItem('F', new char[0]),
        new CharItem('G', new char[] { 'C' }),
      };

      items.Fix<char, CharItem>();
    }

    private class CharItem : IDependencyItem<char>
    {
      public CharItem(char key, char[] dependencies)
      {
        this.Key = key;
        this.Dependencies = dependencies ?? new char[0];
      }

      public char Key { get; private set; }

      public ICollection<char> Dependencies { get; private set; }
    }
  }
}
