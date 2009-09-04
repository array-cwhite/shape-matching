﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiniarAlgebra;
using System.Drawing;

namespace PCA
{
    public static class Utils
    {
        public static readonly int sr_MinRow = 0;
        public static readonly int sr_MaxRow = 1;

        public static void SubstractScalarsByDims(ref DoubleMatrix io_leftHandMatrix, DoubleMatrix i_rightHandVector)
        {
            if ((i_rightHandVector.ColumnsCount > 1) || (io_leftHandMatrix.RowsCount != i_rightHandVector.RowsCount))
            {
                throw new PCAException("Dimension are not meet for substraction of a vector from matrix by rows");
            }

            Func<int, int, double, double> substractByDim = (row, col, Val) => (Val - (double)i_rightHandVector[row, 0]);
            io_leftHandMatrix.Iterate(substractByDim);
        }

        public static void AddScalarsByDims(ref DoubleMatrix io_leftHandMatrix, DoubleMatrix i_rightHandVector)
        {
            if ((i_rightHandVector.ColumnsCount > 1) || (io_leftHandMatrix.RowsCount != i_rightHandVector.RowsCount))
            {
                throw new PCAException("Dimension are not meet for substraction of a vector from matrix by rows");
            }

            Func<int, int, double, double> substractByDim = (row, col, Val) => (Val + (double)i_rightHandVector[row, 0]);
            io_leftHandMatrix.Iterate(substractByDim);
        }

        public static DoubleMatrix ShiftToPozitives(ref DoubleMatrix io_Coordinates,params DoubleMatrix[] io_AffectedAlso)
        {
            DoubleMatrix retMinMax = MinMaxByRow(io_Coordinates);
            DoubleMatrix growByMatrix = new DoubleMatrix(retMinMax.RowsCount, 1);
            growByMatrix.Init(0);

            Func<int, int, double, double> prepareGrowingCell = (row, col, value) =>
            {
                if (retMinMax[row, sr_MinRow] < 0)
                {
                    retMinMax[row, sr_MinRow] = Math.Abs(retMinMax[row, sr_MinRow]);
                    retMinMax[row, sr_MaxRow] += retMinMax[row, sr_MinRow];
                    return retMinMax[row, sr_MinRow];
                }
                else
                {
                    retMinMax[row, sr_MinRow] = value;
                    return value;
                }
            };

            growByMatrix.Iterate(prepareGrowingCell);

            AddScalarsByDims(ref io_Coordinates, growByMatrix);

            for (int i = 0; i < io_AffectedAlso.Length; ++i)
			{
                AddScalarsByDims(ref io_AffectedAlso[i], growByMatrix);
            }

            return retMinMax;
        }

        public static DoubleMatrix MinMaxByRow(DoubleMatrix i_matrix)
        {
            DoubleMatrix retMinMaxMatrix = new DoubleMatrix(i_matrix.RowsCount, 2);

            Func<int, int, double, double> calcAVG = (rows, cols, value) =>
            {
                retMinMaxMatrix[rows, sr_MinRow] = Math.Min(retMinMaxMatrix[rows, 0], value);
                retMinMaxMatrix[rows, sr_MaxRow] = Math.Max(retMinMaxMatrix[rows, 1], value);
                return value;
            };

            i_matrix.Iterate(calcAVG);

            return retMinMaxMatrix;
        }
    }
}
