﻿using System;

namespace Notung.Data
{
  /// <summary>
  /// Легковесный битовый массив без проверки границ
  /// </summary>
  [Serializable]
  internal struct BitArrayHelper
  {
    private const int POWER = 5; // показатель двойки для 32 - размер int в битах
    private readonly int[] m_array;

    private static int GetSize(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      if (length == 0)
        return 0;

      return ((length - 1) / (1 << POWER)) + 1;
    }

    public BitArrayHelper(int length)
    {
      m_array = new int[GetSize(length)];
    }

    public bool this[int index]
    {
      get
      {
        return (m_array[index >> POWER] & (1 << index)) != 0;
      }
      set
      {
        if (value)
          m_array[index >> POWER] |= 1 << index;
        else
          m_array[index >> POWER] &= ~(1 << index);
      }
    }
  }
}