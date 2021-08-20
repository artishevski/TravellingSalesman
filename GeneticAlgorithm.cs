using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    /// <summary>
    /// This class maintains information about length of the way.
    /// </summary>
    class WaysLength
    {
        /// <remarks name = "num">Number of the way in array of ways.</remarks>
        private int num;
        /// <remarks name = "length">Length of the way.</remarks>
        private int length;

        public int Num { get => num; set => num = value; }
        public int Length { get => length; set => length = value; }
    }

    /// <summary>
    /// This class maintains all cities of the route and method to modify it.
    /// </summary>
    class Way
    {
        /// <summary>
        /// Array of all cities in the route.
        /// </summary>
        private int[] route;
        public Way(int size, Random random)
        {
            route = new int[size];
            RandomFilling(size, random);
        }

        /// <summary>
        /// Swapping routes of two Ways.
        /// </summary>
        public void SwapArr(Way way2)
        {
            int temp;
            for (int i = 0; i < route.Length; i++)
            {
                temp = route[i];
                route[i] = way2.route[i];
                way2.route[i] = temp;
            }
        }

        /// <summary>
        /// Generating random route. Firstly we fill the route with cities in order and then mix them.
        /// </summary>
        public void RandomFilling(int size, Random random)
        {
            for (int j = 0; j < size; j++)
            {
                route[j] = j;
            }
            RandomSort(route, random);
        }

        /// <summary>
        /// Mixing the cities in the route.
        /// </summary>
        private static int[] RandomSort(int[] a, Random random)
        {
            int n = a.Length;
            while (n > 1)
            {
                n--;
                int i = random.Next(n + 1);
                int temp = a[i];
                a[i] = a[n];
                a[n] = temp;
            }
            return a;
        }

        public int[] Route { get => route; set => route = value; }
    }

    /// <summary>
    /// The class that launches the program and maintains.
    /// </summary>
    class GeneticAlgorithm
    {
        /// <summary>
        /// Total amount of generated routes.
        /// </summary>
        private int n = 24;

        /// <summary>
        /// Amount of the best routes that we save after each iteration.
        /// </summary>
        private int m = 8;

        private Random random = new Random();

        /// <summary>
        /// Information about distances between cities.
        /// </summary>
        private Cities cities = new Cities();

        /// <summary>
        /// Array of all ways.
        /// </summary>
        private Way[] ways;

        /// <summary>
        /// Array of all ways lengths.
        /// </summary>
        private WaysLength[] waysLength;


        public GeneticAlgorithm()
        {
            waysLength = new WaysLength[n];
            ways = new Way[n];
            for (int i = 0; i < n; i++)
            {
                waysLength[i] = new WaysLength();
                ways[i] = new Way(cities.CitiesNum, random);
                ways[i].RandomFilling(cities.CitiesNum, random);
            }
            // Filling array of ways lengths.
            fillLength();

            // Sorting array of ways lengths by increasing.
            sortLength();
        }

        /// <summary>
        /// This method launches the logics of the program.
        /// </summary>
        public void run()
        {
            for (int i = 0; i < 20; i++)
            {
                mutate();
                fillLength();
                sortLength();
            }

            // Getting the shortest way and transforming it to start from 0 city
            int[] tempRes = ways[0].Route;
            int index = Array.IndexOf(tempRes, 0);
            List<int> res = new List<int>();
            for (int i = index; i < cities.CitiesNum; i++)
            {
                res.Add(tempRes[i]);
            }
            for (int i = 0; i < index; i++)
            {
                res.Add(tempRes[i]);
            }
            Console.Write("The shortes way: ");
            foreach (int i in res)
            {
                Console.Write($"{i} -> ");
            }
            Console.WriteLine(res[0]);
            Console.WriteLine($"Length of this way is: {waysLength[0].Length}");
        }

        /// <summary>
        /// Filling(or updating) array of ways lengths.
        /// </summary>
        private void fillLength()
        {
            // For each way we count length.
            for (int i = 0; i < n; i++)
            {
                int length = 0;
                for (int j = 1; j < cities.CitiesNum; j++)
                {
                    length += cities.CitiesDistance[ways[i].Route[j], ways[i].Route[j - 1]];
                }
                length += cities.CitiesDistance[ways[i].Route[0], ways[i].Route[6]];
                waysLength[i].Num = i;
                waysLength[i].Length = length;
            }
        }

        /// <summary>
        /// Sorting array of lengths by increasing.
        /// </summary>
        private void sortLength()
        {
            // During each iteration we look for the shortest ways of an unsorted part of array and move it to the begining of this part.
            for (int i = 0; i < n; i++)
            {
                int indMin = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (waysLength[j].Length < waysLength[indMin].Length)
                    {
                        indMin = j;
                    }
                }
                int temp = waysLength[i].Num;
                waysLength[i].Num = waysLength[indMin].Num;
                waysLength[indMin].Num = temp;

                temp = waysLength[i].Length;
                waysLength[i].Length = waysLength[indMin].Length;
                waysLength[indMin].Length = temp;
            }
        }

        /// <summary>
        /// Firstly we delete n - m worst ways then duplicate m best ways on these places and after it change places of two cities in each of these ways.
        /// </summary>
        private void mutate()
        {
            // Putting m best ways to the top of the array.
            for (int i = 0; i < m; i++)
            {
                ways[i].SwapArr(ways[waysLength[i].Num]);
            }
            int temp, temp1, temp2;
            for (int i = m; i < n; i++)
            {
                // Copy m best ways to the place of worst ones.
                for (int k = 0; k < cities.CitiesNum; k++)
                {
                    ways[i].Route[k] = ways[i % m].Route[k];
                }
                // Find two random cities of the way and swap their places.
                temp1 = random.Next(cities.CitiesNum);
                temp2 = random.Next(cities.CitiesNum);
                while (temp2 == temp1)
                {
                    temp2 = random.Next(cities.CitiesNum);
                }
                temp = ways[i].Route[temp1];
                ways[i].Route[temp1] = ways[i].Route[temp2];
                ways[i].Route[temp2] = temp;
            }
        }
    }
}
