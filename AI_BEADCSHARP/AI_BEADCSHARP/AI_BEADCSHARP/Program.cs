#define TEST
#define NOWORSE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/*
 TODO:
    -Check if making it a another tread makes a difference?
 */
namespace AI_BEADCSHARP
{

    class Program
    {
        static void Main(string[] args)
        {
            int seed = 123;
            int jobs = 20;
            int machines =  10;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            SimulationWithParams(seed, jobs, machines); //TEST EVERYTHING
            #region Machines5
            //TEST first
            /*seed = 312;
            machines = 5;
            jobs = 10;
            SimulationWithParams(seed, jobs, machines); 
            jobs = 20;
            SimulationWithParams(seed, jobs, machines);
            jobs = 50;
            SimulationWithParams(seed, jobs, machines);
            jobs = 100;
            SimulationWithParams(seed, jobs, machines);
            jobs = 200;
            SimulationWithParams(seed, jobs, machines);
            jobs = 500;
            SimulationWithParams(seed, jobs, machines);*/
            #endregion
            #region Machines10
            // TEST Second
            /*seed = 132;
            machines = 10;
            jobs = 10;
            SimulationWithParams(seed, jobs, machines);
            jobs = 20;
            SimulationWithParams(seed, jobs, machines);
            jobs = 50;
            SimulationWithParams(seed, jobs, machines);
            jobs = 100;
            SimulationWithParams(seed, jobs, machines);
            jobs = 200;
            SimulationWithParams(seed, jobs, machines);
            jobs = 500;
            SimulationWithParams(seed, jobs, machines);*/
            #endregion
            #region Machines20
           // TEST Third
            /*seed = 213;
            machines = 20;
            jobs = 10;
            SimulationWithParams(seed, jobs, machines);
            jobs = 20;
            SimulationWithParams(seed, jobs, machines);
            jobs = 50;
            SimulationWithParams(seed, jobs, machines);
            jobs = 100;
            SimulationWithParams(seed, jobs, machines);
            jobs = 200;
            SimulationWithParams(seed, jobs, machines);
            jobs = 500;
            SimulationWithParams(seed, jobs, machines); */
            #endregion
            stopwatch.Stop();
            Console.WriteLine("Elapsed Time for the whole simulation: {0} seconds.",stopwatch.Elapsed.Seconds);
        }

        private static void SimulationWithParams(int seed, int jobs, int machines)
        {
            int Iteration = 10000000;
            int Cmax = 0;                                       
            double CmaxBest = Double.PositiveInfinity;           
            List<int> jobOrder = new List<int>();           
            int[,] timesTable;                                     
            List<int> jobOrderBest = new List<int>();               
            int[,] jobEndingArrayBest;
            timesTable = new int[machines, jobs];
            GenerateTimesTable(timesTable, seed);
            jobEndingArrayBest = new int[machines, jobs];
            SearchBestWithCooling(jobs,machines,timesTable, jobOrder, jobOrderBest,
                jobEndingArrayBest,Iteration,Cmax,CmaxBest);
            
        }

        private static void SearchBestWithCooling(int jobs,int machines, int[,] timesTable, List<int> jobOrder,List<int> jobOrderBest, int[,] jobEndingArrayBest, int iteration, int cmax, double cmaxBest)
        {
            const double BoltzmannConst = 1.380649;
            int[,] jobEndingArray;
            double temp = 10000;
            double Ptemp;
            double possibility;
            double epsilon = 0.001;
            int i = 1;
            /*Ptemp = Math.Exp(-((176 - 171) / (BoltzmannConst * temp)));
            Console.WriteLine(Ptemp);*/
            Random random = new Random((int)BoltzmannConst);
            while (i < iteration && temp > epsilon)
            {
                i++;

                jobEndingArray = new int[machines, jobs];
                RandomizeJobOrder(jobOrder, jobs);
                CalculateEndTimes(machines, jobOrder, jobEndingArray, timesTable);
                cmax = GetCMax(jobEndingArray, jobs, machines);
                Ptemp = Math.Exp(-((cmaxBest - cmax) / (BoltzmannConst * temp)));
                if (cmax < cmaxBest)
                {
                    cmaxBest = cmax;
                    jobEndingArrayBest = jobEndingArray;
                    jobOrderBest = jobOrder.ToList();
#if TEST
                    Console.WriteLine(temp);
#endif
                }
                //mi megy itt felre?
#if WORSE
                else
                {
                    Console.WriteLine(temp + "else");
                    possibility = random.NextDouble();
                    if (possibility < Math.Exp(-((cmaxBest - cmax) / (BoltzmannConst * temp))))
                    {
                        cmaxBest = cmax;
                        jobEndingArrayBest = jobEndingArray;
                        jobOrderBest = jobOrder.ToList();
#if TEST
                        Console.WriteLine(temp + " choosing a worse thing cos possibility.");
#endif
                    }
                }
#endif
                temp *= 0.999;
            }
            WriteOutBest(cmaxBest, timesTable, jobEndingArrayBest, jobOrderBest);

        }

