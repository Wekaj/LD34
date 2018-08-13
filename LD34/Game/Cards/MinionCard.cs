using LD34.Args;
using LD34.Game.Modifiers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Artemis;
using LD34.Systems;
using Microsoft.Xna.Framework;

namespace LD34.Game.Cards
{
    internal abstract class MinionCard : Card
    {
        private readonly int attack, maxHealth, initiative;
        private readonly List<IModifier> attackModifiers, maxHealthModifiers, initiativeModifiers;
        private int health;
        private int v1;
        private int v2;
        private int v3;
        private int v4;
        private string v5;
        private string v6;
        private Texture2D texture2D;

        protected MinionCard(int cost, int attack, int health, int initiative, string name, string tag, Texture2D piece, Vector2 pieceOffset)
            : base(cost, name, tag)
        {
            this.attack = attack;
            maxHealth = health;
            this.initiative = initiative;
            attackModifiers = new List<IModifier>();
            maxHealthModifiers = new List<IModifier>();
            initiativeModifiers = new List<IModifier>();
            this.health = maxHealth;
            Piece = piece;
            PieceOffset = pieceOffset;
            MaxHealthChanged += MinionCardMaxHealthChanged;
        }

        public event EventHandler<PlayerEventArgs> Played;
        public event EventHandler<AttackEventArgs> Attacked;
        public event EventHandler<CardValueEventArgs> AttackChanged;
        public event EventHandler<CardValueEventArgs> MaxHealthChanged;
        public event EventHandler<CardValueEventArgs> InitiativeChanged;
        public event EventHandler<CardValueEventArgs> HealthChanged;
        public event EventHandler Destroyed;

        public abstract void Initialize();

        public override void Play(EntityWorld world, bool player)
        {
            world.SystemManager.GetSystem<BoardSystem>()[0].Spawn(this, player);
            if (Played != null)
                Played(this, new PlayerEventArgs(player));
        }

        public void Destroy()
        {
            if (Destroyed != null)
                Destroyed(this, EventArgs.Empty);
        }

        public int Attack
        {
            get
            {
                int currentAttack = attack;
                foreach (IModifier modifier in attackModifiers)
                    currentAttack = modifier.Modify(currentAttack);
                return Math.Max(currentAttack, 0);
            }
        }

        public int MaxHealth
        {
            get
            {
                int currentMaxHealth = maxHealth;
                foreach (IModifier modifier in maxHealthModifiers)
                    currentMaxHealth = modifier.Modify(currentMaxHealth);
                return Math.Max(currentMaxHealth, 0);
            }
        }

        public int Initiative
        {
            get
            {
                int currentInitiative = initiative;
                foreach (IModifier modifier in initiativeModifiers)
                    currentInitiative = modifier.Modify(currentInitiative);
                return Math.Max(currentInitiative, 1);
            }
        }

        public int Health
        {
            get { return health; }
            set
            {
                int oldHealth = health;
                health = Math.Min(value, MaxHealth);
                if (oldHealth != health && HealthChanged != null)
                    HealthChanged(this, new CardValueEventArgs(oldHealth, health));
            }
        }

        public int AttackModifier => Attack - attack;
        public int MaxHealthModifier => MaxHealth - maxHealth;
        public int InitiativeModifier => Initiative - initiative;

        public Texture2D Piece { get; }
        public Vector2 PieceOffset { get; }

        public void AddAttackModifier(IModifier modifier)
        {
            int oldAttack = Attack;

            attackModifiers.Add(modifier);
            modifier.Removed += AttackModifierRemoved;

            int newAttack = Attack;
            if (oldAttack != newAttack && AttackChanged != null)
                AttackChanged(this, new CardValueEventArgs(oldAttack, newAttack));
        }

        public void ClearAttackModifiers()
        {
            int oldAttack = Attack;

            foreach (IModifier modifier in attackModifiers)
                modifier.Removed -= AttackModifierRemoved;
            attackModifiers.Clear();

            int newAttack = Attack;
            if (oldAttack != newAttack && AttackChanged != null)
                AttackChanged(this, new CardValueEventArgs(oldAttack, newAttack));
        }

        private void AttackModifierRemoved(object sender, EventArgs e)
        {
            int oldAttack = Attack;

            attackModifiers.Remove(sender as IModifier);

            int newAttack = Attack;
            if (oldAttack != newAttack && AttackChanged != null)
                AttackChanged(this, new CardValueEventArgs(oldAttack, newAttack));
        }

        public void AddMaxHealthModifier(IModifier modifier)
        {
            int oldMaxHealth = MaxHealth;

            maxHealthModifiers.Add(modifier);
            modifier.Removed += MaxHealthModifierRemoved;

            int newMaxHealth = MaxHealth;
            if (oldMaxHealth != newMaxHealth && MaxHealthChanged != null)
                MaxHealthChanged(this, new CardValueEventArgs(oldMaxHealth, newMaxHealth));
        }

        public void ClearMaxHealthModifiers()
        {
            int oldMaxHealth = MaxHealth;

            foreach (IModifier modifier in maxHealthModifiers)
                modifier.Removed -= MaxHealthModifierRemoved;
            maxHealthModifiers.Clear();

            int newMaxHealth = MaxHealth;
            if (oldMaxHealth != newMaxHealth && MaxHealthChanged != null)
                MaxHealthChanged(this, new CardValueEventArgs(oldMaxHealth, newMaxHealth));
        }

        private void MaxHealthModifierRemoved(object sender, EventArgs e)
        {
            int oldMaxHealth = MaxHealth;

            maxHealthModifiers.Remove(sender as IModifier);

            int newMaxHealth = MaxHealth;
            if (oldMaxHealth != newMaxHealth && MaxHealthChanged != null)
                MaxHealthChanged(this, new CardValueEventArgs(oldMaxHealth, newMaxHealth));
        }

        public void AddInitiativeModifier(IModifier modifier)
        {
            int oldInitiative = Initiative;

            initiativeModifiers.Add(modifier);
            modifier.Removed += InitiativeModifierRemoved;

            int newInitiative = Initiative;
            if (oldInitiative != newInitiative && InitiativeChanged != null)
                InitiativeChanged(this, new CardValueEventArgs(oldInitiative, newInitiative));
        }

        public void ClearInitiativeModifiers()
        {
            int oldInitiative = Initiative;

            foreach (IModifier modifier in initiativeModifiers)
                modifier.Removed -= InitiativeModifierRemoved;
            initiativeModifiers.Clear();

            int newInitiative = Initiative;
            if (oldInitiative != newInitiative && InitiativeChanged != null)
                InitiativeChanged(this, new CardValueEventArgs(oldInitiative, newInitiative));
        }

        public void AttackOccured(bool player, int position, int target)
        {
            if (Attacked != null)
                Attacked(this, new AttackEventArgs(player, position, target));
        }

        private void InitiativeModifierRemoved(object sender, EventArgs e)
        {
            int oldInitiative = Initiative;

            initiativeModifiers.Remove(sender as IModifier);

            int newInitiative = Initiative;
            if (oldInitiative != newInitiative && InitiativeChanged != null)
                InitiativeChanged(this, new CardValueEventArgs(oldInitiative, newInitiative));
        }

        private void MinionCardMaxHealthChanged(object sender, CardValueEventArgs e)
        {
            Health = Math.Min(MaxHealth, Health);
        }
    }
}
