using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    //object of this class contains a route of an ant and its length
    class Ant
    {
        private List<int> route;
        private int routeLength;
        public Ant()
        {
            route = new List<int>();
            routeLength = 0;
        }

        public List<int> Route { get => route; set => route = value; }
        public int RouteLength { get => routeLength; set => routeLength = value; }
    }

    //object of this class contains a matrix of pheromone between cities and updates it
    class Pheromone
    {
        private double[,] pheromoneOnTheRoute;
        private double evaporationCoeff;
        private double leftPherCoeff;
        private double defaultPheromone;

        //creating and initializing matrix and other fields
        public Pheromone(int citiesNum, double defaultPheromone, double evaporationCoeff, double leftPherCoeff)
        {
            pheromoneOnTheRoute = new double[citiesNum, citiesNum];
            for (int i = 0; i < citiesNum; i++)
            {
                for (int j = 0; j < citiesNum; j++)
                {
                    pheromoneOnTheRoute[i, j] = defaultPheromone;
                }
                pheromoneOnTheRoute[i, i] = 0;
            }
            this.evaporationCoeff = evaporationCoeff;
            this.leftPherCoeff = leftPherCoeff;
            this.defaultPheromone = defaultPheromone;
        }
        public double[,] PheromoneOnTheRoute { get => pheromoneOnTheRoute; set => pheromoneOnTheRoute = value; }

        //updating matrix of pheromone using ways of given ants
        public void updatePheromone(List<Ant> ants, Cities cities, Ant bestAnt)
        {
            for (int i = 0; i < cities.CitiesNum; i++)
            {
                for (int j = 0; j < cities.CitiesNum; j++)
                {
                    pheromoneOnTheRoute[i, j] *= (1 - evaporationCoeff);
                }
            }
            int city1, city2;
            foreach (Ant ant in ants)
            {
                for (int i = 1; i < ant.Route.Count; i++)
                {
                    city1 = ant.Route[i - 1];
                    city2 = ant.Route[i];  
                    pheromoneOnTheRoute[city1, city2] += leftPherCoeff / ant.RouteLength;
                    pheromoneOnTheRoute[city2, city1] += leftPherCoeff / ant.RouteLength;
                }
                pheromoneOnTheRoute[ant.Route[0], ant.Route[cities.CitiesNum - 1]] += leftPherCoeff / ant.RouteLength;
                pheromoneOnTheRoute[ant.Route[cities.CitiesNum - 1], ant.Route[0]] += leftPherCoeff / ant.RouteLength;
            }
            for(int i = 1;i<cities.CitiesNum;i++)
            {
                pheromoneOnTheRoute[bestAnt.Route[i - 1], bestAnt.Route[i]] += defaultPheromone;
                pheromoneOnTheRoute[bestAnt.Route[i], bestAnt.Route[i - 1]] += defaultPheromone;
            }
            pheromoneOnTheRoute[bestAnt.Route[0], bestAnt.Route[cities.CitiesNum-1]] += defaultPheromone;
            pheromoneOnTheRoute[bestAnt.Route[cities.CitiesNum-1], bestAnt.Route[0]] += defaultPheromone;
        }
    }
    class AntColonyAlgorithm
    {
        private int antAmount = 30;
        private double defaultPheromone = 0.2;
        private double evaporationCoeff = 0.5;
        private double leftPherCoeff = 10;
        private double alpha = 1;
        private double beta = 4;
        private List<Ant> ants = new List<Ant>();
        private Pheromone pheromone;
        private Cities cities = new Cities();
        private Random random = new Random();
        private double[] visitingProbability;
        private List<bool> isVisited;

        //creating ants and Pheromone
        public AntColonyAlgorithm()
        {
            for (int i = 0; i < antAmount; i++)
            {
                ants.Add(new Ant());
            }
            pheromone = new Pheromone(cities.CitiesNum, defaultPheromone, evaporationCoeff, leftPherCoeff);
        }

        //launching all ants and updating matrix of pheromone
        public int run()
        {
            for (int i = 0; i < 20; i++)
            {
                launchAnts();
                Ant bestAnt = ants[0];
                foreach(Ant ant in ants)
                {
                    if(ant.RouteLength < bestAnt.RouteLength)
                    {
                        bestAnt = ant;
                    }
                }
                pheromone.updatePheromone(ants, cities, bestAnt);
            }
            return ants[0].RouteLength;
        }

        //launching every ant of the list
        private void launchAnts()
        {
            int notVisitedCities;
            int currentCity;
            foreach (Ant ant in ants)
            {
                ant.RouteLength = 0;
                ant.Route.Clear();
                isVisited = new List<bool>(new bool[cities.CitiesNum]);
                visitingProbability = new double[cities.CitiesNum];
                currentCity = random.Next(cities.CitiesNum);
                ant.Route.Add(currentCity);
                notVisitedCities = cities.CitiesNum - 1;
                while (notVisitedCities > 0)
                {
                    isVisited[currentCity] = true;
                    notVisitedCities--;
                    int nextCity = chooseCity(currentCity);
                    ant.Route.Add(nextCity);
                    ant.RouteLength += cities.CitiesDistance[currentCity, nextCity];
                    currentCity = nextCity;
                }
                ant.RouteLength += cities.CitiesDistance[currentCity, ant.Route[0]];
            }
        }

        //finding city that we need to visit now
        private int chooseCity(int currentCity)
        {
            fillProbabilities(currentCity);
            double randVal = random.NextDouble();
            double currProb = 0.0;
            int nextCity = -1;
            for (int i = 0; i < cities.CitiesNum; i++)
            {
                if (!isVisited[i])
                {
                    currProb += visitingProbability[i];
                    nextCity = i;
                    if (currProb > randVal)
                    {
                        break;
                    }
                }
            }
            return nextCity;
        }

        //filling probabilities of visiting all not visited cities from given
        private void fillProbabilities(int currentCity)
        {
            double sum = 0;
            for (int i = 0; i < cities.CitiesNum; i++)
            {
                if (!isVisited[i])
                {
                    sum += Math.Pow(1.0/cities.CitiesDistance[i, currentCity],alpha) * Math.Pow(pheromone.PheromoneOnTheRoute[currentCity, i],beta);
                }
            }
            for (int i = 0; i < cities.CitiesNum; i++)
            {
                if (!isVisited[i])
                {
                    visitingProbability[i] = Math.Pow(1.0 / cities.CitiesDistance[i, currentCity], alpha) * Math.Pow(pheromone.PheromoneOnTheRoute[currentCity, i],beta) / sum;
                }
            }
        }
    }
}
