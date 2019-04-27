using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Maths
{
    public class Matrix
    {
        /// <summary>
        /// Data of a matrix
        /// </summary>
        double[,] Data = null; //data[vertical][horizontal]
        /// <summary>
        /// Width of matrix
        /// </summary>
        int Width;
        /// <summary>
        /// Height of matrix
        /// </summary>
        int Height;
        /// <summary>
        /// Constructor of Matrix
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        #region Constructor

        public Matrix(int Width, int Height)
        {
            this.Height = Height;
            this.Width = Width;
            Data = new double[Height, Width];
        }

        public Matrix(double[,] m)
        {
            Height = m.GetLength(0);
            Width = m.GetLength(1);
            Data = new double[Height, Width];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Data[y, x] = m[y, x];
        }

        public Matrix(double[] m)
        {
            Height = 1;
            Width = m.GetLength(0);
            Data = new double[Height, Width];
            for (int x = 0; x < Width; x++)
                Data[0, x] = m[x];
        }

        #endregion Constructor

        #region Opertaional Functions
        //Adding values to matrix
        public void Set(int y, int x, double v)
        {
            Data[y, x] = v;
        }
        //reading values from matrix
        public double Get(int y, int x)
        {
            return Data[y, x];
        }

        public double[,] getMatrix()
        {
            double[,] temp = (double[,])Data.Clone();
            return temp;
        }

        #endregion Opertaional Functions

        //math
        //Calculating the determinant of Matrix
        public double determinant()
        {
            if (Width != Height) throw new Exception("None-square Matrix");
            if (Width == 1) return Data[0, 0];
            else if (Width == 2) return Data[0, 0] * Data[1, 1] - Data[1, 0] * Data[0, 1];
            double postive = 0, negative = 0;
            for (int x = 0; x < Width; x++)
            {
                double pos = 1, neg = 1;
                for (int y = 0; y < Width; y++)
                {
                    pos *= Data[y, (Width + x + y) % Width];
                    neg *= Data[y, (Width + x - y) % Width];
                }
                postive += pos;
                negative += neg;
            }
            return postive - negative;
        }
        //smaller matrix
        public Matrix minorMatrix(int py, int px)
        {
            if (Width != Height) throw new Exception("None-square Matrix");
            else if (Width < 2 || Height < 2) throw new Exception("Matrix too small.");
            else if (py >= Height || px >= Width) throw new Exception("out of matrix bound.");
            Matrix temp = new Matrix(Height - 1, Width - 1);
            for (int y = 0; y < py; y++)
            {
                for (int x = 0; x < px; x++)
                    temp.Set(y, x, Data[y, x]);
                for (int x = px; x < Width - 1; x++)
                    temp.Set(y, x, Data[y, x + 1]);
            }
            for (int y = py; y < Height - 1; y++)
            {
                for (int x = 0; x < px; x++)
                    temp.Set(y, x, Data[y + 1, x]);
                for (int x = px; x < Width - 1; x++)
                    temp.Set(y, x, Data[y + 1, x + 1]);
            }
            return temp;
        }
        //calcuates the determinant of the minor Matrix
        public double minor(int py, int px)
        {
            return this.minorMatrix(py, px).determinant();
        }
        //finding the transpose of the matrix
        public Matrix transpose()
        {
            Matrix temp = new Matrix(Width, Height);
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    temp.Set(x, y, Data[y, x]);
            return temp;
        }
        //finding the cofactor of the matrix
        public Matrix cofactor()
        {
            if (Width != Height) throw new Exception("None-square Matrix");
            Matrix temp = new Matrix(Height, Width);
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    temp.Set(y, x, this.minor(y, x) * Math.Pow(-1, x + y));
            return temp;
        }
        //finding the inverse of the matrix
        public Matrix inverse()
        {
            return this.cofactor().transpose() / this.determinant();
        }

        public Matrix pseudo_inverse()
        {
            return ((this.transpose() * this).inverse()) * this.transpose();
        }
        //addition
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Width != m2.Height) throw new Exception("Matrix unmatch");
            Matrix temp = new Matrix(m1.Height, m1.Width);
            for (int y = 0; y < temp.Height; y++)
                for (int x = 0; x < temp.Width; x++)
                    temp.Set(y, x, m1.Get(y, x) + m2.Get(y, x));
            return temp;
        }
        //subtraction
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            return m1 + (m2 * -1);
        }
        //multiplication
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Width != m2.Height) throw new Exception("Matrix unmatch");
            Matrix temp = new Matrix(m1.Height, m2.Width);
            try
            {
                for (int y = 0; y < temp.Height; y++)
                    for (int x = 0; x < temp.Width; x++)
                    {
                        double v = 0;//hold the value for a grid
                        for (int i = 0; i < m1.Width; i++)
                            v += m1.Get(y, i) * m2.Get(i, x);
                        temp.Set(y, x, v);
                    }
            }
            catch (System.IndexOutOfRangeException) { }
            return temp;
        }
        //constant multiplication
        public static Matrix operator *(Matrix m1, double d)
        {
            Matrix temp = new Matrix(m1.Height, m1.Width);
            for (int y = 0; y < temp.Height; y++)
                for (int x = 0; x < temp.Width; x++)
                {
                    temp.Set(y, x, m1.Get(y, x) * d);
                }
            return temp;
        }
        //division
        public static Matrix operator /(Matrix m1, double d)
        {
            return m1 * (1 / d);
        }
        //to string matrix
        public override string ToString()
        {
            string str = "";
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    str += Data[y, x] + ", ";
                str += "\n";
            }
            return str;
        }
    }
}
