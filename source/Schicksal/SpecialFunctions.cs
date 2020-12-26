using System;
using System.Diagnostics;

namespace Schicksal
{
  /// <summary>
  /// Code from free version of alglib for C#
  /// </summary>
  internal static class SpecialFunctions
  {
    #region Helper

    const double machineepsilon = 5E-16;
    const double maxrealnumber = 1E300;
    const double minrealnumber = 1E-300;

    private static bool isfinite(double d)
    {
      return !System.Double.IsNaN(d) && !System.Double.IsInfinity(d);
    }

    private static double sqr(double X)
    {
      return X * X;
    }

    #endregion

    public static double fdistribution(int a, int b, double x)
    {
      double result = 0;
      double w = 0;

      Debug.Assert((a >= 1 && b >= 1) && (double)(x) >= (double)(0), "Domain error in FDistribution");
      w = a * x;
      w = w / (b + w);
      result = incompletebeta(0.5 * a, 0.5 * b, w);
      return result;
    }

    public static double fcdistribution(int a, int b, double x)
    {
      double result = 0;
      double w = 0;

      Debug.Assert((a >= 1 && b >= 1) && (double)(x) >= (double)(0), "Domain error in FCDistribution");
      w = b / (b + a * x);
      result = incompletebeta(0.5 * b, 0.5 * a, w);
      return result;
    }

    public static double invfdistribution(int a, int b, double y)
    {
      double result = 0;
      double w = 0;

      Debug.Assert(((a >= 1 && b >= 1) && (double)(y) > (double)(0)) && (double)(y) <= (double)(1), "Domain error in InvFDistribution");

      //
      // Compute probability for x = 0.5
      //
      w = incompletebeta(0.5 * b, 0.5 * a, 0.5);

      //
      // If that is greater than y, then the solution w < .5
      // Otherwise, solve at 1-y to remove cancellation in (b - b*w)
      //
      if ((double)(w) > (double)(y) || (double)(y) < (double)(0.001))
      {
        w = invincompletebeta(0.5 * b, 0.5 * a, y);
        result = (b - b * w) / (a * w);
      }
      else
      {
        w = invincompletebeta(0.5 * a, 0.5 * b, 1.0 - y);
        result = b * w / (a * (1.0 - w));
      }
      return result;
    }

    /*************************************************************************
    Student's t distribution

    Computes the integral from minus infinity to t of the Student
    t distribution with integer k > 0 degrees of freedom:

                                         t
                                         -
                                        | |
                 -                      |         2   -(k+1)/2
                | ( (k+1)/2 )           |  (     x   )
          ----------------------        |  ( 1 + --- )        dx
                        -               |  (      k  )
          sqrt( k pi ) | ( k/2 )        |
                                      | |
                                       -
                                      -inf.

    Relation to incomplete beta integral:

           1 - stdtr(k,t) = 0.5 * incbet( k/2, 1/2, z )
    where
           z = k/(k + t**2).

    For t < -2, this is the method of computation.  For higher t,
    a direct method is derived from integration by parts.
    Since the function is symmetric about t=0, the area under the
    right tail of the density is found by calling the function
    with -t instead of t.

    ACCURACY:

    Tested at random 1 <= k <= 25.  The "domain" refers to t.
                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE     -100,-2      50000       5.9e-15     1.4e-15
       IEEE     -2,100      500000       2.7e-15     4.9e-17

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double studenttdistribution(int k, double t)
    {
      double result = 0;
      double x = 0;
      double rk = 0;
      double z = 0;
      double f = 0;
      double tz = 0;
      double p = 0;
      double xsqk = 0;
      int j = 0;

      Debug.Assert(k > 0, "Domain error in StudentTDistribution");
      if ((double)(t) == (double)(0))
      {
        result = 0.5;
        return result;
      }
      if ((double)(t) < (double)(-2.0))
      {
        rk = k;
        z = rk / (rk + t * t);
        result = 0.5 * incompletebeta(0.5 * rk, 0.5, z);
        return result;
      }
      if ((double)(t) < (double)(0))
      {
        x = -t;
      }
      else
      {
        x = t;
      }
      rk = k;
      z = 1.0 + x * x / rk;
      if (k % 2 != 0)
      {
        xsqk = x / Math.Sqrt(rk);
        p = Math.Atan(xsqk);
        if (k > 1)
        {
          f = 1.0;
          tz = 1.0;
          j = 3;
          while (j <= k - 2 && (double)(tz / f) > (double)(machineepsilon))
          {
            tz = tz * ((j - 1) / (z * j));
            f = f + tz;
            j = j + 2;
          }
          p = p + f * xsqk / z;
        }
        p = p * 2.0 / Math.PI;
      }
      else
      {
        f = 1.0;
        tz = 1.0;
        j = 2;
        while (j <= k - 2 && (double)(tz / f) > (double)(machineepsilon))
        {
          tz = tz * ((j - 1) / (z * j));
          f = f + tz;
          j = j + 2;
        }
        p = f * x / Math.Sqrt(z * rk);
      }
      if ((double)(t) < (double)(0))
      {
        p = -p;
      }
      result = 0.5 + 0.5 * p;
      return result;
    }

    /*************************************************************************
    Functional inverse of Student's t distribution

    Given probability p, finds the argument t such that stdtr(k,t)
    is equal to p.

    ACCURACY:

    Tested at random 1 <= k <= 100.  The "domain" refers to p:
                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE    .001,.999     25000       5.7e-15     8.0e-16
       IEEE    10^-6,.001    25000       2.0e-12     2.9e-14

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invstudenttdistribution(int k, double p)
    {
      double result = 0;
      double t = 0;
      double rk = 0;
      double z = 0;
      int rflg = 0;

      Debug.Assert((k > 0 && (double)(p) > (double)(0)) && (double)(p) < (double)(1), "Domain error in InvStudentTDistribution");
      rk = k;
      if ((double)(p) > (double)(0.25) && (double)(p) < (double)(0.75))
      {
        if ((double)(p) == (double)(0.5))
        {
          result = 0;
          return result;
        }
        z = 1.0 - 2.0 * p;
        z = invincompletebeta(0.5, 0.5 * rk, Math.Abs(z));
        t = Math.Sqrt(rk * z / (1.0 - z));
        if ((double)(p) < (double)(0.5))
        {
          t = -t;
        }
        result = t;
        return result;
      }
      rflg = -1;
      if ((double)(p) >= (double)(0.5))
      {
        p = 1.0 - p;
        rflg = 1;
      }
      z = invincompletebeta(0.5 * rk, 0.5, 2.0 * p);
      if ((double)(maxrealnumber * z) < (double)(rk))
      {
        result = rflg * maxrealnumber;
        return result;
      }
      t = Math.Sqrt(rk / z - rk);
      result = rflg * t;
      return result;
    }

    /*************************************************************************
      Chi-square distribution

      Returns the area under the left hand tail (from 0 to x)
      of the Chi square probability density function with
      v degrees of freedom.


                                        x
                                         -
                             1          | |  v/2-1  -t/2
       P( x | v )   =   -----------     |   t      e     dt
                         v/2  -       | |
                        2    | (v/2)   -
                                        0

      where x is the Chi-square variable.

      The incomplete gamma integral is used, according to the
      formula

      y = chdtr( v, x ) = igam( v/2.0, x/2.0 ).

      The arguments must both be positive.

      ACCURACY:

      See incomplete gamma function


      Cephes Math Library Release 2.8:  June, 2000
      Copyright 1984, 1987, 2000 by Stephen L. Moshier
      *************************************************************************/
    public static double chisquaredistribution(double v,
        double x)
    {
      double result = 0;

      Debug.Assert((double)(x) >= (double)(0) && (double)(v) >= (double)(1), "Domain error in ChiSquareDistribution");
      result = incompletegamma(v / 2.0, x / 2.0);
      return result;
    }


