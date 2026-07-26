using System;

namespace Can_I_retire_yet.MonteCarlo
{
    class RetirementMonteCarlo
    {

        // Random number generator
        private static readonly Random rng = new Random();

        // Portfolio parameters
        public double InitialBalance { get; set; }
        public double AnnualWithdrawal { get; set; }
        public double StockMeanReturn { get; set; } // e.g., 0.07 = 7%
        public double StockStdDev { get; set; }     // e.g., 0.15 = 15%
        public double BondMeanReturn { get; set; }  // e.g., 0.03 = 3%
        public double BondStdDev { get; set; }      // e.g., 0.05 = 5%
        public double StockAllocation { get; set; } // e.g., 0.6 = 60% stocks
        public int Years { get; set; }
        public int Simulations { get; set; }

        public RetirementMonteCarlo(
            double initialBalance,
            double annualWithdrawal,
            double stockMeanReturn,
            double stockStdDev,
            double bondMeanReturn,
            double bondStdDev,
            double stockAllocation,
            int years,
            int simulations)
        {
            InitialBalance = initialBalance;
            AnnualWithdrawal = annualWithdrawal;
            StockMeanReturn = stockMeanReturn;
            StockStdDev = stockStdDev;
            BondMeanReturn = bondMeanReturn;
            BondStdDev = bondStdDev;
            StockAllocation = stockAllocation;
            Years = years;
            Simulations = simulations;
        }

        // Run the Monte Carlo simulation
        public double RunSimulation()
        {
            int successCount = 0;

            for (int sim = 0; sim < Simulations; sim++)
            {
                double balance = InitialBalance;

                for (int year = 0; year < Years; year++)
                {
                    // Withdraw at the start of the year
                    balance -= AnnualWithdrawal;
                    if (balance <= 0)
                    {
                        balance = 0;
                        break; // Portfolio depleted
                    }

                    // Simulate returns for stocks and bonds
                    double stockReturn = RandomNormal(StockMeanReturn, StockStdDev);
                    double bondReturn = RandomNormal(BondMeanReturn, BondStdDev);

                    // Weighted portfolio return
                    double portfolioReturn = StockAllocation * stockReturn +
                                             (1 - StockAllocation) * bondReturn;

                    // Apply growth
                    balance *= (1 + portfolioReturn);
                }

                if (balance > 0) successCount++;
            }

            // Probability of success
            return (double)successCount / Simulations;
        }

        // Generate normally distributed random numbers using Box-Muller transform
        private static double RandomNormal(double mean, double stdDev)
        {
            double u1 = 1.0 - rng.NextDouble(); // avoid log(0)
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}
