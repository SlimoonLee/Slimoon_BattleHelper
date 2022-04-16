using UnityModManagerNet;

namespace Slimoon_BattleHelper

{
    public class Settings : UnityModManager.ModSettings
    {
		public bool G_Cheat = false;
		public bool GodModeForGongFaUse = false;
		public bool AllWeaponPermitted = false;
		public bool OnlyForTaiWu = false;
		public bool TwoPMove = false;
		public int SaveP1 = 20;
		public int SaveP2 = 90;
		public bool Cheat = false;
		public bool AutoChange = false;
		public bool ForYunv = false;
		public bool ForcedCost = false;
		public int ForcedCostType = 0;
		public bool ForcedBuWei = false;
		public int ForcedBuWeiType = 0;
		public bool WeakBuWei = false;
		public bool ChanceGoFirst = false;
		public bool ChanceEnhancedByHand = false;
		public bool GivpNoBest = false;
		public bool DamGoFirst = false;
		//public int PartGoFirst = 0;
        //此处添加变量
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