    /*************************************************************************
    Complemented Chi-square distribution

    Returns the area under the right hand tail (from x to
    infinity) of the Chi square probability density function
    with v degrees of freedom:

                                     inf.
                                       -
                           1          | |  v/2-1  -t/2
     P( x | v )   =   -----------     |   t      e     dt
                       v/2  -       | |
                      2    | (v/2)   -
                                      x

    where x is the Chi-square variable.

    The incomplete gamma integral is used, according to the
    formula

    y = chdtr( v, x ) = igamc( v/2.0, x/2.0 ).

    The arguments must both be positive.

    ACCURACY:

    See incomplete gamma function

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double chisquarecdistribution(double v,
        double x)
    {
      double result = 0;

      Debug.Assert((double)(x) >= (double)(0) && (double)(v) >= (double)(1), "Domain error in ChiSquareDistributionC");
      result = incompletegammac(v / 2.0, x / 2.0);
      return result;
    }


    /*************************************************************************
    Inverse of complemented Chi-square distribution

    Finds the Chi-square argument x such that the integral
    from x to infinity of the Chi-square density is equal
    to the given cumulative probability y.

    This is accomplished using the inverse gamma integral
    function and the relation

       x/2 = igami( df/2, y );

    ACCURACY:

    See inverse incomplete gamma function


    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invchisquaredistribution(double v, double y)
    {
      double result = 0;

      Debug.Assert(((double)(y) >= (double)(0) && (double)(y) <= (double)(1)) && (double)(v) >= (double)(1), "Domain error in InvChiSquareDistribution");
      result = 2 * invincompletegammac(0.5 * v, y);
      return result;
    }


    /*************************************************************************
    Incomplete beta integral

    Returns incomplete beta integral of the arguments, evaluated
    from zero to x.  The function is defined as

                     x
        -            -
       | (a+b)      | |  a-1     b-1
     -----------    |   t   (1-t)   dt.
      -     -     | |
     | (a) | (b)   -
                    0

    The domain of definition is 0 <= x <= 1.  In this
    implementation a and b are restricted to positive values.
    The integral from x to 1 may be obtained by the symmetry
    relation

       1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).

    The integral is evaluated by a continued fraction expansion
    or, when b*x is small, by a power series.

    ACCURACY:

    Tested at uniformly distributed random points (a,b,x) with a and b
    in "domain" and x between 0 and 1.
                                           Relative error
    arithmetic   domain     # trials      peak         rms
       IEEE      0,5         10000       6.9e-15     4.5e-16
       IEEE      0,85       250000       2.2e-13     1.7e-14
       IEEE      0,1000      30000       5.3e-12     6.3e-13
       IEEE      0,10000    250000       9.3e-11     7.1e-12
       IEEE      0,100000    10000       8.7e-10     4.8e-11
    Outputs smaller than the IEEE gradual underflow threshold
    were excluded from these statistics.

    Cephes Math Library, Release 2.8:  June, 2000
    Copyright 1984, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double incompletebeta(double a, double b, double x)
    {
      double result = 0;
      double t = 0;
      double xc = 0;
      double w = 0;
      double y = 0;
      int flag = 0;
      double sg = 0;
      double big = 0;
      double biginv = 0;
      double maxgam = 0;
      double minlog = 0;
      double maxlog = 0;

      big = 4.503599627370496e15;
      biginv = 2.22044604925031308085e-16;
      maxgam = 171.624376956302725;
      minlog = Math.Log(minrealnumber);
      maxlog = Math.Log(maxrealnumber);
      Debug.Assert((double)(a) > (double)(0) && (double)(b) > (double)(0), "Domain error in IncompleteBeta");
      Debug.Assert((double)(x) >= (double)(0) && (double)(x) <= (double)(1), "Domain error in IncompleteBeta");
      if ((double)(x) == (double)(0))
      {
        result = 0;
        return result;
      }
      if ((double)(x) == (double)(1))
      {
        result = 1;
        return result;
      }
      flag = 0;
      if ((double)(b * x) <= (double)(1.0) && (double)(x) <= (double)(0.95))
      {
        result = incompletebetaps(a, b, x, maxgam);
        return result;
      }
      w = 1.0 - x;
      if ((double)(x) > (double)(a / (a + b)))
      {
        flag = 1;
        t = a;
        a = b;
        b = t;
        xc = x;
        x = w;
      }
      else
      {
        xc = w;
      }
      if ((flag == 1 && (double)(b * x) <= (double)(1.0)) && (double)(x) <= (double)(0.95))
      {
        t = incompletebetaps(a, b, x, maxgam);
        if ((double)(t) <= (double)(machineepsilon))
        {
          result = 1.0 - machineepsilon;
        }
        else
        {
          result = 1.0 - t;
        }
        return result;
      }
      y = x * (a + b - 2.0) - (a - 1.0);
      if ((double)(y) < (double)(0.0))
      {
        w = incompletebetafe(a, b, x, big, biginv);
      }
      else
      {
        w = incompletebetafe2(a, b, x, big, biginv) / xc;
      }
      y = a * Math.Log(x);
      t = b * Math.Log(xc);
      if (((double)(a + b) < (double)(maxgam) && (double)(Math.Abs(y)) < (double)(maxlog)) && (double)(Math.Abs(t)) < (double)(maxlog))
      {
        t = Math.Pow(xc, b);
        t = t * Math.Pow(x, a);
        t = t / a;
        t = t * w;
        t = t * (gammafunction(a + b) / (gammafunction(a) * gammafunction(b)));
        if (flag == 1)
        {
          if ((double)(t) <= (double)(machineepsilon))
          {
            result = 1.0 - machineepsilon;
          }
          else
          {
            result = 1.0 - t;
          }
        }
        else
        {
          result = t;
        }
        return result;
      }
      y = y + t + lngamma(a + b, ref sg) - lngamma(a, ref sg) - lngamma(b, ref sg);
      y = y + Math.Log(w / a);
      if ((double)(y) < (double)(minlog))
      {
        t = 0.0;
      }
      else
      {
        t = Math.Exp(y);
      }
      if (flag == 1)
      {
        if ((double)(t) <= (double)(machineepsilon))
        {
          t = 1.0 - machineepsilon;
        }
        else
        {
          t = 1.0 - t;
        }
      }
      result = t;
      return result;
    }

    /*************************************************************************
    Inverse of imcomplete beta integral

    Given y, the function finds x such that

     incbet( a, b, x ) = y .

    The routine performs interval halving or Newton iterations to find the
    root of incbet(a,b,x) - y = 0.


    ACCURACY:

                         Relative error:
                   x     a,b
    arithmetic   domain  domain  # trials    peak       rms
       IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
       IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
       IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
    With a and b constrained to half-integer or integer values:
       IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
       IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
    With a = .5, b constrained to half-integer or integer values:
       IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1996, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invincompletebeta(double a, double b, double y)
    {
      double result = 0;
      double aaa = 0;
      double bbb = 0;
      double y0 = 0;
      double d = 0;
      double yyy = 0;
      double x = 0;
      double x0 = 0;
      double x1 = 0;
      double lgm = 0;
      double yp = 0;
      double di = 0;
      double dithresh = 0;
      double yl = 0;
      double yh = 0;
      double xt = 0;
      int i = 0;
      int rflg = 0;
      int dir = 0;
      int nflg = 0;
      double s = 0;
      int mainlooppos = 0;
      int ihalve = 0;
      int ihalvecycle = 0;
      int newt = 0;
      int newtcycle = 0;
      int breaknewtcycle = 0;
      int breakihalvecycle = 0;

      i = 0;
      Debug.Assert((double)(y) >= (double)(0) && (double)(y) <= (double)(1), "Domain error in InvIncompleteBeta");

      //
      // special cases
      //
      if ((double)(y) == (double)(0))
      {
        result = 0;
        return result;
      }
      if ((double)(y) == (double)(1.0))
      {
        result = 1;
        return result;
      }

      //
      // these initializations are not really necessary,
      // but without them compiler complains about 'possibly uninitialized variables'.
      //
      dithresh = 0;
      rflg = 0;
      aaa = 0;
      bbb = 0;
      y0 = 0;
      x = 0;
      yyy = 0;
      lgm = 0;
      dir = 0;
      di = 0;

      //
      // normal initializations
      //
      x0 = 0.0;
      yl = 0.0;
      x1 = 1.0;
      yh = 1.0;
      nflg = 0;
      mainlooppos = 0;
      ihalve = 1;
      ihalvecycle = 2;
      newt = 3;
      newtcycle = 4;
      breaknewtcycle = 5;
      breakihalvecycle = 6;

      //
      // main loop
      //
      while (true)
      {

        //
        // start
        //
        if (mainlooppos == 0)
        {
          if ((double)(a) <= (double)(1.0) || (double)(b) <= (double)(1.0))
          {
            dithresh = 1.0e-6;
            rflg = 0;
            aaa = a;
            bbb = b;
            y0 = y;
            x = aaa / (aaa + bbb);
            yyy = incompletebeta(aaa, bbb, x);
            mainlooppos = ihalve;
            continue;
          }
          else
          {
            dithresh = 1.0e-4;
          }
          yp = -invnormaldistribution(y);
          if ((double)(y) > (double)(0.5))
          {
            rflg = 1;
            aaa = b;
            bbb = a;
            y0 = 1.0 - y;
            yp = -yp;
          }
          else
          {
            rflg = 0;
            aaa = a;
            bbb = b;
            y0 = y;
          }
          lgm = (yp * yp - 3.0) / 6.0;
          x = 2.0 / (1.0 / (2.0 * aaa - 1.0) + 1.0 / (2.0 * bbb - 1.0));
          d = yp * Math.Sqrt(x + lgm) / x - (1.0 / (2.0 * bbb - 1.0) - 1.0 / (2.0 * aaa - 1.0)) * (lgm + 5.0 / 6.0 - 2.0 / (3.0 * x));
          d = 2.0 * d;
          if ((double)(d) < (double)(Math.Log(minrealnumber)))
          {
            x = 0;
            break;
          }
          x = aaa / (aaa + bbb * Math.Exp(d));
          yyy = incompletebeta(aaa, bbb, x);
          yp = (yyy - y0) / y0;
          if ((double)(Math.Abs(yp)) < (double)(0.2))
          {
            mainlooppos = newt;
            continue;
          }
          mainlooppos = ihalve;
          continue;
        }

        //
        // ihalve
        //
        if (mainlooppos == ihalve)
        {
          dir = 0;
          di = 0.5;
          i = 0;
          mainlooppos = ihalvecycle;
          continue;
        }

        //
        // ihalvecycle
        //
        if (mainlooppos == ihalvecycle)
        {
          if (i <= 99)
          {
            if (i != 0)
            {
              x = x0 + di * (x1 - x0);
              if ((double)(x) == (double)(1.0))
              {
                x = 1.0 - machineepsilon;
              }
              if ((double)(x) == (double)(0.0))
              {
                di = 0.5;
                x = x0 + di * (x1 - x0);
                if ((double)(x) == (double)(0.0))
                {
                  break;
                }
              }
              yyy = incompletebeta(aaa, bbb, x);
              yp = (x1 - x0) / (x1 + x0);
              if ((double)(Math.Abs(yp)) < (double)(dithresh))
              {
                mainlooppos = newt;
                continue;
              }
              yp = (yyy - y0) / y0;
              if ((double)(Math.Abs(yp)) < (double)(dithresh))
              {
                mainlooppos = newt;
                continue;
              }
            }
            if ((double)(yyy) < (double)(y0))
            {
              x0 = x;
              yl = yyy;
              if (dir < 0)
              {
                dir = 0;
                di = 0.5;
              }
              else
              {
                if (dir > 3)
                {
                  di = 1.0 - (1.0 - di) * (1.0 - di);
                }
                else
                {
                  if (dir > 1)
                  {
                    di = 0.5 * di + 0.5;
                  }
                  else
                  {
                    di = (y0 - yyy) / (yh - yl);
                  }
                }
              }
              dir = dir + 1;
              if ((double)(x0) > (double)(0.75))
              {
                if (rflg == 1)
                {
                  rflg = 0;
                  aaa = a;
                  bbb = b;
                  y0 = y;
                }
                else
                {
                  rflg = 1;
                  aaa = b;
                  bbb = a;
                  y0 = 1.0 - y;
                }
                x = 1.0 - x;
                yyy = incompletebeta(aaa, bbb, x);
                x0 = 0.0;
                yl = 0.0;
                x1 = 1.0;
                yh = 1.0;
                mainlooppos = ihalve;
                continue;
              }
            }
            else
            {
              x1 = x;
              if (rflg == 1 && (double)(x1) < (double)(machineepsilon))
              {
                x = 0.0;
                break;
              }
              yh = yyy;
              if (dir > 0)
              {
                dir = 0;
                di = 0.5;
              }
              else
              {
                if (dir < -3)
                {
                  di = di * di;
                }
                else
                {
                  if (dir < -1)
                  {
                    di = 0.5 * di;
                  }
                  else
                  {
                    di = (yyy - y0) / (yh - yl);
                  }
                }
              }
              dir = dir - 1;
            }
            i = i + 1;
            mainlooppos = ihalvecycle;
            continue;
          }
          else
          {
            mainlooppos = breakihalvecycle;
            continue;
          }
        }

        //
        // breakihalvecycle
        //
        if (mainlooppos == breakihalvecycle)
        {
          if ((double)(x0) >= (double)(1.0))
          {
            x = 1.0 - machineepsilon;
            break;
          }
          if ((double)(x) <= (double)(0.0))
          {
            x = 0.0;
            break;
          }
          mainlooppos = newt;
          continue;
        }

        //
        // newt
        //
        if (mainlooppos == newt)
        {
          if (nflg != 0)
          {
            break;
          }
          nflg = 1;
          lgm = lngamma(aaa + bbb, ref s) - lngamma(aaa, ref s) - lngamma(bbb, ref s);
          i = 0;
          mainlooppos = newtcycle;
          continue;
        }

        //
        // newtcycle
        //
        if (mainlooppos == newtcycle)
        {
          if (i <= 7)
          {
            if (i != 0)
            {
              yyy = incompletebeta(aaa, bbb, x);
            }
            if ((double)(yyy) < (double)(yl))
            {
              x = x0;
              yyy = yl;
            }
            else
            {
              if ((double)(yyy) > (double)(yh))
              {
                x = x1;
                yyy = yh;
              }
              else
              {
                if ((double)(yyy) < (double)(y0))
                {
                  x0 = x;
                  yl = yyy;
                }
                else
                {
                  x1 = x;
                  yh = yyy;
                }
              }
            }
            if ((double)(x) == (double)(1.0) || (double)(x) == (double)(0.0))
            {
              mainlooppos = breaknewtcycle;
              continue;
            }
            d = (aaa - 1.0) * Math.Log(x) + (bbb - 1.0) * Math.Log(1.0 - x) + lgm;
            if ((double)(d) < (double)(Math.Log(minrealnumber)))
            {
              break;
            }
            if ((double)(d) > (double)(Math.Log(maxrealnumber)))
            {
              mainlooppos = breaknewtcycle;
              continue;
            }
            d = Math.Exp(d);
            d = (yyy - y0) / d;
            xt = x - d;
            if ((double)(xt) <= (double)(x0))
            {
              yyy = (x - x0) / (x1 - x0);
              xt = x0 + 0.5 * yyy * (x - x0);
              if ((double)(xt) <= (double)(0.0))
              {
                mainlooppos = breaknewtcycle;
                continue;
              }
            }
            if ((double)(xt) >= (double)(x1))
            {
              yyy = (x1 - x) / (x1 - x0);
              xt = x1 - 0.5 * yyy * (x1 - x);
              if ((double)(xt) >= (double)(1.0))
              {
                mainlooppos = breaknewtcycle;
                continue;
              }
            }
            x = xt;
            if ((double)(Math.Abs(d / x)) < (double)(128.0 * machineepsilon))
            {
              break;
            }
            i = i + 1;
            mainlooppos = newtcycle;
            continue;
          }
          else
          {
            mainlooppos = breaknewtcycle;
            continue;
          }
        }

        //
        // breaknewtcycle
        //
        if (mainlooppos == breaknewtcycle)
        {
          dithresh = 256.0 * machineepsilon;
          mainlooppos = ihalve;
          continue;
        }
      }

      //
      // done
      //
      if (rflg != 0)
      {
        if ((double)(x) <= (double)(machineepsilon))
        {
          x = 1.0 - machineepsilon;
        }
        else
        {
          x = 1.0 - x;
        }
      }
      result = x;
      return result;
    }


    /*************************************************************************
    Continued fraction expansion #1 for incomplete beta integral

    Cephes Math Library, Release 2.8:  June, 2000
    Copyright 1984, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    private static double incompletebetafe(double a,
        double b,
        double x,
        double big,
        double biginv)
    {
      double result = 0;
      double xk = 0;
      double pk = 0;
      double pkm1 = 0;
      double pkm2 = 0;
      double qk = 0;
      double qkm1 = 0;
      double qkm2 = 0;
      double k1 = 0;
      double k2 = 0;
      double k3 = 0;
      double k4 = 0;
      double k5 = 0;
      double k6 = 0;
      double k7 = 0;
      double k8 = 0;
      double r = 0;
      double t = 0;
      double ans = 0;
      double thresh = 0;
      int n = 0;

      k1 = a;
      k2 = a + b;
      k3 = a;
      k4 = a + 1.0;
      k5 = 1.0;
      k6 = b - 1.0;
      k7 = k4;
      k8 = a + 2.0;
      pkm2 = 0.0;
      qkm2 = 1.0;
      pkm1 = 1.0;
      qkm1 = 1.0;
      ans = 1.0;
      r = 1.0;
      n = 0;
      thresh = 3.0 * machineepsilon;
      do
      {
        xk = -(x * k1 * k2 / (k3 * k4));
        pk = pkm1 + pkm2 * xk;
        qk = qkm1 + qkm2 * xk;
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        xk = x * k5 * k6 / (k7 * k8);
        pk = pkm1 + pkm2 * xk;
        qk = qkm1 + qkm2 * xk;
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        if ((double)(qk) != (double)(0))
        {
          r = pk / qk;
        }
        if ((double)(r) != (double)(0))
        {
          t = Math.Abs((ans - r) / r);
          ans = r;
        }
        else
        {
          t = 1.0;
        }
        if ((double)(t) < (double)(thresh))
        {
          break;
        }
        k1 = k1 + 1.0;
        k2 = k2 + 1.0;
        k3 = k3 + 2.0;
        k4 = k4 + 2.0;
        k5 = k5 + 1.0;
        k6 = k6 - 1.0;
        k7 = k7 + 2.0;
        k8 = k8 + 2.0;
        if ((double)(Math.Abs(qk) + Math.Abs(pk)) > (double)(big))
        {
          pkm2 = pkm2 * biginv;
          pkm1 = pkm1 * biginv;
          qkm2 = qkm2 * biginv;
          qkm1 = qkm1 * biginv;
        }
        if ((double)(Math.Abs(qk)) < (double)(biginv) || (double)(Math.Abs(pk)) < (double)(biginv))
        {
          pkm2 = pkm2 * big;
          pkm1 = pkm1 * big;
          qkm2 = qkm2 * big;
          qkm1 = qkm1 * big;
        }
        n = n + 1;
      }
      while (n != 300);
      result = ans;
      return result;
    }


    /*************************************************************************
    Continued fraction expansion #2
    for incomplete beta integral

    Cephes Math Library, Release 2.8:  June, 2000
    Copyright 1984, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    private static double incompletebetafe2(double a,
        double b,
        double x,
        double big,
        double biginv)
    {
      double result = 0;
      double xk = 0;
      double pk = 0;
      double pkm1 = 0;
      double pkm2 = 0;
      double qk = 0;
      double qkm1 = 0;
      double qkm2 = 0;
      double k1 = 0;
      double k2 = 0;
      double k3 = 0;
      double k4 = 0;
      double k5 = 0;
      double k6 = 0;
      double k7 = 0;
      double k8 = 0;
      double r = 0;
      double t = 0;
      double ans = 0;
      double z = 0;
      double thresh = 0;
      int n = 0;

      k1 = a;
      k2 = b - 1.0;
      k3 = a;
      k4 = a + 1.0;
      k5 = 1.0;
      k6 = a + b;
      k7 = a + 1.0;
      k8 = a + 2.0;
      pkm2 = 0.0;
      qkm2 = 1.0;
      pkm1 = 1.0;
      qkm1 = 1.0;
      z = x / (1.0 - x);
      ans = 1.0;
      r = 1.0;
      n = 0;
      thresh = 3.0 * machineepsilon;
      do
      {
        xk = -(z * k1 * k2 / (k3 * k4));
        pk = pkm1 + pkm2 * xk;
        qk = qkm1 + qkm2 * xk;
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        xk = z * k5 * k6 / (k7 * k8);
        pk = pkm1 + pkm2 * xk;
        qk = qkm1 + qkm2 * xk;
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        if ((double)(qk) != (double)(0))
        {
          r = pk / qk;
        }
        if ((double)(r) != (double)(0))
        {
          t = Math.Abs((ans - r) / r);
          ans = r;
        }
        else
        {
          t = 1.0;
        }
        if ((double)(t) < (double)(thresh))
        {
          break;
        }
        k1 = k1 + 1.0;
        k2 = k2 - 1.0;
        k3 = k3 + 2.0;
        k4 = k4 + 2.0;
        k5 = k5 + 1.0;
        k6 = k6 + 1.0;
        k7 = k7 + 2.0;
        k8 = k8 + 2.0;
        if ((double)(Math.Abs(qk) + Math.Abs(pk)) > (double)(big))
        {
          pkm2 = pkm2 * biginv;
          pkm1 = pkm1 * biginv;
          qkm2 = qkm2 * biginv;
          qkm1 = qkm1 * biginv;
        }
        if ((double)(Math.Abs(qk)) < (double)(biginv) || (double)(Math.Abs(pk)) < (double)(biginv))
        {
          pkm2 = pkm2 * big;
          pkm1 = pkm1 * big;
          qkm2 = qkm2 * big;
          qkm1 = qkm1 * big;
        }
        n = n + 1;
      }
      while (n != 300);
      result = ans;
      return result;
    }


    /*************************************************************************
    Power series for incomplete beta integral.
    Use when b*x is small and x not too close to 1.

    Cephes Math Library, Release 2.8:  June, 2000
    Copyright 1984, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    private static double incompletebetaps(double a,
        double b,
        double x,
        double maxgam)
    {
      double result = 0;
      double s = 0;
      double t = 0;
      double u = 0;
      double v = 0;
      double n = 0;
      double t1 = 0;
      double z = 0;
      double ai = 0;
      double sg = 0;

      ai = 1.0 / a;
      u = (1.0 - b) * x;
      v = u / (a + 1.0);
      t1 = v;
      t = u;
      n = 2.0;
      s = 0.0;
      z = machineepsilon * ai;
      while ((double)(Math.Abs(v)) > (double)(z))
      {
        u = (n - b) * x / n;
        t = t * u;
        v = t / (a + n);
        s = s + v;
        n = n + 1.0;
      }
      s = s + t1;
      s = s + ai;
      u = a * Math.Log(x);
      if ((double)(a + b) < (double)(maxgam) && (double)(Math.Abs(u)) < (double)(Math.Log(maxrealnumber)))
      {
        t = gammafunction(a + b) / (gammafunction(a) * gammafunction(b));
        s = s * t * Math.Pow(x, a);
      }
      else
      {
        t = lngamma(a + b, ref sg) - lngamma(a, ref sg) - lngamma(b, ref sg) + u + Math.Log(s);
        if ((double)(t) < (double)(Math.Log(minrealnumber)))
        {
          s = 0.0;
        }
        else
        {
          s = Math.Exp(t);
        }
      }
      result = s;
      return result;
    }

    /*************************************************************************
    Gamma function

    Input parameters:
        X   -   argument

    Domain:
        0 < X < 171.6
        -170 < X < 0, X is not an integer.

    Relative error:
     arithmetic   domain     # trials      peak         rms
        IEEE    -170,-33      20000       2.3e-15     3.3e-16
        IEEE     -33,  33     20000       9.4e-16     2.2e-16
        IEEE      33, 171.6   20000       2.3e-15     3.2e-16

    Cephes Math Library Release 2.8:  June, 2000
    Original copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
    Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
    *************************************************************************/
    public static double gammafunction(double x)
    {
      double result = 0;
      double p = 0;
      double pp = 0;
      double q = 0;
      double qq = 0;
      double z = 0;
      int i = 0;
      double sgngam = 0;

      sgngam = 1;
      q = Math.Abs(x);
      if ((double)(q) > (double)(33.0))
      {
        if ((double)(x) < (double)(0.0))
        {
          p = (int)Math.Floor(q);
          i = (int)Math.Round(p);
          if (i % 2 == 0)
          {
            sgngam = -1;
          }
          z = q - p;
          if ((double)(z) > (double)(0.5))
          {
            p = p + 1;
            z = q - p;
          }
          z = q * Math.Sin(Math.PI * z);
          z = Math.Abs(z);
          z = Math.PI / (z * gammastirf(q));
        }
        else
        {
          z = gammastirf(x);
        }
        result = sgngam * z;
        return result;
      }
      z = 1;
      while ((double)(x) >= (double)(3))
      {
        x = x - 1;
        z = z * x;
      }
      while ((double)(x) < (double)(0))
      {
        if ((double)(x) > (double)(-0.000000001))
        {
          result = z / ((1 + 0.5772156649015329 * x) * x);
          return result;
        }
        z = z / x;
        x = x + 1;
      }
      while ((double)(x) < (double)(2))
      {
        if ((double)(x) < (double)(0.000000001))
        {
          result = z / ((1 + 0.5772156649015329 * x) * x);
          return result;
        }
        z = z / x;
        x = x + 1.0;
      }
      if ((double)(x) == (double)(2))
      {
        result = z;
        return result;
      }
      x = x - 2.0;
      pp = 1.60119522476751861407E-4;
      pp = 1.19135147006586384913E-3 + x * pp;
      pp = 1.04213797561761569935E-2 + x * pp;
      pp = 4.76367800457137231464E-2 + x * pp;
      pp = 2.07448227648435975150E-1 + x * pp;
      pp = 4.94214826801497100753E-1 + x * pp;
      pp = 9.99999999999999996796E-1 + x * pp;
      qq = -2.31581873324120129819E-5;
      qq = 5.39605580493303397842E-4 + x * qq;
      qq = -4.45641913851797240494E-3 + x * qq;
      qq = 1.18139785222060435552E-2 + x * qq;
      qq = 3.58236398605498653373E-2 + x * qq;
      qq = -2.34591795718243348568E-1 + x * qq;
      qq = 7.14304917030273074085E-2 + x * qq;
      qq = 1.00000000000000000320 + x * qq;
      result = z * pp / qq;
      return result;
    }


