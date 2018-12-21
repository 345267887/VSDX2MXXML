﻿//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2016 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class includes methods to convert Java rectangular arrays (jagged arrays
//	with inner arrays of the same length).
//----------------------------------------------------------------------------------------

using mxGraph.io.vsdx.theme;

namespace mxGraph
{

    internal static class RectangularArrays
    {
        internal static OoxmlColor[][] ReturnRectangularOoxmlColorArray(int size1, int size2)
        {
            OoxmlColor[][] newArray = new OoxmlColor[size1][];
            for (int array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new OoxmlColor[size2];
            }

            return newArray;
        }

        internal static int[][] ReturnRectangularIntArray(int size1, int size2)
        {
            int[][] newArray = new int[size1][];
            for (int array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new int[size2];
            }

            return newArray;
        }

        internal static double[][] ReturnRectangularDoubleArray(int size1, int size2)
        {
            double[][] newArray = new double[size1][];
            for (int array1 = 0; array1 < size1; array1++)
            {
                newArray[array1] = new double[size2];
            }

            return newArray;
        }
    }
}