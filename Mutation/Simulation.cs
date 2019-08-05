﻿using System;

namespace Mutation
{
    class Simulation
    {
        private CreatureManager _creatureManager;
        private Random _random;

        public Simulation()
        {
            _random = new Random();
            _creatureManager = new CreatureManager(_random);
        }

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

            _creatureManager.AddCreatureToTrack(creature1);
            _creatureManager.AddCreatureToTrack(creature2);
            _creatureManager.AddCreatureToTrack(creature3);
            _creatureManager.AddCreatureToTrack(creature4);
        }

        public void PrintStats(int trialNumber)
        {
            var totalCreatures = _creatureManager.TotalNumberOfCreatures;

            if (totalCreatures == 0)
            {
                Console.Out.WriteLine($"Everything is dead at trial #{trialNumber}.");
                return;
            }

            Console.Out.WriteLine($"==Trial #{trialNumber}==");
            var ids = _creatureManager.GetTrackedIds();

            foreach (var id in ids)
            {
                var creaturesOfType = _creatureManager.CountOfCreatures(id);
                var percentageOfPopulation = (double) creaturesOfType / totalCreatures;

                Console.Out.WriteLine(
                    $"Creature {id}: {creaturesOfType}/{totalCreatures} ({percentageOfPopulation * 100:F2}%)");
            }
        }

        public void Update()
        {
            _creatureManager.Update();
        }

        public bool AreAllCreaturesDead()
        {
            return _creatureManager.TotalNumberOfCreatures == 0;
        }

        public static void Main(string[] args)
        {
            var simulation = new Simulation();

            simulation.AddCreatures(5);

            var totalRounds = 100;

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