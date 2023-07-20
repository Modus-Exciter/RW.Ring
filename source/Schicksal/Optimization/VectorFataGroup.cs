using Schicksal.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
    public class VectorDataGroup : IDataGroup
    {
        private double[] m_values;

        public double[] Values { get { return m_values; } }

        public VectorDataGroup(double[] array)
        {
            if (array == null) throw new ArgumentNullException("array");
            m_values = new double[array.Length];
            Array.Copy(array, m_values, array.Length);
        }

        public int Count { get { return m_values.Length; } }

        public double this[int index] { get { return m_values[index]; } set { m_values[index] = value; } }

        public override string ToString() { return string.Join(" ", m_values.Select(x => x.ToString("E3"))); }

        public void Zeros() { Array.Clear(m_values, 0, this.Count); }

        public static implicit operator double[](VectorDataGroup a)
        {
            return a.m_values.ToArray();
        }

        public static VectorDataGroup operator -(VectorDataGroup a)
        {
            return new VectorDataGroup(a.m_values.Select(value => -value).ToArray());
        }

        public static VectorDataGroup operator +(VectorDataGroup a, VectorDataGroup b)
        {
            if (a.Count != b.Count) throw new ArgumentException("Dimensions of vectors don't agree");
            double[] res = new double[a.Count];
            for (int i = 0; i < res.Length; i++)
                res[i] = a[i] + b[i];
            return new VectorDataGroup(res);
        }

        public static VectorDataGroup operator -(VectorDataGroup a, VectorDataGroup b)
        {
            return a + (-b);
        }

        public static VectorDataGroup operator /(VectorDataGroup a, double b)
        {
            return new VectorDataGroup(a.m_values.Select(value => value / b).ToArray());
        }

        public static VectorDataGroup operator *(double b, VectorDataGroup a)
        {
            return new VectorDataGroup(a.m_values.Select(value => value * b).ToArray());
        }

        public static VectorDataGroup operator *(VectorDataGroup a, double b) { return b * a; }

        public IEnumerator<double> GetEnumerator()
        {
            return ((IList<double>)m_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
        }
    }
}
