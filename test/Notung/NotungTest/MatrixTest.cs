using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notung.Data;
using System.Collections.Generic;

namespace NotungTest
{
  [TestClass]
  public class MatrixTest
  {

    [TestMethod]
    public void FillMatrixWithDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, true);

      matrix[0, 0] = 1;
      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 1] = 6;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 2] = 10;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 3] = 13;
      matrix[3, 4] = 14;
      matrix[4, 4] = 15;

      Assert.AreEqual(1, matrix[0, 0]);
      Assert.AreEqual(2, matrix[0, 1]);
      Assert.AreEqual(3, matrix[0, 2]);
      Assert.AreEqual(4, matrix[0, 3]);
      Assert.AreEqual(5, matrix[0, 4]);
      Assert.AreEqual(6, matrix[1, 1]);
      Assert.AreEqual(7, matrix[1, 2]);
      Assert.AreEqual(8, matrix[1, 3]);
      Assert.AreEqual(9, matrix[1, 4]);
      Assert.AreEqual(10, matrix[2, 2]);
      Assert.AreEqual(11, matrix[3, 2]);
      Assert.AreEqual(12, matrix[2, 4]);
      Assert.AreEqual(13, matrix[3, 3]);
      Assert.AreEqual(14, matrix[3, 4]);
      Assert.AreEqual(15, matrix[4, 4]);
    }

    [TestMethod]
    public void FillMatrixWithoutDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, false);

      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 4] = 14;

      Assert.AreEqual(2, matrix[0, 1]);
      Assert.AreEqual(3, matrix[2, 0]);
      Assert.AreEqual(4, matrix[0, 3]);
      Assert.AreEqual(5, matrix[0, 4]);
      Assert.AreEqual(7, matrix[1, 2]);
      Assert.AreEqual(8, matrix[1, 3]);
      Assert.AreEqual(9, matrix[1, 4]);
      Assert.AreEqual(11, matrix[3, 2]);
      Assert.AreEqual(12, matrix[2, 4]);
      Assert.AreEqual(14, matrix[3, 4]);
    }

    private IEnumerable<T> GetRowData<T>(IMatrix<T> matrix, int index)
    {
      for (int i = 0; i < matrix.ColumnCount; i++)
      {
        if (i == index && matrix is TriangleMatrix<T>
          && !((TriangleMatrix<T>)matrix).WithDiagonal)
          yield return default(T);
        else
          yield return matrix[index, i];
      }
    }

    [TestMethod]
    public void MatrixRowWithDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, true);

      matrix[0, 0] = 1;
      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 1] = 6;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 2] = 10;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 3] = 13;
      matrix[3, 4] = 14;
      matrix[4, 4] = 15;

      var row = this.GetRowData(matrix, 3).ToArray();

      Assert.AreEqual(5, row.Length);

      Assert.AreEqual(4, row[0]);
      Assert.AreEqual(8, row[1]);
      Assert.AreEqual(11, row[2]);
      Assert.AreEqual(13, row[3]);
      Assert.AreEqual(14, row[4]);
    }

    [TestMethod]
    public void MatrixRowWithoutDiagonal()
    {
      TriangleMatrix<int> matrix = new TriangleMatrix<int>(5, false);

      matrix[0, 1] = 2;
      matrix[0, 2] = 3;
      matrix[0, 3] = 4;
      matrix[0, 4] = 5;
      matrix[1, 2] = 7;
      matrix[3, 1] = 8;
      matrix[1, 4] = 9;
      matrix[2, 3] = 11;
      matrix[2, 4] = 12;
      matrix[3, 4] = 14;

      var row = this.GetRowData(matrix, 3).ToArray();

      Assert.AreEqual(5, row.Length);
      Assert.AreEqual(4, row[0]);
      Assert.AreEqual(8, row[1]);
      Assert.AreEqual(11, row[2]);
      Assert.AreEqual(0, row[3]);
      Assert.AreEqual(14, row[4]);
    }
  }
}