    /*************************************************************************
    Natural logarithm of gamma function

    Input parameters:
        X       -   argument

    Result:
        logarithm of the absolute value of the Gamma(X).

    Output parameters:
        SgnGam  -   sign(Gamma(X))

    Domain:
        0 < X < 2.55e305
        -2.55e305 < X < 0, X is not an integer.

    ACCURACY:
    arithmetic      domain        # trials     peak         rms
       IEEE    0, 3                 28000     5.4e-16     1.1e-16
       IEEE    2.718, 2.556e305     40000     3.5e-16     8.3e-17
    The error criterion was relative when the function magnitude
    was greater than one but absolute when it was less than one.

    The following test used the relative error criterion, though
    at certain points the relative error could be much higher than
    indicated.
       IEEE    -200, -4             10000     4.8e-16     1.3e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1989, 1992, 2000 by Stephen L. Moshier
    Translated to AlgoPascal by Bochkanov Sergey (2005, 2006, 2007).
    *************************************************************************/
    public static double lngamma(double x, ref double sgngam)
    {
      double result = 0;
      double a = 0;
      double b = 0;
      double c = 0;
      double p = 0;
      double q = 0;
      double u = 0;
      double w = 0;
      double z = 0;
      int i = 0;
      double logpi = 0;
      double ls2pi = 0;
      double tmp = 0;

      sgngam = 0;

      sgngam = 1;
      logpi = 1.14472988584940017414;
      ls2pi = 0.91893853320467274178;
      if ((double)(x) < (double)(-34.0))
      {
        q = -x;
        w = lngamma(q, ref tmp);
        p = (int)Math.Floor(q);
        i = (int)Math.Round(p);
        if (i % 2 == 0)
        {
          sgngam = -1;
        }
        else
        {
          sgngam = 1;
        }
        z = q - p;
        if ((double)(z) > (double)(0.5))
        {
          p = p + 1;
          z = p - q;
        }
        z = q * Math.Sin(Math.PI * z);
        result = logpi - Math.Log(z) - w;
        return result;
      }
      if ((double)(x) < (double)(13))
      {
        z = 1;
        p = 0;
        u = x;
        while ((double)(u) >= (double)(3))
        {
          p = p - 1;
          u = x + p;
          z = z * u;
        }
        while ((double)(u) < (double)(2))
        {
          z = z / u;
          p = p + 1;
          u = x + p;
        }
        if ((double)(z) < (double)(0))
        {
          sgngam = -1;
          z = -z;
        }
        else
        {
          sgngam = 1;
        }
        if ((double)(u) == (double)(2))
        {
          result = Math.Log(z);
          return result;
        }
        p = p - 2;
        x = x + p;
        b = -1378.25152569120859100;
        b = -38801.6315134637840924 + x * b;
        b = -331612.992738871184744 + x * b;
        b = -1162370.97492762307383 + x * b;
        b = -1721737.00820839662146 + x * b;
        b = -853555.664245765465627 + x * b;
        c = 1;
        c = -351.815701436523470549 + x * c;
        c = -17064.2106651881159223 + x * c;
        c = -220528.590553854454839 + x * c;
        c = -1139334.44367982507207 + x * c;
        c = -2532523.07177582951285 + x * c;
        c = -2018891.41433532773231 + x * c;
        p = x * b / c;
        result = Math.Log(z) + p;
        return result;
      }
      q = (x - 0.5) * Math.Log(x) - x + ls2pi;
      if ((double)(x) > (double)(100000000))
      {
        result = q;
        return result;
      }
      p = 1 / (x * x);
      if ((double)(x) >= (double)(1000.0))
      {
        q = q + ((7.9365079365079365079365 * 0.0001 * p - 2.7777777777777777777778 * 0.001) * p + 0.0833333333333333333333) / x;
      }
      else
      {
        a = 8.11614167470508450300 * 0.0001;
        a = -(5.95061904284301438324 * 0.0001) + p * a;
        a = 7.93650340457716943945 * 0.0001 + p * a;
        a = -(2.77777777730099687205 * 0.001) + p * a;
        a = 8.33333333333331927722 * 0.01 + p * a;
        q = q + a / x;
      }
      result = q;
      return result;
    }


