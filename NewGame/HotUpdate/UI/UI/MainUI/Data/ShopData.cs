using System.Data.Common;
using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData 
{
    public  List<ShopItemData> shopItemDatas = new List<ShopItemData>();
    
    public ShopData() 
    {
        SetShopItemDatas();
    }
    public void SetShopItemDatas() 
    {
        var goodsList = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.FindAll(x=>x.VipgoodsFlag==0);
        for (int i = 0; i < goodsList.Count; i++)
        {
            shopItemDatas.Add(new ShopItemData(goodsList[i]));
        }
       
    }
}

public class ShopItemData 
{
    public int shopId;//商品id
    public int sortId;//排序id
    public int originalPirce;//原价
    public int realityPirce;//实际价格
    public int diamond;//实际到账金额
    public int additional;//额外奖励
    public int type;//购买类型
    public int catExp;//招财猫经验
    public string commodity;//额外赠送百分比值
    public int vipExp;//vip经验
    public int needVipLev;//购买所需vip等级
    public int vipGoodsFlag;//vip商品标识
    public string describe;//描述
    public string imgName;//商品图标
    public ShopItemData(cfg.Game.Shop_Config data) 
    {
        if (data!=null)
        {
            shopId = data.BuyId;
            sortId = data.Id;
            originalPirce = data.BuyOld;
            realityPirce = data.BuyNew;
            diamond = data.Diamond;
            additional = data.GiveDiamond;
            type = data.BuyType;
            catExp = data.CatExp;
            commodity = data.CommodityName;
            vipExp = data.VipExp;
            needVipLev = data.NeedViplev;
            vipGoodsFlag = data.VipgoodsFlag;
            describe = data.Description;
            imgName = data.Img;
        }
        
    }
}

public class VipData
{
    public List<VipItemData> vipItemDatas = new List<VipItemData>();
    public VipData() 
    {
        SetVipItemDatas();
    }

    public void SetVipItemDatas() 
    {
        //var packList = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.FindAll(x => x.VipgoodsFlag != 0);
        //var vipList = ConfigCtrl.Instance.Tables.TbVip_lvl_Config.DataList;
        //for (int i = 0; i < vipList.Count; i++)
        //{
        //    if (i==0)
        //    {
        //        vipItemDatas.Add(new VipItemData(null,vipList[i]));
        //    }
        //    else
        //    {
        //        vipItemDatas.Add(new VipItemData(packList[i-1], vipList[i]));
        //    }
            
        //}
    }
}

public class VipItemData
{
    public ShopItemData ShopData;//礼包
    public int Id;
    public int NextLv;
    public int NeedExp;
    public string VipLv;
    public string ProgressProve;
    public List<string> privilege = new List<string>();
    public VipItemData(cfg.Game.Shop_Config data,cfg.Game.Vip_lvl_Config vipData)
    {
        if (data != null)
        {
            ShopData = new ShopItemData(data);
        }
        
        Id = vipData.ID;
        NextLv = vipData.NextLv;
        NeedExp = vipData.NeedExp/100;
        VipLv = vipData.VipLvl.Substring(vipData.VipLvl.Length-1);
        ProgressProve = vipData.ProgressProve;
        privilege.Add(vipData.Privilege1);
        privilege.Add(vipData.Privilege2);
        privilege.Add(vipData.Privilege3);
        privilege.Add(vipData.Privilege4);
        privilege.Add(vipData.Privilege5);
        privilege.Add(vipData.Privilege6);
    }
}
