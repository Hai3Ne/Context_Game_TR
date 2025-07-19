using System;

public class ItemApplyModel
{
	public ItemApplyModel ()
	{
	}

	public static uint CheckEffectType(uint itemID)
	{
		ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet (itemID);
		EnumItemUseType useType = (EnumItemUseType)itemVo.UseType;
		if (useType == EnumItemUseType.EffectType) {
			return (uint)itemVo.Value0;
		}
		return 0;
	}
}