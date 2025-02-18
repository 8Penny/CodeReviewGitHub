using System;
using System.Collections.Generic;
using System.Linq;
using Settings;

namespace Services.Talents
{
    public static class TalentsHelper
    {

        private static float[] SPEEDS = new float[] {1.2f, 1.25f, 1.30f, 1.35f, 1.5f, 1.6f, 1.7f};
        private static float[] BENCHES_SPEEDS = new float[] {1.2f, 1.25f, 1.30f};
        private static float[] BENCHES_RESOURCES = new float[] {0.95f, 0.90f, 0.85f};
        private static float[] INACTIVE_MINUTES = new float[] {140, 180, 200, 250};
        private static float[] FLOWER_TIME = new float[] {0.8f, 0.65f, 0.5f};
        private static float[] FLOWER_SOFT = new float[] {1.5f, 2f, 2.5f};
        private static float[] CART_TIME = new float[] {0.8f, 0.65f, 0.5f};
        private static float[] CART_EMERALD_CHANCE = new float[] {0.2f, 0.35f, 0.4f};

        public static String GetDescription(Ability talent)
        {
            int index = (int)talent.Id % 10;
            if (talent.Id is >= AbilityType.Speed1 and <= AbilityType.Mining7)
            {
                return string.Format(talent.Description, SPEEDS[index]);
            }
            if (talent.Id is >= AbilityType.Time1 and <= AbilityType.Time4)
            {
                return string.Format(talent.Description, INACTIVE_MINUTES[index]);
            }

            if (talent.Id is >= AbilityType.Workbench2 and <= AbilityType.Workbench7)
            {
                bool isSpeed = index % 2 == 1;
                float[] values = isSpeed ? BENCHES_SPEEDS : BENCHES_RESOURCES;
                
                return string.Format(talent.Description, isSpeed? values[index / 2]:values[-1 + (index / 2)]);
            }
            
            if (talent.Id is >= AbilityType.CraftTable2 and <= AbilityType.CraftTable7)
            {
                bool isSpeed = index % 2 == 1;
                float[] values = isSpeed ? BENCHES_SPEEDS : BENCHES_RESOURCES;
                
                return string.Format(talent.Description, isSpeed? values[index / 2]:values[-1 + (index / 2)]);
            }

            if (talent.Id is >= AbilityType.Flower2 and <= AbilityType.Flower7)
            {
                bool isTime = index % 2 == 1;
                float[] values = isTime ? FLOWER_TIME : FLOWER_SOFT;
                return string.Format(talent.Description, isTime? 100 * (1-values[index / 2]) : 100 * values[-1 + (index / 2)]);
            }

            if (talent.Id is >= AbilityType.Cart2 and <= AbilityType.Cart7)
            {
                bool isTime = index % 2 == 1;
                float[] values = isTime ? CART_TIME : CART_EMERALD_CHANCE;
                return string.Format(talent.Description, isTime? 100 * (1-values[index / 2]) : 100 * values[-1 + (index / 2)]);
            }
            
            return talent.Description;
        }

        public static float GetFlowerCooldown(List<AbilityType> talents)
        {
            return GetOddParameter(talents, AbilityType.Flower2, AbilityType.Flower6, FLOWER_TIME);
        }
        public static float GetCartCooldown(List<AbilityType> talents)
        {
            return GetOddParameter(talents, AbilityType.Cart2, AbilityType.Cart6, CART_TIME);
        }
        public static float GetCartEmeraldChance(List<AbilityType> talents)
        {
            return GetEventParameter(talents, AbilityType.Cart3, AbilityType.Cart7, CART_EMERALD_CHANCE, 0f);
        }
        public static float GetFlowerSoftMultiplier(List<AbilityType> talents)
        {
            return GetEventParameter(talents, AbilityType.Flower3, AbilityType.Flower7, FLOWER_SOFT);
        }
        
        public static float GetSpeedParameter(List<AbilityType> talents)
        {
            return GetParameter(talents, AbilityType.Speed1, AbilityType.Speed7, SPEEDS);
        }
        public static float GetBackpackParameter(List<AbilityType> talents)
        {
            return GetParameter(talents, AbilityType.Backpack1, AbilityType.Backpack7, SPEEDS);
        }
        public static float GetMiningParameter(List<AbilityType> talents)
        {
            return GetParameter(talents, AbilityType.Mining1, AbilityType.Mining7, SPEEDS);
        }
        public static float GetWorkbenchSpeed(List<AbilityType> talents)
        {
            return GetOddParameter(talents, AbilityType.Workbench2, AbilityType.Workbench6, BENCHES_SPEEDS);
        }
        public static float GetWorkbenchDemandMultiplier(List<AbilityType> talents)
        {
            return GetEventParameter(talents, AbilityType.Workbench3, AbilityType.Workbench7, BENCHES_RESOURCES);
        }
        public static float GetCraftTableSpeed(List<AbilityType> talents)
        {
            return GetOddParameter(talents, AbilityType.CraftTable2, AbilityType.CraftTable6, BENCHES_SPEEDS);
        }
        public static float GetCraftTableDemandMultiplier(List<AbilityType> talents)
        {
            return GetEventParameter(talents, AbilityType.CraftTable3, AbilityType.CraftTable7, BENCHES_RESOURCES);
        }
        public static float GetInactiveMinutes(List<AbilityType> talents)
        {
            float result = GetParameter(talents, AbilityType.Time1, AbilityType.Time4, INACTIVE_MINUTES);
            if (result < 2f)
            {
                return 120;
            }

            return result;
        }

        private static float GetParameter(List<AbilityType> talents, AbilityType start, AbilityType end, float[] values)
        {
            talents = talents.OrderByDescending(q => (int)q).Distinct().ToList();
            foreach (var talent in talents)
            {
                int talentId = (int) talent;
                if (talentId < (int) start)
                {
                    continue;
                }
                if (talentId > (int) end)
                {
                    continue;
                }

                int index = (int) talent % 10;
                return values[index];
            }

            return 1f;
        }

        private static float GetOddParameter(List<AbilityType> talents, AbilityType start, AbilityType end, float[] values, float defaultValue = 1f)// with indices 1,3,5
        {
            talents = talents.OrderBy(q => q).Distinct().ToList();
            foreach (var talent in talents)
            {
                int talentId = (int) talent;
                if (talentId < (int) start)
                {
                    continue;
                }
                if (talentId > (int) end)
                {
                    continue;
                }

                int index = (int) talent % 10;
                if (index % 2 == 0)
                {
                    continue;
                }
                
                return values[index / 2];
            }

            return defaultValue;
        }

        private static float GetEventParameter(List<AbilityType> talents, AbilityType start, AbilityType end, float[] values, float defaultValue = 1f)// with indices 2,4,6
        {
            talents = talents.OrderByDescending(q => q).ToList();
            foreach (var talent in talents)
            {
                int talentId = (int) talent;
                if (talentId < (int) start)
                {
                    continue;
                }
                if (talentId > (int) end)
                {
                    continue;
                }

                int index = (int) talent % 10;
                if (index % 2 == 1)
                {
                    continue;
                }
                
                return values[-1 + (index / 2)];
            }

            return defaultValue;
        }
    }
}