    private static double gammastirf(double x)
    {
      double result = 0;
      double y = 0;
      double w = 0;
      double v = 0;
      double stir = 0;

      w = 1 / x;
      stir = 7.87311395793093628397E-4;
      stir = -2.29549961613378126380E-4 + w * stir;
      stir = -2.68132617805781232825E-3 + w * stir;
      stir = 3.47222221605458667310E-3 + w * stir;
      stir = 8.33333333333482257126E-2 + w * stir;
      w = 1 + w * stir;
      y = Math.Exp(x);
      if ((double)(x) > (double)(143.01608))
      {
        v = Math.Pow(x, 0.5 * x - 0.25);
        y = v * (v / y);
      }
      else
      {
        y = Math.Pow(x, x - 0.5) / y;
      }
      result = 2.50662827463100050242 * y * w;
      return result;
    }

    /*************************************************************************
    Incomplete gamma integral

    The function is defined by

                              x
                               -
                      1       | |  -t  a-1
     igam(a,x)  =   -----     |   e   t   dt.
                     -      | |
                    | (a)    -
                              0


    In this implementation both arguments must be positive.
    The integral is evaluated by either a power series or
    continued fraction expansion, depending on the relative
    values of a and x.

    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0,30       200000       3.6e-14     2.9e-15
       IEEE      0,100      300000       9.9e-14     1.5e-14

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1985, 1987, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double incompletegamma(double a,
        double x)
    {
      double result = 0;
      double igammaepsilon = 0;
      double ans = 0;
      double ax = 0;
      double c = 0;
      double r = 0;
      double tmp = 0;

      igammaepsilon = 0.000000000000001;
      if ((double)(x) <= (double)(0) || (double)(a) <= (double)(0))
      {
        result = 0;
        return result;
      }
      if ((double)(x) > (double)(1) && (double)(x) > (double)(a))
      {
        result = 1 - incompletegammac(a, x);
        return result;
      }
      ax = a * Math.Log(x) - x - lngamma(a, ref tmp);
      if ((double)(ax) < (double)(-709.78271289338399))
      {
        result = 0;
        return result;
      }
      ax = Math.Exp(ax);
      r = a;
      c = 1;
      ans = 1;
      do
      {
        r = r + 1;
        c = c * x / r;
        ans = ans + c;
      }
      while ((double)(c / ans) > (double)(igammaepsilon));
      result = ans * ax / a;
      return result;
    }


    /*************************************************************************
    Complemented incomplete gamma integral

    The function is defined by


     igamc(a,x)   =   1 - igam(a,x)

                               inf.
                                 -
                        1       | |  -t  a-1
                  =   -----     |   e   t   dt.
                       -      | |
                      | (a)    -
                                x


    In this implementation both arguments must be positive.
    The integral is evaluated by either a power series or
    continued fraction expansion, depending on the relative
    values of a and x.

    ACCURACY:

    Tested at random a, x.
                   a         x                      Relative error:
    arithmetic   domain   domain     # trials      peak         rms
       IEEE     0.5,100   0,100      200000       1.9e-14     1.7e-15
       IEEE     0.01,0.5  0,100      200000       1.4e-13     1.6e-15

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1985, 1987, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double incompletegammac(double a, double x)
    {
      double result = 0;
      double igammaepsilon = 0;
      double igammabignumber = 0;
      double igammabignumberinv = 0;
      double ans = 0;
      double ax = 0;
      double c = 0;
      double yc = 0;
      double r = 0;
      double t = 0;
      double y = 0;
      double z = 0;
      double pk = 0;
      double pkm1 = 0;
      double pkm2 = 0;
      double qk = 0;
      double qkm1 = 0;
      double qkm2 = 0;
      double tmp = 0;

      igammaepsilon = 0.000000000000001;
      igammabignumber = 4503599627370496.0;
      igammabignumberinv = 2.22044604925031308085 * 0.0000000000000001;
      if ((double)(x) <= (double)(0) || (double)(a) <= (double)(0))
      {
        result = 1;
        return result;
      }
      if ((double)(x) < (double)(1) || (double)(x) < (double)(a))
      {
        result = 1 - incompletegamma(a, x);
        return result;
      }
      ax = a * Math.Log(x) - x - lngamma(a, ref tmp);
      if ((double)(ax) < (double)(-709.78271289338399))
      {
        result = 0;
        return result;
      }
      ax = Math.Exp(ax);
      y = 1 - a;
      z = x + y + 1;
      c = 0;
      pkm2 = 1;
      qkm2 = x;
      pkm1 = x + 1;
      qkm1 = z * x;
      ans = pkm1 / qkm1;
      do
      {
        c = c + 1;
        y = y + 1;
        z = z + 2;
        yc = y * c;
        pk = pkm1 * z - pkm2 * yc;
        qk = qkm1 * z - qkm2 * yc;
        if ((double)(qk) != (double)(0))
        {
          r = pk / qk;
          t = Math.Abs((ans - r) / r);
          ans = r;
        }
        else
        {
          t = 1;
        }
        pkm2 = pkm1;
        pkm1 = pk;
        qkm2 = qkm1;
        qkm1 = qk;
        if ((double)(Math.Abs(pk)) > (double)(igammabignumber))
        {
          pkm2 = pkm2 * igammabignumberinv;
          pkm1 = pkm1 * igammabignumberinv;
          qkm2 = qkm2 * igammabignumberinv;
          qkm1 = qkm1 * igammabignumberinv;
        }
      }
      while ((double)(t) > (double)(igammaepsilon));
      result = ans * ax;
      return result;
    }


    /*************************************************************************
    Inverse of complemented imcomplete gamma integral

    Given p, the function finds x such that

     igamc( a, x ) = p.

    Starting with the approximate value

            3
     x = a t

     where

     t = 1 - d - ndtri(p) sqrt(d)

    and

     d = 1/9a,

    the routine performs up to 10 Newton iterations to find the
    root of igamc(a,x) - p = 0.

    ACCURACY:

    Tested at random a, p in the intervals indicated.

                   a        p                      Relative error:
    arithmetic   domain   domain     # trials      peak         rms
       IEEE     0.5,100   0,0.5       100000       1.0e-14     1.7e-15
       IEEE     0.01,0.5  0,0.5       100000       9.0e-14     3.4e-15
       IEEE    0.5,10000  0,0.5        20000       2.3e-13     3.8e-14

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invincompletegammac(double a, double y0)
    {
      double result = 0;
      double igammaepsilon = 0;
      double iinvgammabignumber = 0;
      double x0 = 0;
      double x1 = 0;
      double x = 0;
      double yl = 0;
      double yh = 0;
      double y = 0;
      double d = 0;
      double lgm = 0;
      double dithresh = 0;
      int i = 0;
      int dir = 0;
      double tmp = 0;

      igammaepsilon = 0.000000000000001;
      iinvgammabignumber = 4503599627370496.0;
      x0 = iinvgammabignumber;
      yl = 0;
      x1 = 0;
      yh = 1;
      dithresh = 5 * igammaepsilon;
      d = 1 / (9 * a);
      y = 1 - d - invnormaldistribution(y0) * Math.Sqrt(d);
      x = a * y * y * y;
      lgm = lngamma(a, ref tmp);
      i = 0;
      while (i < 10)
      {
        if ((double)(x) > (double)(x0) || (double)(x) < (double)(x1))
        {
          d = 0.0625;
          break;
        }
        y = incompletegammac(a, x);
        if ((double)(y) < (double)(yl) || (double)(y) > (double)(yh))
        {
          d = 0.0625;
          break;
        }
        if ((double)(y) < (double)(y0))
        {
          x0 = x;
          yl = y;
        }
        else
        {
          x1 = x;
          yh = y;
        }
        d = (a - 1) * Math.Log(x) - x - lgm;
        if ((double)(d) < (double)(-709.78271289338399))
        {
          d = 0.0625;
          break;
        }
        d = -Math.Exp(d);
        d = (y - y0) / d;
        if ((double)(Math.Abs(d / x)) < (double)(igammaepsilon))
        {
          result = x;
          return result;
        }
        x = x - d;
        i = i + 1;
      }
      if ((double)(x0) == (double)(iinvgammabignumber))
      {
        if ((double)(x) <= (double)(0))
        {
          x = 1;
        }
        while ((double)(x0) == (double)(iinvgammabignumber))
        {
          x = (1 + d) * x;
          y = incompletegammac(a, x);
          if ((double)(y) < (double)(y0))
          {
            x0 = x;
            yl = y;
            break;
          }
          d = d + d;
        }
      }
      d = 0.5;
      dir = 0;
      i = 0;
      while (i < 400)
      {
        x = x1 + d * (x0 - x1);
        y = incompletegammac(a, x);
        lgm = (x0 - x1) / (x1 + x0);
        if ((double)(Math.Abs(lgm)) < (double)(dithresh))
        {
          break;
        }
        lgm = (y - y0) / y0;
        if ((double)(Math.Abs(lgm)) < (double)(dithresh))
        {
          break;
        }
        if ((double)(x) <= (double)(0.0))
        {
          break;
        }
        if ((double)(y) >= (double)(y0))
        {
          x1 = x;
          yh = y;
          if (dir < 0)
          {
            dir = 0;
            d = 0.5;
          }
          else
          {
            if (dir > 1)
            {
              d = 0.5 * d + 0.5;
            }
            else
            {
              d = (y0 - yl) / (yh - yl);
            }
          }
          dir = dir + 1;
        }
        else
        {
          x0 = x;
          yl = y;
          if (dir > 0)
          {
            dir = 0;
            d = 0.5;
          }
          else
          {
            if (dir < -1)
            {
              d = 0.5 * d;
            }
            else
            {
              d = (y0 - yl) / (yh - yl);
            }
          }
          dir = dir - 1;
        }
        i = i + 1;
      }
      result = x;
      return result;
    }


    /*************************************************************************
    Error function

    The integral is

                              x
                               -
                    2         | |          2
      erf(x)  =  --------     |    exp( - t  ) dt.
                 sqrt(pi)   | |
                             -
                              0

    For 0 <= |x| < 1, erf(x) = x * P4(x**2)/Q5(x**2); otherwise
    erf(x) = 1 - erfc(x).


    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0,1         30000       3.7e-16     1.0e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double errorfunction(double x)
    {
      double result = 0;
      double xsq = 0;
      double s = 0;
      double p = 0;
      double q = 0;

      s = Math.Sign(x);
      x = Math.Abs(x);
      if ((double)(x) < (double)(0.5))
      {
        xsq = x * x;
        p = 0.007547728033418631287834;
        p = -0.288805137207594084924010 + xsq * p;
        p = 14.3383842191748205576712 + xsq * p;
        p = 38.0140318123903008244444 + xsq * p;
        p = 3017.82788536507577809226 + xsq * p;
        p = 7404.07142710151470082064 + xsq * p;
        p = 80437.3630960840172832162 + xsq * p;
        q = 0.0;
        q = 1.00000000000000000000000 + xsq * q;
        q = 38.0190713951939403753468 + xsq * q;
        q = 658.070155459240506326937 + xsq * q;
        q = 6379.60017324428279487120 + xsq * q;
        q = 34216.5257924628539769006 + xsq * q;
        q = 80437.3630960840172826266 + xsq * q;
        result = s * 1.1283791670955125738961589031 * x * p / q;
        return result;
      }
      if ((double)(x) >= (double)(10))
      {
        result = s;
        return result;
      }
      result = s * (1 - errorfunctionc(x));
      return result;
    }


    /*************************************************************************
    Complementary error function

     1 - erf(x) =

                              inf.
                                -
                     2         | |          2
      erfc(x)  =  --------     |    exp( - t  ) dt
                  sqrt(pi)   | |
                              -
                               x


    For small x, erfc(x) = 1 - erf(x); otherwise rational
    approximations are computed.


    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0,26.6417   30000       5.7e-14     1.5e-14

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double errorfunctionc(double x)
    {
      double result = 0;
      double p = 0;
      double q = 0;

      if ((double)(x) < (double)(0))
      {
        result = 2 - errorfunctionc(-x);
        return result;
      }
      if ((double)(x) < (double)(0.5))
      {
        result = 1.0 - errorfunction(x);
        return result;
      }
      if ((double)(x) >= (double)(10))
      {
        result = 0;
        return result;
      }
      p = 0.0;
      p = 0.5641877825507397413087057563 + x * p;
      p = 9.675807882987265400604202961 + x * p;
      p = 77.08161730368428609781633646 + x * p;
      p = 368.5196154710010637133875746 + x * p;
      p = 1143.262070703886173606073338 + x * p;
      p = 2320.439590251635247384768711 + x * p;
      p = 2898.0293292167655611275846 + x * p;
      p = 1826.3348842295112592168999 + x * p;
      q = 1.0;
      q = 17.14980943627607849376131193 + x * q;
      q = 137.1255960500622202878443578 + x * q;
      q = 661.7361207107653469211984771 + x * q;
      q = 2094.384367789539593790281779 + x * q;
      q = 4429.612803883682726711528526 + x * q;
      q = 6089.5424232724435504633068 + x * q;
      q = 4958.82756472114071495438422 + x * q;
      q = 1826.3348842295112595576438 + x * q;
      result = Math.Exp(-sqr(x)) * p / q;
      return result;
    }


    /*************************************************************************
    Same as normalcdf(), obsolete name.
    *************************************************************************/
    public static double normaldistribution(double x)
    {
      double result = 0;

      result = 0.5 * (errorfunction(x / 1.41421356237309504880) + 1);
      return result;
    }


