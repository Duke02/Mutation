using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mutation
{
    public class CreatureManager
    {
        /// <summary>
        /// The creatures that are currently being simulated.
        /// </summary>
        private List<Creature> _creatures;

        /// <summary>
        /// The random number generator each Creature will have a reference to.
        /// </summary>
        private Random _random;

        /// <summary>
        /// The base creature models that this simulation is tracking.
        /// </summary>
        private Dictionary<int, Creature> _trackedCreatures;

        /// <summary>
        /// The base constructor for the Creature Manager.
        /// </summary>
        /// <param name="random">The random number generator for all creatures to use.</param>
        public CreatureManager(Random random)
        {
            _creatures = new List<Creature>();
            _trackedCreatures = new Dictionary<int, Creature>();
            _random = random;
        }

        /// <summary>
        /// The total number of creatures that are alive.
        /// </summary>
        public int TotalNumberOfCreatures => _creatures.Count(creature => creature.IsAlive);

        /// <summary>
        /// The number of living creatures with the given id.
        /// </summary>
        /// <param name="id">The id to use.</param>
        /// <returns>The number of living creatures in the simulation with the id.</returns>
        public int CountOfCreatures(int id)
        {
            return _creatures.Count(creature => creature.IsAlive && creature.Id == id);
        }

        /// <summary>
        /// Gets a list of the ids of all creatures that are being tracked.
        /// </summary>
        /// <returns></returns>
        public List<int> GetTrackedIds()
        {
            return _trackedCreatures.Keys.ToList();
        }

        /// <summary>
        /// Starts tracking the creature.
        /// </summary>
        /// <param name="creature">The creature to track.</param>
        public void AddCreatureToTrack(Creature creature)
        {
            if (!_trackedCreatures.ContainsKey(creature.Id))
            {
                creature.RandomGenerator = _random;
                _trackedCreatures.Add(creature.Id, creature);
            }
        }

        /// <summary>
        /// Adds a creature to the simulation.
        /// </summary>
        /// <param name="creature">The creature to be added.</param>
        public void AddCreatureToSimulation(Creature creature)
        {
            if (!_trackedCreatures.ContainsKey(creature.Id))
            {
                AddCreatureToTrack(creature);
            }

            creature.RandomGenerator = _random;
            _creatures.Add(creature);
        }

        /// <summary>
        /// Updates all of the creatures. To be called each clock cycle.
        /// </summary>
        public void Update()
        {
            var creaturesToRemove = new List<Creature>();
            var creaturesToAdd = new List<Creature>();

            creaturesToAdd.AddRange(SpawnCreatures());

            foreach (var creature in _creatures)
            {
                var result = creature.Update();

                // If the creature did nothing,
                if (result == 0)
                {
                    continue;
                }

                // If the creature died or replicated itself,
                if (result == creature.Id)
                {
                    if (!creature.IsAlive)
                    {
                        creaturesToRemove.Add(creature);
                    }
                    else
                    {
                        creaturesToAdd.Add(creature);
                    }
                }
                // If the creature mutated,
                else
                {
                    creaturesToAdd.Add(CreateNewCreature(result));
                }
            }

            _creatures.RemoveAll(creature => creaturesToRemove.Contains(creature));
            _creatures.AddRange(creaturesToAdd);
        }

        public void LoadFromJson(StreamReader streamReader)
        {
            var serializer = new JsonSerializer();
            var creatures = serializer.Deserialize<List<Creature>>(new JsonTextReader(streamReader));

            foreach (var creature in creatures)
            {
                AddCreatureToTrack(creature);
            }
        }

        /// <summary>
        /// Spawns a new creature with the given id, if the id is being tracked.
        /// </summary>
        /// <param name="idOfCreature">The id for the creature.</param>
        /// <returns>The generated creature if the id was being tracked.</returns>
        private Creature CreateNewCreature(int idOfCreature)
        {
            if (!_trackedCreatures.ContainsKey(idOfCreature))
            {
                return null;
            }

            var trackedCreature = _trackedCreatures[idOfCreature];
            return new Creature(trackedCreature);
        }

        /// <summary>
        /// Spawns creatures that can be born spontaneously (a creature that comes into existence without a prior creature).
        /// </summary>
        /// <returns>A list of all newly spawned creatures.</returns>
        private List<Creature> SpawnCreatures()
        {
            var spontaneousCreatures = _trackedCreatures.Values.Where(creature => creature.BirthChance > 0).ToList();
            var totalBirthRates = spontaneousCreatures.Sum(creature => creature.BirthChance);

            var output = new List<Creature>();

            var roll = _random.NextDouble() * totalBirthRates;

            foreach (var creature in spontaneousCreatures)
            {
                if (creature.BirthChance > roll)
                {
                    output.Add(CreateNewCreature(creature.Id));
                }

                roll -= creature.BirthChance;
            }

            return output;
        }
    }
}