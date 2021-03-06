﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Formatter;
using TitanBot.TypeReaders;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.Overrides
{
    class Formatter : OutputFormatter
    {
        static Dictionary<BonusType, string> BonusTypeMap { get; } = LoadBonusTypes();
        static Dictionary<EquipmentClass, string> EquipmentClassMap { get; } = LoadEquipmentClasses();
        static Dictionary<HelperType, string> HelperTypeMap { get; } = LoadHelperTypes();

        static Dictionary<HelperType, string> LoadHelperTypes()
            => new Dictionary<HelperType, string>
            {
                { HelperType.Melee, "Melee" },
                { HelperType.Ranged, "Ranged" },
                { HelperType.Spell, "Spell" },
            };
        static Dictionary<EquipmentClass, string> LoadEquipmentClasses()
            => new Dictionary<EquipmentClass, string>{
                    { EquipmentClass.Aura, "Aura" },
                    { EquipmentClass.Hat, "Hat" },
                    { EquipmentClass.None, "None" },
                    { EquipmentClass.Slash, "Slash" },
                    { EquipmentClass.Suit, "Suit" },
                    { EquipmentClass.Weapon, "Weapon" },
                };
        static Dictionary<BonusType, string> LoadBonusTypes()
            => new Dictionary<BonusType, string>
            {
                {BonusType.AllDamage, "All Damage"},
                {BonusType.AllHelperDamage, "All Hero Damage"},
                {BonusType.ArmorBoost, "Armor Equipment Bonus"},
                {BonusType.ArtifactDamage, "Artifact Damage"},
                {BonusType.BurstDamageSkillAmount, "Heavenly Strike Effect"},
                {BonusType.BurstDamageSkillMana, "Heavenly Strike Cost"},
                {BonusType.ChestAmount, "Chesterson Amount"},
                {BonusType.ChestChance, "Chesterson Chance"},
                {BonusType.CritBoostSkillDuration, "Critical Strike Duration"},
                {BonusType.CritBoostSkillMana, "Critical Strike Cost"},
                {BonusType.CritChance, "Critical Chance"},
                {BonusType.DoubleFairyChance, "Double Fairy Chance"},
                {BonusType.GoldAll, "All Gold"},
                {BonusType.GoldBoss, "Boss Gold"},
                {BonusType.GoldMonster, "Titan Gold"},
                {BonusType.Goldx10Chance, "x10 Gold Chance"},
                {BonusType.HandOfMidasSkillAmount, "Hand of Midas Effect"},
                {BonusType.HandOfMidasSkillDuration, "Hand of Midas Duration"},
                {BonusType.HandOfMidasSkillMana, "Hand of Midas Cost"},
                {BonusType.HelmetBoost, "Helmet Equipment Bonus"},
                {BonusType.HelperBoostSkillAmount, "War Cry Effect"},
                {BonusType.HelperBoostSkillDuration, "War Cry Duration"},
                {BonusType.HelperBoostSkillMana, "War Cry Cost"},
                {BonusType.HelperUpgradeCost, "Hero Cost"},
                {BonusType.HSArtifactDamage, "All Artifact Damage"},
                {BonusType.MeleeHelperDamage, "Melee Hero Damage"},
                {BonusType.PetDamageMult, "Pet Damage"},
                {BonusType.PrestigeRelic, "Prestige Relics"},
                {BonusType.RangedHelperDamage, "Ranged Hero Damage"},
                {BonusType.ShadowCloneSkillAmount, "Shadow Clone Effect"},
                {BonusType.ShadowCloneSkillDuration, "Shadow Clone Duration"},
                {BonusType.ShadowCloneSkillMana, "Shadow Clone Cost"},
                {BonusType.SlashBoost, "Slash Equipment Bonus"},
                {BonusType.SpellHelperDamage, "Spell Hero Damage"},
                {BonusType.SwordBoost, "Weapon Equipment Bonus"},
                {BonusType.TapBoostSkillAmount, "Fire Sword Effect"},
                {BonusType.TapBoostSkillDuration, "Fire Sword Duration"},
                {BonusType.TapBoostSkillMana, "Fire Sword Cost"},
                {BonusType.TapDamage, "Tap Damage"},
                {BonusType.SplashDamage, "Splash Damage"},
                {BonusType.ManaRegen, "Mana Regen"},
                {BonusType.CritDamage, "Crit Damage"},
                {BonusType.ManaPoolCap, "Mana Pool Cap"},
                {BonusType.TapDamageFromHelpers, "% Tap Damage From Heroes"},
                {BonusType.MonsterHP, "Titan HP"}
            };

        public Formatter(ICommandContext context, bool altFormat) : base(context, altFormat)
        {
            Add<int>(Beautify);
            Add<double>(Beautify);
            Add<BonusType>(Beautify);
            Add<EquipmentClass>(Beautify);
            Add<HelperType>(Beautify);
            Add<TimeSpan>(Beautify);
        }

        public string Beautify(TimeSpan value)
        {
            var s = $"{value.Seconds} {(value.Seconds != 1 ? "seconds" : "second")}";
            var m = $"{value.Minutes} {(value.Minutes != 1 ? "minutes" : "minute")}";
            var h = $"{value.Hours} {(value.Hours != 1 ? "hours" : "hour")}";
            var d = $"{value.Days} {(value.Days != 1 ? "days" : "day")}";

            var sections = new List<string>();
            if (value.Days != 0)
                sections.Add(d);
            if (value.Hours != 0)
                sections.Add(h);
            if (value.Minutes != 0)
                sections.Add(m);
            if (value.Seconds != 0 || sections.Count == 0)
                sections.Add(s);

            return sections.Join(", ", " and ");
        }
        
        static string Format(string t)
            => new string(t.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();

        private string Beautify(int value)
            => Beautify((double)value);

        string alphabet = "abcdefghijklmnopqrstuvwxyz";

        private string Beautify(double value)
        {
            string sign = "";
            
            if (double.IsNaN(value))
                return "NaN";
            if (value == 0)
                return "0";
            if (value < 0)
            {
                sign = "-";
                value = -value;
            }
            if (double.IsInfinity(value))
                return sign+"∞";
            var postfixes = new string[] { "", "K", "M", "B", "T" };
            var magnitude = (int)Math.Floor(Math.Log10(value)) / 3;
            string postfix;

            if (magnitude > postfixes.Length - 1)
                postfix = alphabet[(magnitude - (postfixes.Length)) / 26].ToString() +
                          alphabet[(magnitude - (postfixes.Length)) % 26].ToString();
            else
                postfix = postfixes[magnitude];

            return sign+string.Format("{0:0.##} " + postfix, value / (Math.Pow(10, magnitude * 3)));
        }

        private string Beautify(BonusType value)
        {
            if (BonusTypeMap.ContainsKey(value))
                return BonusTypeMap[value];
            return value.ToString();
        }

        private string Beautify(EquipmentClass value)
        {
            if (EquipmentClassMap.ContainsKey(value))
                return EquipmentClassMap[value];
            return value.ToString();
        }

        private string Beautify(HelperType value)
        {
            if (HelperTypeMap.ContainsKey(value))
                return HelperTypeMap[value];
            return value.ToString();
        }
    }
}