    /*************************************************************************
    Normal distribution PDF

    Returns Gaussian probability density function:

                   1
       f(x)  = --------- * exp(-x^2/2)
               sqrt(2pi)

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double normalpdf(double x)
    {
      double result = 0;

      Debug.Assert(isfinite(x), "NormalPDF: X is infinite");
      result = Math.Exp(-(x * x / 2)) / Math.Sqrt(2 * Math.PI);
      return result;
    }


    /*************************************************************************
    Normal distribution CDF

    Returns the area under the Gaussian probability density
    function, integrated from minus infinity to x:

                               x
                                -
                      1        | |          2
       ndtr(x)  = ---------    |    exp( - t /2 ) dt
                  sqrt(2pi)  | |
                              -
                             -inf.

                =  ( 1 + erf(z) ) / 2
                =  erfc(z) / 2

    where z = x/sqrt(2). Computation is via the functions
    erf and erfc.


    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE     -13,0        30000       3.4e-14     6.7e-15

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double normalcdf(double x)
    {
      double result = 0;

      result = 0.5 * (errorfunction(x / 1.41421356237309504880) + 1);
      return result;
    }


    /*************************************************************************
    Inverse of the error function

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double inverf(double e)
    {
      double result = 0;

      result = invnormaldistribution(0.5 * (e + 1)) / Math.Sqrt(2);
      return result;
    }


    /*************************************************************************
    Same as invnormalcdf(), deprecated name
    *************************************************************************/
    public static double invnormaldistribution(double y0)
    {
      double result = 0;

      result = invnormalcdf(y0);
      return result;
    }


