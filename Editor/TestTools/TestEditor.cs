using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class TestEditor : EditorWindow {
    public class ItemMessage {
        public string name;
        public ItemsVo vo;
        public AutoUseVo AutoUseVo;
        public int InitCount = 0;//初始数量
        public bool IsMeet = true;//是否进行条件释放
        public bool IsAuto = false;//是否自动释放
        public int UseCount = 0;//已经使用数量
        public float CD = 0;//技能使用CD
        public float _time = 0;
    }
    [MenuItem("Tools/测试工具 %t")]
    static void AddWindow() {
        //创建窗口
        EditorWindow.GetWindow<TestEditor>(false, "测试工具",true).Show();
    }

    public void Awake() {
        List<ResLoadItem> loadList = new List<ResLoadItem>();
        ConfigTables.Instance.LunchAllConf(loadList);
        foreach (var itm in loadList) {
            string url = Application.dataPath + "/" + itm.resId + ".byte";
            byte[] buffer = System.IO.File.ReadAllBytes(url);
            BinaryAsset asset = new BinaryAsset();
            asset.bytes = buffer;
            itm.finishCb.TryCall(asset);
        }
    }

    public long mInitGold;
    public List<ItemMessage> mItemList = new List<ItemMessage>();
    private Vector2 _scrollPos;
    //绘制窗口时调用
    void OnGUI() {
        EditorGUILayout.Space();
        if (GUILayout.Button("重置数据")) {
            mInitGold = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.FModel.SelfClientSeat);
            mItemList.Clear();
            TimeRoomVo roomVo = SceneLogic.Instance.RoomVo;
            //TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet(903u);
            if (roomVo != null) {
                ItemsVo vo;
                for (int i = 0; i < roomVo.Items.Length; i++) {
                    vo = FishConfig.Instance.Itemconf.TryGet(roomVo.Items[i]);
                    mItemList.Add(new ItemMessage {
                        name = StringTable.GetString(vo.ItemName),
                        vo = vo,
                        AutoUseVo = FishConfig.Instance.mAutoUseConf.TryGet(vo.AutoType),
                        InitCount = RoleItemModel.Instance.getItemCount(vo.CfgID),
                    });
                }
                for (int i = 0; i < roomVo.Heroes.Length; i++) {
                    vo = FishConfig.Instance.Itemconf.TryGet(roomVo.Heroes[i]);
                    mItemList.Add(new ItemMessage {
                        name = StringTable.GetString(vo.ItemName),
                        vo = vo,
                        AutoUseVo = FishConfig.Instance.mAutoUseConf.TryGet(vo.AutoType),
                        InitCount = RoleItemModel.Instance.getItemCount(vo.CfgID),
                    });
                }
            }
        }

        if (mItemList.Count > 0) {
            long gold = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.FModel.SelfClientSeat);
            EditorGUILayout.LabelField(string.Format("初始金额:{0}", this.mInitGold));
            EditorGUILayout.LabelField(string.Format("现有金额:{0}", gold));
            EditorGUILayout.LabelField(string.Format("盈亏:{0}", gold - this.mInitGold));
        }
        ItemMessage msg;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        for (int i = 0; i < mItemList.Count; i++) {
            msg = mItemList[i];
            EditorGUILayout.LabelField(string.Format("道具:{0}", msg.name));
            int count = RoleItemModel.Instance.getItemCount(msg.vo.CfgID);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("现有数量:{0}", count), GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField(string.Format("初始数量:{0}", msg.InitCount));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            msg.IsAuto = EditorGUILayout.ToggleLeft(" 是否自动释放", msg.IsAuto, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField(string.Format("已使用个数:{0}", msg.UseCount));
            EditorGUILayout.EndHorizontal();
            if (msg.IsAuto) {
                msg.IsMeet = EditorGUILayout.ToggleLeft(" 是否条件释放", msg.IsMeet);
            }
            msg.CD = EditorGUILayout.FloatField(" 技能释放间隔", msg.CD);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("刷新")) {
            this.Repaint();
        }
        EditorGUILayout.Space();
    }

    ItemMessage item;
    public void Update() {
        for (int i = 0; i < mItemList.Count; i++) {
            item = mItemList[i];
            if (item.IsAuto) {
                if (item._time < item.CD) {
                    item._time += Time.deltaTime;
                } else {
                    if (item.IsMeet == false || (item.IsMeet == true && ItemManager.IsMeet(SceneLogic.Instance.PlayerMgr.MySelf, item.AutoUseVo))) {
                        item._time = 0;
                        if ((EnumItemType)item.vo.ItemType == EnumItemType.Hero) {//英雄技能
                            if (RoleItemModel.Instance.getItemCount(item.vo.CfgID) > 0 && SceneLogic.Instance.HeroMgr.LaunchHero(item.vo.CfgID)) {
                                item.UseCount++;
                                this.Repaint();
                            }
                        } else {// if ((EnumItemType)item.vo.ItemType == EnumItemType.Skill || (EnumItemType)item.vo.ItemType == EnumItemType.CallFish) {//道具技能
                            if (SceneLogic.Instance.LogicUI.ItemListUI.UserSkill(item.vo.CfgID, GameConfig.OP_QuickBuy)) {
                                item.UseCount++;
                                this.Repaint();
                            }
                        }
                    }
                }
            }
        }
    }
}
