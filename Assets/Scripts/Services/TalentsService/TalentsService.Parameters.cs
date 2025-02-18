namespace Services.Talents
{
    public partial class TalentsService
    {
        public float CatSpeedMultiplier { get; private set; }
        public float BackpackMultiplier { get; private set; }
        public float MiningRateMultiplier { get; private set; }
        public float WorkbenchMultiplier { get; private set; }
        public float CraftTableMultiplier { get; private set; }
        public float WorkbenchDemandMultiplier { get; private set; }
        public float CraftTableDemandMultiplier { get; private set; }
        public float InactiveMinutes { get; private set; }
        public float FlowerSoftMultiplier { get; private set; }
        public float FlowerCooldownMultiplier { get; private set; }
        public float CartCooldownMultiplier { get; private set; }
        public float CartEmeraldChance { get; private set; }
        

        private void UpdateParameters()
        {
            CatSpeedMultiplier = TalentsHelper.GetSpeedParameter(UnlockedAbilities);
            BackpackMultiplier  = TalentsHelper.GetBackpackParameter(UnlockedAbilities);
            MiningRateMultiplier  = TalentsHelper.GetMiningParameter(UnlockedAbilities);
            
            WorkbenchMultiplier  = TalentsHelper.GetWorkbenchSpeed(UnlockedAbilities);
            WorkbenchDemandMultiplier = TalentsHelper.GetWorkbenchDemandMultiplier(UnlockedAbilities);
            
            CraftTableMultiplier  = TalentsHelper.GetCraftTableSpeed(UnlockedAbilities);
            CraftTableDemandMultiplier = TalentsHelper.GetCraftTableDemandMultiplier(UnlockedAbilities);

            InactiveMinutes = TalentsHelper.GetInactiveMinutes(UnlockedAbilities);
            
            FlowerSoftMultiplier = TalentsHelper.GetFlowerSoftMultiplier(UnlockedAbilities); 
            FlowerCooldownMultiplier = TalentsHelper.GetFlowerCooldown(UnlockedAbilities); 
            CartCooldownMultiplier = TalentsHelper.GetCartCooldown(UnlockedAbilities); 
            CartEmeraldChance = TalentsHelper.GetCartEmeraldChance(UnlockedAbilities);
        }

    }
}