        private static void WriteOutBest(double CmaxBest,int[,]timesTable, int[,] jobEndingArrayBest, List<int> jobOrderBest)
        {
            Console.WriteLine("The best results:");
            Console.WriteLine("TimeTable: (Machines(row)/Jobs(col))");
            WriteOutTables(timesTable);
            Console.WriteLine(PrintJobOrder(jobOrderBest));
            Console.WriteLine("EndingTable: (Machines(row)/Jobs(col))");
            WriteOutTables(jobEndingArrayBest);
            Console.WriteLine("CMaxBest: {0}", CmaxBest);
        }

        private static void GenerateTimesTable(int[,] timesTable, int seed)
        {
            Random random = new Random(seed);
            int rowLength = timesTable.GetLength(0);
            int colLength = timesTable.GetLength(1);

            for (int j = 0; j < rowLength; j++)
            {
                for (int m = 0; m < colLength; m++)
                {
                    timesTable[j, m] = random.Next(1,10);
                }
            }

        }

        private static void RandomizeJobOrder(List<int> jobOrder,int numberOfJobs)
        {
            jobOrder.Clear();
            int index = 0;
            int randomOrder;
            Random random = new Random();
            do
            {
                randomOrder = random.Next(0, numberOfJobs);
                if (!jobOrder.Contains(randomOrder))
                {
                    jobOrder.Add(randomOrder);
                    index++;
                }
            } while (index < numberOfJobs);
        }

        private static string PrintJobOrder(List<int> Order)
        {
            string order= "";
            foreach (int job in Order)
            {
                order = order + (job+1) + " ";
            }
            return "The order of the jobs: " +order;
        }

        private static int GetCMax(int[,] jobEndingArray, int jobs, int machines)
        {
            return jobEndingArray[machines - 1, jobs -1 ];
        }

        private static void WriteOutTables(int[,] ArraysToWrite)
        {
            int rowLength = ArraysToWrite.GetLength(0);
            int colLength = ArraysToWrite.GetLength(1);

            for (int j = 0; j < rowLength; j++)
            {
                for (int m = 0; m < colLength; m++)
                {
                    Console.Write(string.Format("{0}  ", ArraysToWrite[j, m]));
                }
                Console.Write(Environment.NewLine);
            }
        }
        private static void CalculateEndTimes(int machines, List<int> jobOrder, int[,] jobEndingArray, int[,] timesTable)
        {
            for (int j = 0; j < jobOrder.Count; j++)
            {
                for (int m = 0; m < machines; m++)
                {
                    if (j == 0 && m == 0 ) //handle first job on machine
                    {
                        jobEndingArray[m, j] = timesTable[m, jobOrder[j]] + jobEndingArray[m, j];
                    }
                    else if (m == 0) //handle first machine
                    {
                        jobEndingArray[m, j] = timesTable[m, jobOrder[j]] + jobEndingArray[m, j-1];
                    }
                    else if (j == 0) //handle first job
                    {
                        jobEndingArray[m, j] = timesTable[m, jobOrder[j]] + jobEndingArray[m-1, j];
                    }
                    else  //handle everyting else
                    {
                        if (jobEndingArray[m, j-1] < jobEndingArray[m - 1, j]) 
                        {
                            jobEndingArray[m, j] = timesTable[m, jobOrder[j]] + jobEndingArray[m - 1, j];
                        }
                        if (jobEndingArray[m - 1, j] <= jobEndingArray[m, j - 1])
                        {
                            jobEndingArray[m, j] = timesTable[m, jobOrder[j]] + jobEndingArray[m, j - 1];
                        }
                    }
                }

            }

        }
    }
}
