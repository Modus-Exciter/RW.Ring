using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Data;

namespace NotungTest
{
  [TestClass]
  public class GraphTest
  {
    [TestMethod]
    public void UnweightedListAddArcOriented()
    {
      IUnweightedGraph graph = new UnweightedNestedList(6, true);

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

      Assert.AreEqual(2, graph.OutGoingCount(0));
      Assert.AreEqual(0, graph.OutGoingCount(3));
      Assert.AreEqual(1, graph.OutGoingCount(2));
      Assert.AreEqual(1, graph.OutGoingCount(4));
    }

    [TestMethod]
    public void UnweightedListAddArcUnoriented()
    {
      IUnweightedGraph graph = new UnweightedNestedList(6, false);

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

      Assert.AreEqual(2, graph.OutGoingCount(0));
      Assert.AreEqual(1, graph.OutGoingCount(3));
      Assert.AreEqual(1, graph.OutGoingCount(2));
      Assert.AreEqual(2, graph.OutGoingCount(4));
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumber()
    {
      IUnweightedGraph graph = new UnweightedNestedList(6, true);

      graph.AddArc(3, 6);
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void WrongPeakNumberReverse()
    {
      IUnweightedGraph graph = new UnweightedNestedList(6, true);

      graph.AddArc(6, 2);
    }

    [TestMethod]
    public void RemoveArcOrientedList()
    {
      IUnweightedGraph graph = new UnweightedNestedList(6, true);

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
      IUnweightedGraph graph = new UnweightedNestedList(6, false);

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
      IUnweightedGraph graph = new UnweightedNestedList(10, true);
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
      var res = TopologicalSort.Kan(graph);

      foreach (var item in res)
      {
        checker.Add(item);

        foreach (var arc in graph.IncomingArcs(item))
          Assert.IsTrue(checker.Contains(arc));
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void KanSortError()
    {
      IUnweightedGraph graph = new UnweightedNestedList(10, true);
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

      TopologicalSort.Kan(graph);
    }
  }
}
