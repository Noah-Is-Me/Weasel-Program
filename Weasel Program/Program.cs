using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Weasel_Program
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();

            string targetString = "METHINKS IT IS LIKE A WEASEL";

            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

            int attempts = 15;
            int populationSize = 100;
            float mutationChance = 0.00001f;
            float increment = 0.00001f;
            float totalIncrements = 100;

            Dictionary<float, float> averageGenerations = new Dictionary<float, float>();

            for (int z = 0; z < totalIncrements; z++)
            {
                averageGenerations.Add(mutationChance, 0);
                stopwatch.Restart();

                for (int n = 0; n < attempts; n++)
                {

                    
                    string[] strings = new string[populationSize];
                    int[] scores = new int[populationSize];

                    bool halt = false;

                    string fittestString;
                    int fittestScore = 0;

                    int generations = 0;
                    string startingString = "";

                    for (int i = 0; i < targetString.Length; i++)
                    {
                        startingString += characters[random.Next(0, characters.Length)];
                    }
                    fittestString = startingString;

                    for (int i = 0; i < populationSize; i++)
                    {
                        strings[i] = startingString;
                        scores[i] = 0;
                    }

                    while (!halt)
                    {
                        generations++;
                        for (int i = 0; i < strings.Length; i++)
                        {

                            for (int j = 0; j < strings[i].Length; j++)
                            {
                                double roll = random.NextDouble();
                                if (roll <= mutationChance)
                                {
                                    char newCharacter = characters[random.Next(0, characters.Length)];
                                    strings[i] = strings[i].Substring(0, j) + newCharacter + strings[i].Substring(j + 1);
                                }

                                if (strings[i][j] == targetString[j])
                                {
                                    scores[i] += 1;
                                }
                            }

                            if (scores[i] > fittestScore)
                            {
                                fittestScore = scores[i];
                                fittestString = strings[i];
                            }

                            if (strings[i] == targetString)
                            {
                                halt = true;
                            }
                        }

                        strings = new string[populationSize];
                        scores = new int[populationSize];

                        for (int i = 0; i < populationSize; i++)
                        {
                            strings[i] = fittestString;
                            scores[i] = 0;
                        }
                        //Debug.WriteLine(fittestString);
                    }

                    averageGenerations[mutationChance] += generations;
                }

                Debug.WriteLine("");
                Debug.WriteLine(z);
                Debug.WriteLine(stopwatch.ElapsedMilliseconds + "ms");

                averageGenerations[mutationChance] /= attempts;

                mutationChance += increment;
            }

            Debug.WriteLine("");
            foreach (KeyValuePair<float, float> dataPair in averageGenerations)
            {
                Debug.WriteLine(dataPair.Key + "; " + dataPair.Value);
            }
            CreateGraph(averageGenerations);



            void CreateGraph(Dictionary<float, float> data)
            {
                Chart chart = new Chart();

                ChartArea CA = chart.ChartAreas.Add("A1");
                Series generations = chart.Series.Add("Average Generations");
                generations.ChartType = SeriesChartType.FastLine;

                chart.BackColor = Color.White;
                CA.BackColor = Color.White;

                CA.AxisY.Title = "Average Generations";
                CA.AxisX.Title = "Mutation Rates (mutations per gene)";

                CA.AxisX.TitleAlignment = StringAlignment.Center;
                CA.AxisY.TitleAlignment = StringAlignment.Center;

                CA.AxisX.TitleFont = new Font("Ariel", 15, FontStyle.Bold);
                CA.AxisY.TitleFont = new Font("Ariel", 15, FontStyle.Bold);

                CA.AxisX.Minimum = 0;

                chart.Titles.Add("Average Generations to Mutation Rate");
                chart.Titles.ElementAt(0).Font = new Font("Ariel", 15, FontStyle.Bold);
                chart.Size = new Size(1920, 1080);
                chart.Series["Average Generations"].BorderWidth = 4;
                chart.Series["Average Generations"].Color = Color.Blue;

                chart.AntiAliasing = AntiAliasingStyles.Graphics;
                chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;


                foreach (KeyValuePair<float, float> dataPoint in data)
                {
                    chart.Series["Average Generations"].Points.AddXY(Math.Round(dataPoint.Key, 4), dataPoint.Value);
                }


                string graphInfo = $"Weasel Program {DateTime.Now.ToString("MMM-dd-yyyy hh-mm-ss-fff tt")}";
                //string imagePath = @"C:\\Users\\Noah Sonfield\\source\\repos\\Graph Test\\Graph Test\\Graphs\\" + graphInfo + ".png";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDirectory).FullName).FullName).FullName;
                string graphsFolderPath = Path.Combine(projectDirectory, "Graphs");
                string imagePath = Path.Combine(graphsFolderPath, graphInfo + ".png");

                Debug.WriteLine("Image Path: " + imagePath);

                chart.SaveImage(imagePath, ChartImageFormat.Png);
            }
        }
    }
}
