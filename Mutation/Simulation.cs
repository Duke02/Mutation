using System;
using System.IO;

namespace Mutation
{
    class Simulation
    {
        /// <summary>
        /// The random number generator that each object in the simulation is to use.
        /// </summary>
        private Random _random;

        /// <summary>
        /// The base constructor for the Simulation.
        /// </summary>
        public Simulation()
        {
            _random = new Random();
            CreatureManager = new CreatureManager(_random);
        }

        /// <summary>
        /// The object that manages every creature.
        /// </summary>
        public CreatureManager CreatureManager { get; private set; }

        /// <summary>
        /// Adds starting creatures to the creature manager.
        /// </summary>
        public void AddCreatures(int initialCreatures)
        {
            var creature1 = new Creature(1, 0.5, 0.05, 0.5);
            var creature2 = new Creature(2, 0.15, 0.75);
            var creature3 = new Creature(3, 0.075, 0.25);
            var creature4 = new Creature(4, 0.1, 0.5);

            creature1.AddMutation(2, 0.1);
            creature1.AddMutation(4, 0.05);

            creature2.AddMutation(3, 0.15);

            creature3.AddMutation(1, 0.05);

            CreatureManager.AddCreatureToTrack(creature1);
            CreatureManager.AddCreatureToTrack(creature2);
            CreatureManager.AddCreatureToTrack(creature3);
            CreatureManager.AddCreatureToTrack(creature4);
        }

        /// <summary>
        /// Prints the stats for the simulation at the given trial number.
        /// </summary>
        /// <param name="trialNumber">The current trial number within the simulation.</param>
        public void PrintStats(int trialNumber)
        {
            var totalCreatures = CreatureManager.TotalNumberOfCreatures;

            if (totalCreatures == 0)
            {
                Console.Out.WriteLine($"Everything is dead at trial #{trialNumber}.");
                return;
            }

            Console.Out.WriteLine($"==Trial #{trialNumber}==");
            var ids = CreatureManager.GetTrackedIds();

            foreach (var id in ids)
            {
                var creaturesOfType = CreatureManager.CountOfCreatures(id);
                var percentageOfPopulation = (double) creaturesOfType / totalCreatures;

                Console.Out.WriteLine(
                    $"Creature {id}: {creaturesOfType}/{totalCreatures} ({percentageOfPopulation * 100:F2}%)");
            }
        }

        /// <summary>
        /// Updates each object within the simulation.
        /// </summary>
        public void Update()
        {
            CreatureManager.Update();
        }

        /// <summary>
        /// Returns true if there is no living creature in the simulation.
        /// </summary>
        /// <returns></returns>
        public bool AreAllCreaturesDead()
        {
            return CreatureManager.TotalNumberOfCreatures == 0;
        }

        public static void Main(string[] args)
        {
            var simulation = new Simulation();

            try
            {
                using (var file = File.OpenText("creatures.json"))
                {
                    simulation.CreatureManager.LoadFromJson(file);
                }
            }
            catch (Exception)
            {
                Console.Out.WriteLine("Error loading creatures from file. Will just add simulation creatures.");
                simulation.AddCreatures(5);
            }

            const int totalRounds = 100;

            for (var i = 1; i <= totalRounds; i++)
            {
                if (i > 1 && simulation.AreAllCreaturesDead())
                {
                    break;
                }

                simulation.Update();
                simulation.PrintStats(i);
            }
        }
    }
}