    /*************************************************************************
    Inverse of Normal CDF

    Returns the argument, x, for which the area under the
    Gaussian probability density function (integrated from
    minus infinity to x) is equal to y.


    For small arguments 0 < y < exp(-2), the program computes
    z = sqrt( -2.0 * log(y) );  then the approximation is
    x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z).
    There are two rational functions P/Q, one for 0 < y < exp(-32)
    and the other for y up to exp(-2).  For larger arguments,
    w = y - 0.5, and  x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2)).

    ACCURACY:

                         Relative error:
    arithmetic   domain        # trials      peak         rms
       IEEE     0.125, 1        20000       7.2e-16     1.3e-16
       IEEE     3e-308, 0.135   50000       4.6e-16     9.8e-17

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invnormalcdf(double y0)
    {
      double result = 0;
      double expm2 = 0;
      double s2pi = 0;
      double x = 0;
      double y = 0;
      double z = 0;
      double y2 = 0;
      double x0 = 0;
      double x1 = 0;
      int code = 0;
      double p0 = 0;
      double q0 = 0;
      double p1 = 0;
      double q1 = 0;
      double p2 = 0;
      double q2 = 0;

      expm2 = 0.13533528323661269189;
      s2pi = 2.50662827463100050242;
      if ((double)(y0) <= (double)(0))
      {
        result = -maxrealnumber;
        return result;
      }
      if ((double)(y0) >= (double)(1))
      {
        result = maxrealnumber;
        return result;
      }
      code = 1;
      y = y0;
      if ((double)(y) > (double)(1.0 - expm2))
      {
        y = 1.0 - y;
        code = 0;
      }
      if ((double)(y) > (double)(expm2))
      {
        y = y - 0.5;
        y2 = y * y;
        p0 = -59.9633501014107895267;
        p0 = 98.0010754185999661536 + y2 * p0;
        p0 = -56.6762857469070293439 + y2 * p0;
        p0 = 13.9312609387279679503 + y2 * p0;
        p0 = -1.23916583867381258016 + y2 * p0;
        q0 = 1;
        q0 = 1.95448858338141759834 + y2 * q0;
        q0 = 4.67627912898881538453 + y2 * q0;
        q0 = 86.3602421390890590575 + y2 * q0;
        q0 = -225.462687854119370527 + y2 * q0;
        q0 = 200.260212380060660359 + y2 * q0;
        q0 = -82.0372256168333339912 + y2 * q0;
        q0 = 15.9056225126211695515 + y2 * q0;
        q0 = -1.18331621121330003142 + y2 * q0;
        x = y + y * y2 * p0 / q0;
        x = x * s2pi;
        result = x;
        return result;
      }
      x = Math.Sqrt(-(2.0 * Math.Log(y)));
      x0 = x - Math.Log(x) / x;
      z = 1.0 / x;
      if ((double)(x) < (double)(8.0))
      {
        p1 = 4.05544892305962419923;
        p1 = 31.5251094599893866154 + z * p1;
        p1 = 57.1628192246421288162 + z * p1;
        p1 = 44.0805073893200834700 + z * p1;
        p1 = 14.6849561928858024014 + z * p1;
        p1 = 2.18663306850790267539 + z * p1;
        p1 = -(1.40256079171354495875 * 0.1) + z * p1;
        p1 = -(3.50424626827848203418 * 0.01) + z * p1;
        p1 = -(8.57456785154685413611 * 0.0001) + z * p1;
        q1 = 1;
        q1 = 15.7799883256466749731 + z * q1;
        q1 = 45.3907635128879210584 + z * q1;
        q1 = 41.3172038254672030440 + z * q1;
        q1 = 15.0425385692907503408 + z * q1;
        q1 = 2.50464946208309415979 + z * q1;
        q1 = -(1.42182922854787788574 * 0.1) + z * q1;
        q1 = -(3.80806407691578277194 * 0.01) + z * q1;
        q1 = -(9.33259480895457427372 * 0.0001) + z * q1;
        x1 = z * p1 / q1;
      }
      else
      {
        p2 = 3.23774891776946035970;
        p2 = 6.91522889068984211695 + z * p2;
        p2 = 3.93881025292474443415 + z * p2;
        p2 = 1.33303460815807542389 + z * p2;
        p2 = 2.01485389549179081538 * 0.1 + z * p2;
        p2 = 1.23716634817820021358 * 0.01 + z * p2;
        p2 = 3.01581553508235416007 * 0.0001 + z * p2;
        p2 = 2.65806974686737550832 * 0.000001 + z * p2;
        p2 = 6.23974539184983293730 * 0.000000001 + z * p2;
        q2 = 1;
        q2 = 6.02427039364742014255 + z * q2;
        q2 = 3.67983563856160859403 + z * q2;
        q2 = 1.37702099489081330271 + z * q2;
        q2 = 2.16236993594496635890 * 0.1 + z * q2;
        q2 = 1.34204006088543189037 * 0.01 + z * q2;
        q2 = 3.28014464682127739104 * 0.0001 + z * q2;
        q2 = 2.89247864745380683936 * 0.000001 + z * q2;
        q2 = 6.79019408009981274425 * 0.000000001 + z * q2;
        x1 = z * p2 / q2;
      }
      x = x0 - x1;
      if (code != 0)
      {
        x = -x;
      }
      result = x;
      return result;
    }


    /*************************************************************************
    Bivariate normal PDF

    Returns probability density function of the bivariate  Gaussian  with
    correlation parameter equal to Rho:

                             1              (    x^2 - 2*rho*x*y + y^2  )
        f(x,y,rho) = ----------------- * exp( - ----------------------- )
                     2pi*sqrt(1-rho^2)      (        2*(1-rho^2)        )


    with -1<rho<+1 and arbitrary x, y.

    This function won't fail as long as Rho is in (-1,+1) range.

      -- ALGLIB --
         Copyright 15.11.2019 by Bochkanov Sergey
    *************************************************************************/
    public static double bivariatenormalpdf(double x, double y, double rho)
    {
      double result = 0;
      double onerho2 = 0;

      Debug.Assert(isfinite(x), "BivariateNormalCDF: X is infinite");
      Debug.Assert(isfinite(y), "BivariateNormalCDF: Y is infinite");
      Debug.Assert(isfinite(rho), "BivariateNormalCDF: Rho is infinite");
      Debug.Assert((double)(-1) < (double)(rho) && (double)(rho) < (double)(1), "BivariateNormalCDF: Rho is not in (-1,+1) range");
      onerho2 = (1 - rho) * (1 + rho);
      result = Math.Exp(-((x * x + y * y - 2 * rho * x * y) / (2 * onerho2))) / (2 * Math.PI * Math.Sqrt(onerho2));
      return result;
    }


