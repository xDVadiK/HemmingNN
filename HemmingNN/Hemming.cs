using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace HemmingNN
{
    internal class Hemming
    {
        private int ZAYsize; // Number of Z A Y neurons (number of reference images)
        private int dimensionNeuronS; // Dimension of S neurons
        private double coefficientK;
        private double coefficientUn;
        private double coefficientEpsilon;
        private int[,] vArrays; // Vectors of reference images
        private int[] sArrays; // Vector of the presented image
        private List<Button> recognizedImage; // Elements of the recognized image

        public Hemming(List<Button> referenceImages, List<Button> recognizedImage, List<Button> providedImage, double coefficientK)
        {
            this.recognizedImage = recognizedImage;

            ZAYsize = referenceImages.Count / 25;
            dimensionNeuronS = providedImage.Count;
            this.coefficientK = coefficientK;
            coefficientUn = 1 / coefficientK;
            coefficientEpsilon = 1 / (double)(referenceImages.Count / 25);

            // Filling in an array of vectors of reference images
            int shift = 0;
            vArrays = new int[ZAYsize, dimensionNeuronS];
            for (int i = 0; i < ZAYsize; i++)
            {
                for (int j = 0; j < dimensionNeuronS; j++)
                {
                    Button button = referenceImages[j + shift];
                    vArrays[i, j] = button.BackColor == Color.Black ? 1 : -1;
                }
                shift += dimensionNeuronS;
            }

            // Vector of the presented image
            sArrays = new int[dimensionNeuronS];
            for (int i = 0; i < dimensionNeuronS; i++) 
            {
                Button element = providedImage[i];
                sArrays[i] = element.BackColor == Color.Black ? 1 : -1;
            }
        }

        public bool Recognition()
        {
            // Formation of the matrix of connections of the lower subnet of the Hamming network
            double[,] w = new double[dimensionNeuronS, ZAYsize];
            for (int i = 0; i < dimensionNeuronS; i++)
            {
                for (int j = 0; j < ZAYsize; j++)
                {
                    w[i, j] = vArrays[j, i] * 0.5;
                }
            }

            // Calculation of the Z displacement of neurons
            double[] b = new double[ZAYsize];
            for (int i = 0; i < ZAYsize; i++)
            {
                b[i] = dimensionNeuronS * 0.5;
            }

            // Calculation of input signals of Z neurons
            double[] UinZ = new double[ZAYsize];
            for (int i = 0; i < ZAYsize; i++)
            {
                UinZ[i] = b[i];
                for (int j = 0; j < dimensionNeuronS; j++)
                {
                    UinZ[i] += w[j,i] * sArrays[j];
                }
            }

            // Calculation of output signals of Z neurons
            double[] UoutZ = new double[ZAYsize];
            for (int i = 0; i < ZAYsize; i++)
            {
                if (UinZ[i] <= 0)
                {
                    UoutZ[i] = 0;
                }
                else if (UinZ[i] > 0 && UinZ[i] <= coefficientUn)
                {
                    UoutZ[i] = (double)((decimal)coefficientK * (decimal)UinZ[i]);
                }
                else UoutZ[i] = coefficientUn;
            }

            // Formation of the input vector of the Maxnet subnet
            double[,] UoutA = new double[ZAYsize, ZAYsize];
            for (int i = 0; i < ZAYsize; i++)
            {
                UoutA[0, i] = UoutZ[i];
            }

            // Calculation of the iterative process in the Maxnet subnet
            for (int i = 1; i < ZAYsize; i++)
            {
                for (int j = 0; j < ZAYsize; j++)
                {
                    decimal f = (decimal)UoutA[i - 1, j] - (decimal)coefficientEpsilon * (decimal)Summation(UoutA, i, j);
                    UoutA[i, j] = g((double)((decimal)UoutA[i - 1, j] - (decimal)coefficientEpsilon * (decimal)Summation(UoutA, i, j)));
                }
            }

            // Formation of the vector of Y elements
            double[] NeuronY = new double[ZAYsize];
            int count = 0;
            int index = -1;
            for (int i = 0; i < ZAYsize; i++)
            {
                if(UoutA[ZAYsize - 1, i] > 0)
                {
                    NeuronY[i] = 1;
                    count++;
                    if (count > 1)
                    {
                        index = -1;
                        break;
                    } 
                    else
                    {
                        index = i;
                    }
                }
                else
                {
                    NeuronY[i] = 0;
                }
            }

            // Rendering an image
            if (index != -1)
            {
                for (int i = 0; i < dimensionNeuronS; i++)
                {
                    if (vArrays[index, i] == 1)
                    {
                        recognizedImage[i].BackColor = Color.Black;
                    }
                    else
                    {
                        recognizedImage[i].UseVisualStyleBackColor = true;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private double Summation(double[,] UoutA, int i, int j)
        {
            double sum = 0;
            for (int k = 0; k < ZAYsize; k++)
            {
                sum += j == k ? 0 : UoutA[i - 1, k];
            }
            return sum;
        }

        private static double g(double Uin)
        {
            return Uin > 0 ? Uin : 0;
        }
    }
}