using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutation
{
    public class Creature
    {
        private Random _random;

        /// <summary>
        /// The base constructor for the creature.
        /// </summary>
        /// <param name="id">The identification for the creature.</param>
        /// <param name="deathChance">The chance this creature will die with each clock cycle.</param>
        /// <param name="replicationChance">The chance this creature will replicate with each clock cycle.</param>
        /// <param name="mutationChances">The chances this creature will mutate into a given creature.</param>
        public Creature(int id, double deathChance, double replicationChance, Dictionary<int, double> mutationChances) :
            this(id, deathChance, replicationChance)
        {
            MutationChances = mutationChances;
        }

        public Creature(int id, double deathChance, double replicationChance)
        {
            Id = id;
            DeathChance = deathChance;
            ReplicationChance = replicationChance;
            MutationChances = new Dictionary<int, double>();
            IsAlive = true;
            _random = new Random();

            // If the chances are invalid, 
            if (DeathChance + ReplicationChance > 1)
            {
                // Correct them.
                // (This is like creating a unit vector in mathematics)
                var total = DeathChance + ReplicationChance;
                DeathChance /= total;
                ReplicationChance /= total;
            }

            StandbyChance = 1 - DeathChance - ReplicationChance;
        }

        public Creature(Creature creatureToCopy) :
            this(creatureToCopy.Id, creatureToCopy.DeathChance,
                creatureToCopy.ReplicationChance, creatureToCopy.MutationChances)
        {
        }

        /// <summary>
        /// The identification of this creature's type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// True if the creature is alive.
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// The chance this creature will die at each cycle.
        ///Rep
        /// When summed with <see cref="ReplicationChance"/> and <see cref="StandbyChance"/>,
        /// the total should be 1.
        /// </summary>
        public double DeathChance { get; set; }

        /// <summary>
        /// The chance this creature will attempt to replicate itself at each cycle.
        ///
        /// When summed with <see cref="DeathChance"/> and <see cref="StandbyChance"/>,
        /// the total should be 1.
        /// </summary>
        public double ReplicationChance { get; set; }

        /// <summary>
        /// The chance this creature will do nothing at each clock cycle.
        ///
        /// When summed with <see cref="DeathChance"/> and <see cref="ReplicationChance"/>,
        /// the total should be 1.
        /// </summary>
        public double StandbyChance { get; set; }

        /// <summary>
        /// The chances that this creature will mutate to a creature with the given key.
        ///
        /// Creature must be attempting to replicate itself before this can be used.
        /// </summary>
        public Dictionary<int, double> MutationChances { get; set; }

        /// <summary>
        /// Determines if the creature dies, replicates, mutates, or does nothing for a given clock cycle.
        /// </summary>
        /// <returns>If this creature mutates, the id of the mutation. If the creature does nothing, 0. Otherwise, this creature's Id.</returns>
        public int Update()
        {
            var roll = _random.NextDouble();

            if (DeathChance > roll)
            {
                Die();
                return Id;
            }

            roll -= DeathChance;

            if (ReplicationChance > roll)
            {
                var mutationRoll = GetRollForMutation();
                foreach (var mutationChance in MutationChances)
                {
                    if (mutationChance.Value > mutationRoll)
                    {
                        return mutationChance.Key;
                    }

                    mutationRoll -= mutationChance.Value;
                }

                // The creature successfully replicated itself.
                return Id;
            }

            roll -= ReplicationChance;

            // The creature did nothing.
            return 0;
        }

        /// <summary>
        /// Adds or updates the mutation going to the creature with the given id with the given chance.
        /// </summary>
        /// <param name="id">The id to add or update.</param>
        /// <param name="chance">The chance this creature will produce a creature with the given id.</param>
        public void AddMutation(int id, double chance)
        {
            if (MutationChances.ContainsKey(id))
            {
                MutationChances[id] = chance;
            }
            else
            {
                MutationChances.Add(id, chance);
            }
        }

        /// <summary>
        /// Forces the creature to die.
        /// </summary>
        private void Die()
        {
            IsAlive = false;
        }

        /// <summary>
        /// Gets a random number used to determine if the creature mutates or replicates perfectly.
        /// </summary>
        /// <returns>The roll for mutation.</returns>
        private double GetRollForMutation()
        {
            var maxValue = MutationChances.Sum(item => item.Value) + ReplicationChance;
            return _random.NextDouble() * maxValue;
        }
    }
}