    /*************************************************************************
    Bivariate normal CDF

    Returns the area under the bivariate Gaussian  PDF  with  correlation
    parameter equal to Rho, integrated from minus infinity to (x,y):


                                              x      y
                                              -      -  
                                1            | |    | | 
        bvn(x,y,rho) = -------------------   |      |   f(u,v,rho)*du*dv
                        2pi*sqrt(1-rho^2)  | |    | |    
                                            -      -
                                           -INF   -INF

                                               
    where

                          (    u^2 - 2*rho*u*v + v^2  )
        f(u,v,rho)   = exp( - ----------------------- )
                          (        2*(1-rho^2)        )


    with -1<rho<+1 and arbitrary x, y.

    This subroutine uses high-precision approximation scheme proposed  by
    Alan Genz in "Numerical  Computation  of  Rectangular  Bivariate  and
    Trivariate Normal and  t  probabilities",  which  computes  CDF  with
    absolute error roughly equal to 1e-14.

    This function won't fail as long as Rho is in (-1,+1) range.

      -- ALGLIB --
         Copyright 15.11.2019 by Bochkanov Sergey
    *************************************************************************/
    public static double bivariatenormalcdf(double x, double y, double rho)
    {
      double result = 0;
      double rangea = 0;
      double rangeb = 0;
      double s = 0;
      double v = 0;
      double v0 = 0;
      double v1 = 0;
      double fxys = 0;
      double ta = 0;
      double tb = 0;
      double tc = 0;

      Debug.Assert(isfinite(x), "BivariateNormalCDF: X is infinite");
      Debug.Assert(isfinite(y), "BivariateNormalCDF: Y is infinite");
      Debug.Assert(isfinite(rho), "BivariateNormalCDF: Rho is infinite");
      Debug.Assert((double)(-1) < (double)(rho) && (double)(rho) < (double)(1), "BivariateNormalCDF: Rho is not in (-1,+1) range");
      if ((double)(rho) == (double)(0))
      {
        result = normalcdf(x) * normalcdf(y);
        return result;
      }
      if ((double)(Math.Abs(rho)) <= (double)(0.8))
      {

        //
        // Rho is small, compute integral using using formula (3) by Alan Genz, integrated
        // by means of 10-point Gauss-Legendre quadrature
        //
        rangea = 0;
        rangeb = Math.Asin(rho);
        v = 0;
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2491470458134028, -0.1252334085114689);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2491470458134028, 0.1252334085114689);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2334925365383548, -0.3678314989981802);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2334925365383548, 0.3678314989981802);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2031674267230659, -0.5873179542866175);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.2031674267230659, 0.5873179542866175);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.1600783285433462, -0.7699026741943047);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.1600783285433462, 0.7699026741943047);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.1069393259953184, -0.9041172563704749);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.1069393259953184, 0.9041172563704749);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.0471753363865118, -0.9815606342467192);
        v = v + bvnintegrate3(rangea, rangeb, x, y, 0.0471753363865118, 0.9815606342467192);
        v = v * 0.5 * (rangeb - rangea) / (2 * Math.PI);
        result = normalcdf(x) * normalcdf(y) + v;
      }
      else
      {

        //
        // Rho is large, compute integral using using formula (6) by Alan Genz, integrated
        // by means of 20-point Gauss-Legendre quadrature.
        //
        x = -x;
        y = -y;
        s = Math.Sign(rho);
        if ((double)(s) > (double)(0))
        {
          fxys = normalcdf(-Math.Max(x, y));
        }
        else
        {
          fxys = Math.Max(0.0, normalcdf(-x) - normalcdf(y));
        }
        rangea = 0;
        rangeb = Math.Sqrt((1 - rho) * (1 + rho));

        //
        // Compute first term (analytic integral) from formula (6)
        //
        ta = rangeb;
        tb = Math.Abs(x - s * y);
        tc = (4 - s * x * y) / 8;
        v0 = ta * (1 - tc * (tb * tb - ta * ta) / 3) * Math.Exp(-(tb * tb / (2 * ta * ta))) - tb * (1 - tc * tb * tb / 3) * Math.Sqrt(2 * Math.PI) * normalcdf(-(tb / ta));
        v0 = v0 * Math.Exp(-(s * x * y / 2)) / (2 * Math.PI);

        //
        // Compute second term (numerical integral, 20-point Gauss-Legendre rule) from formula (6)
        //
        v1 = 0;
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1527533871307258, -0.0765265211334973);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1527533871307258, 0.0765265211334973);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1491729864726037, -0.2277858511416451);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1491729864726037, 0.2277858511416451);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1420961093183820, -0.3737060887154195);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1420961093183820, 0.3737060887154195);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1316886384491766, -0.5108670019508271);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1316886384491766, 0.5108670019508271);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1181945319615184, -0.6360536807265150);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1181945319615184, 0.6360536807265150);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1019301198172404, -0.7463319064601508);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.1019301198172404, 0.7463319064601508);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0832767415767048, -0.8391169718222188);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0832767415767048, 0.8391169718222188);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0626720483341091, -0.9122344282513259);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0626720483341091, 0.9122344282513259);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0406014298003869, -0.9639719272779138);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0406014298003869, 0.9639719272779138);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0176140071391521, -0.9931285991850949);
        v1 = v1 + bvnintegrate6(rangea, rangeb, x, y, s, 0.0176140071391521, 0.9931285991850949);
        v1 = v1 * 0.5 * (rangeb - rangea) / (2 * Math.PI);
        result = fxys - s * (v0 + v1);
      }
      result = Math.Max(result, 0);
      result = Math.Min(result, 1);
      return result;
    }


    /*************************************************************************
    Internal function which computes integrand of  formula  (3)  by  Alan
    Genz times Gaussian weights (passed by user).

      -- ALGLIB --
         Copyright 15.11.2019 by Bochkanov Sergey
    *************************************************************************/
    private static double bvnintegrate3(double rangea,
        double rangeb,
        double x,
        double y,
        double gw,
        double gx)
    {
      double result = 0;
      double r = 0;
      double t2 = 0;
      double dd = 0;
      double sinr = 0;
      double cosr = 0;

      r = (rangeb - rangea) * 0.5 * gx + (rangeb + rangea) * 0.5;
      t2 = Math.Tan(0.5 * r);
      dd = 1 / (1 + t2 * t2);
      sinr = 2 * t2 * dd;
      cosr = (1 - t2 * t2) * dd;
      result = gw * Math.Exp(-((x * x + y * y - 2 * x * y * sinr) / (2 * cosr * cosr)));
      return result;
    }


    /*************************************************************************
    Internal function which computes integrand of  formula  (6)  by  Alan
    Genz times Gaussian weights (passed by user).

      -- ALGLIB --
         Copyright 15.11.2019 by Bochkanov Sergey
    *************************************************************************/
    private static double bvnintegrate6(double rangea,
        double rangeb,
        double x,
        double y,
        double s,
        double gw,
        double gx)
    {
      double result = 0;
      double r = 0;
      double exphsk22x2 = 0;
      double exphsk2 = 0;
      double sqrt1x2 = 0;
      double exphsk1sqrt1x2 = 0;

      r = (rangeb - rangea) * 0.5 * gx + (rangeb + rangea) * 0.5;
      exphsk22x2 = Math.Exp(-((x - s * y) * (x - s * y) / (2 * r * r)));
      exphsk2 = Math.Exp(-(x * s * y / 2));
      sqrt1x2 = Math.Sqrt((1 - r) * (1 + r));
      exphsk1sqrt1x2 = Math.Exp(-(x * s * y / (1 + sqrt1x2)));
      result = gw * exphsk22x2 * (exphsk1sqrt1x2 / sqrt1x2 - exphsk2 * (1 + (4 - x * y * s) * r * r / 8));
      return result;
    }
  }
}