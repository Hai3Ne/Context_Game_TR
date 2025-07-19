# MINIMAL REQUIRED DATA - Fish_3D Standalone Chi Tiết

## 🎯 **TỔNG QUAN**

Để vào thẳng Fish_3D game mà bypass main hall, cần **4 nhóm data chính:**

1. **📊 Core Game Data** - Room/Table/Seat info
2. **👤 User Identity Data** - Player authentication 
3. **🔧 Game Configuration** - Launcher, room settings
4. **🎮 Runtime Data** - Players, buffs, hero data

---

## 📊 **1. CORE GAME DATA (Required)**

### **JoinRoomInfo Structure:**
```csharp
public class JoinRoomInfo {
    // ===== BẮT BUỘC =====
    public uint RoomID;              // ID phòng game (e.g., 901)
    public ushort TableID;           // Số bàn (0-65535) 
    public byte Seat;                // Vị trí ngồi (0-5)
    public EnumRoomType roomType;    // Loại phòng
    public bool IsLookOn;            // false = chơi, true = xem
    
    // ===== LAUNCHER CONFIG =====
    public uint ServerLauncherTypeID;  // Config súng bắn
    public uint RateValue;             // Tỉ lệ bắn (1-100)
    public long LcrEngery;             // Năng lượng súng
    
    // ===== VISUAL CONFIG =====
    public uint BackgroundImage;       // ID background (1-10)
    public float mXiuYuQiEndTime;     // Thời gian kết thúc휴渔期 (0 = không có)
    
    // ===== MULTIPLAYER DATA =====
    public PlayerJoinTableInfo[] playerJoinArray;    // Danh sách players
    public HashSet<ushort> PlayerSeatList;           // Ghế đã được chiếm
    
    // ===== GAME STATE =====
    public tagHeroCache[] HeroData;                  // Hero/Boss data
    public List<tagBuffCache[]> buffCacheList;       // Buff data (4 arrays)
    public ushort LastBulletID;                      // ID đạn cuối cùng
}
```

### **Giá trị tối thiểu:**
```csharp
JoinRoomInfo minimalData = new JoinRoomInfo {
    // Core identifiers
    RoomID = 901,                    // Room mặc định
    TableID = 1,                     // Bàn số 1
    Seat = 0,                        // Ghế đầu tiên
    roomType = EnumRoomType.Normal_TimeLimit,
    IsLookOn = false,                // Chơi thật, không xem
    
    // Launcher defaults
    ServerLauncherTypeID = 0,        // Sẽ được tính sau
    RateValue = 1,                   // Tỉ lệ bắn cơ bản
    LcrEngery = 1000,               // Năng lượng ban đầu
    
    // Visual defaults
    BackgroundImage = 1,             // Background đầu tiên
    mXiuYuQiEndTime = 0,            // Không có휴渔기
    LastBulletID = 0,               // Bắt đầu từ 0
    
    // Collections (khởi tạo rỗng)
    PlayerSeatList = new HashSet<ushort>(),
    buffCacheList = new List<tagBuffCache[]>(),
    HeroData = new tagHeroCache[0],
    playerJoinArray = new PlayerJoinTableInfo[0]  // Sẽ setup sau
};
```

---

## 👤 **2. USER IDENTITY DATA (Required)**

### **PlayerInfo Structure:**
```csharp
public class PlayerInfo {
    // ===== BẮT BUỘC =====
    public uint UserID;              // ID duy nhất của user
    public string NickName;          // Tên hiển thị
    public long GoldNum;             // Số vàng hiện tại
    public int VipLv;                // Level VIP (ảnh hưởng launcher)
    
    // ===== GAME SESSION =====
    public byte ChairSeat;           // Ghế ngồi (0-5)
    public ushort TableID;           // Bàn chơi
    public byte UserStatus;          // Trạng thái (US_SIT, US_PLAYING, etc.)
    
    // ===== VISUAL/OPTIONAL =====
    public bool Gender;              // Giới tính (true=male, false=female)
    public uint FaceID;              // ID avatar (1-100)
    public uint Grade;               // Cấp độ player
    public byte PreUserStatus;       // Trạng thái trước đó
}
```

### **Setup User Identity:**
```csharp
public static void SetupUserIdentity(uint userID = 0, string nickName = null) {
    // Generate UserID if not provided
    if (userID == 0) {
        userID = GenerateUserID();  // Various methods below
    }
    
    if (string.IsNullOrEmpty(nickName)) {
        nickName = $"Player_{userID}";
    }
    
    RoleInfoModel.Instance.Self = new PlayerInfo {
        // Identity
        UserID = userID,
        NickName = nickName,
        
        // Game economics
        GoldNum = 1000000,           // Starting gold
        VipLv = 1,                   // Basic VIP
        
        // Session info
        ChairSeat = 0,               // Will be set from JoinRoomInfo
        TableID = 1,                 // Will be set from JoinRoomInfo
        UserStatus = (byte)EnumUserStats.US_SIT,
        
        // Defaults
        Gender = true,               // Male
        FaceID = 1,                  // Default avatar
        Grade = 1,                   // Beginner level
        PreUserStatus = (byte)EnumUserStats.US_FREE
    };
    
    // Global settings
    RoleInfoModel.Instance.RoomCfgID = 901;  // Default room
    RoleInfoModel.Instance.isInRoom = true;
    RoleInfoModel.Instance.GameMode = EnumGameMode.Mode_Time;
    RoleInfoModel.Instance.CoinMode = EnumCoinMode.Gold;
}
```

---

## 🔧 **3. USER ID GENERATION METHODS**

### **Option A: Static ID (Offline)**
```csharp
public static uint GenerateUserID_Static() {
    return 99999;  // Fixed ID for offline play
}
```

### **Option B: Hardware-Based**
```csharp
public static uint GenerateUserID_Hardware() {
    string hwid = SystemInfo.deviceUniqueIdentifier;
    return (uint)hwid.GetHashCode();
}
```

### **Option C: Time-Based**
```csharp
public static uint GenerateUserID_Time() {
    return (uint)(System.DateTime.Now.Ticks % uint.MaxValue);
}
```

### **Option D: Random**
```csharp
public static uint GenerateUserID_Random() {
    System.Random rand = new System.Random();
    return (uint)rand.Next(100000, 999999);
}
```

### **Option E: Command Line**
```csharp
public static uint GenerateUserID_CommandLine() {
    string[] args = System.Environment.GetCommandLineArgs();
    
    for (int i = 0; i < args.Length - 1; i++) {
        if (args[i] == "-userid") {
            if (uint.TryParse(args[i + 1], out uint userId)) {
                return userId;
            }
        }
    }
    
    return GenerateUserID_Hardware();  // Fallback
}
```

---

## 🎮 **4. GAME CONFIGURATION DATA**

### **A. Room Configuration:**
```csharp
public static void SetupRoomConfig(uint roomID) {
    // Load room config from data
    TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet(roomID);
    
    if (roomVo == null) {
        LogMgr.LogWarning($"Room {roomID} not found, using default 901");
        roomVo = FishConfig.Instance.TimeRoomConf.TryGet(901u);
    }
    
    // Apply room settings
    RoleInfoModel.Instance.RoomCfgID = roomVo.CfgID;
    
    // Room requirements (for validation)
    uint requiredGold = roomVo.Gold;
    int requiredVip = roomVo.MinEnterMember;
    
    // Ensure user meets requirements
    if (RoleInfoModel.Instance.Self.GoldNum < requiredGold) {
        RoleInfoModel.Instance.Self.GoldNum = requiredGold * 2;  // Give enough gold
    }
    
    if (RoleInfoModel.Instance.Self.VipLv < requiredVip) {
        RoleInfoModel.Instance.Self.VipLv = requiredVip;         // Upgrade VIP
    }
}
```

### **B. Launcher Configuration:**
```csharp
public static void SetupLauncherConfig(JoinRoomInfo joinInfo) {
    TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet(joinInfo.RoomID);
    
    if (roomVo != null) {
        // Get launcher info based on room and VIP level
        uint cfg_id; byte lv; uint rate;
        FishConfig.GetLauncherInfo(roomVo, RoleInfoModel.Instance.Self.VipLv, 
                                  out cfg_id, out lv, out rate);
        
        joinInfo.ServerLauncherTypeID = Utility.MakeServerLCRID(cfg_id, lv, true);
        joinInfo.RateValue = rate;
    } else {
        // Default launcher config
        joinInfo.ServerLauncherTypeID = Utility.MakeServerLCRID(1, 1, true);
        joinInfo.RateValue = 1;
    }
}
```

---

## 🎯 **5. PLAYER ARRAY SETUP**

### **Single Player Setup:**
```csharp
public static void SetupSinglePlayerArray(JoinRoomInfo joinInfo) {
    PlayerJoinTableInfo playerInfo = new PlayerJoinTableInfo {
        playerInfo = RoleInfoModel.Instance.Self,
        Seat = joinInfo.Seat,
        ChangeGold = RoleInfoModel.Instance.Self.GoldNum,
        lcrEngery = joinInfo.LcrEngery,
        ServerLauncherTypeID = joinInfo.ServerLauncherTypeID,
        RateValue = joinInfo.RateValue
    };
    
    joinInfo.playerJoinArray = new PlayerJoinTableInfo[] { playerInfo };
    joinInfo.PlayerSeatList.Add(joinInfo.Seat);
}
```

### **Multi-Player Setup (Optional):**
```csharp
public static void SetupMultiPlayerArray(JoinRoomInfo joinInfo, int botCount = 2) {
    List<PlayerJoinTableInfo> players = new List<PlayerJoinTableInfo>();
    
    // Add self
    players.Add(new PlayerJoinTableInfo {
        playerInfo = RoleInfoModel.Instance.Self,
        Seat = joinInfo.Seat,
        ChangeGold = RoleInfoModel.Instance.Self.GoldNum,
        lcrEngery = joinInfo.LcrEngery,
        ServerLauncherTypeID = joinInfo.ServerLauncherTypeID,
        RateValue = joinInfo.RateValue
    });
    
    // Add bots
    for (int i = 0; i < botCount; i++) {
        byte botSeat = (byte)((joinInfo.Seat + i + 1) % 6);  // Avoid conflicts
        
        PlayerInfo botPlayer = new PlayerInfo {
            UserID = (uint)(200000 + i),
            NickName = $"Bot_{i + 1}",
            GoldNum = 500000,
            VipLv = 1,
            ChairSeat = botSeat,
            TableID = joinInfo.TableID,
            UserStatus = (byte)EnumUserStats.US_SIT,
            Gender = i % 2 == 0,
            FaceID = (uint)(i + 2),
            Grade = 1
        };
        
        players.Add(new PlayerJoinTableInfo {
            playerInfo = botPlayer,
            Seat = botSeat,
            ChangeGold = botPlayer.GoldNum,
            lcrEngery = joinInfo.LcrEngery,
            ServerLauncherTypeID = joinInfo.ServerLauncherTypeID,
            RateValue = joinInfo.RateValue
        });
        
        joinInfo.PlayerSeatList.Add(botSeat);
    }
    
    joinInfo.playerJoinArray = players.ToArray();
}
```

---

## 🎮 **6. BUFF & HERO DATA SETUP**

### **Buff Cache Setup:**
```csharp
public static void SetupBuffCache(JoinRoomInfo joinInfo) {
    // Initialize 4 empty buff arrays (required by game)
    joinInfo.buffCacheList = new List<tagBuffCache[]>();
    
    for (int i = 0; i < 4; i++) {
        joinInfo.buffCacheList.Add(new tagBuffCache[0]);
    }
}
```

### **Hero Data Setup:**
```csharp
public static void SetupHeroData(JoinRoomInfo joinInfo) {
    // For basic game, can use empty hero data
    joinInfo.HeroData = new tagHeroCache[0];
    
    // Or setup some default heroes
    /*
    joinInfo.HeroData = new tagHeroCache[] {
        new tagHeroCache { HeroID = 1, Level = 1 },
        new tagHeroCache { HeroID = 2, Level = 1 }
    };
    */
}
```

---

## ⚡ **7. COMPLETE SETUP FUNCTION**

### **All-in-One Setup:**
```csharp
public static JoinRoomInfo CreateCompleteJoinRoomInfo(
    uint roomID = 901, 
    ushort tableID = 1, 
    byte seat = 0,
    uint userID = 0,
    string nickName = null,
    bool addBots = false) {
    
    // 1. Setup user identity
    SetupUserIdentity(userID, nickName);
    
    // 2. Create basic JoinRoomInfo
    JoinRoomInfo joinInfo = new JoinRoomInfo {
        RoomID = roomID,
        TableID = tableID,
        Seat = seat,
        roomType = EnumRoomType.Normal_TimeLimit,
        IsLookOn = false,
        LcrEngery = 1000,
        BackgroundImage = 1,
        mXiuYuQiEndTime = 0,
        LastBulletID = 0,
        PlayerSeatList = new HashSet<ushort>(),
        buffCacheList = new List<tagBuffCache[]>(),
        HeroData = new tagHeroCache[0]
    };
    
    // 3. Setup room config
    SetupRoomConfig(roomID);
    
    // 4. Setup launcher config
    SetupLauncherConfig(joinInfo);
    
    // 5. Setup player array
    if (addBots) {
        SetupMultiPlayerArray(joinInfo, 2);
    } else {
        SetupSinglePlayerArray(joinInfo);
    }
    
    // 6. Setup buff cache
    SetupBuffCache(joinInfo);
    
    // 7. Setup hero data
    SetupHeroData(joinInfo);
    
    // 8. Update user with final data
    RoleInfoModel.Instance.Self.ChairSeat = seat;
    RoleInfoModel.Instance.Self.TableID = tableID;
    
    return joinInfo;
}
```

---

## 🚀 **8. USAGE EXAMPLES**

### **Example 1: Simplest Setup**
```csharp
void Start() {
    JoinRoomInfo joinInfo = CreateCompleteJoinRoomInfo();
    StartCoroutine(StartGameWithDelay(joinInfo));
}
```

### **Example 2: Custom Setup**
```csharp
void Start() {
    JoinRoomInfo joinInfo = CreateCompleteJoinRoomInfo(
        roomID: 902,
        tableID: 5,
        seat: 2,
        userID: 12345,
        nickName: "MyPlayer",
        addBots: true
    );
    
    StartCoroutine(StartGameWithDelay(joinInfo));
}
```

### **Example 3: Command Line Setup**
```csharp
void Start() {
    uint roomID = ParseRoomIDFromArgs();
    ushort tableID = ParseTableIDFromArgs();
    uint userID = GenerateUserID_CommandLine();
    
    JoinRoomInfo joinInfo = CreateCompleteJoinRoomInfo(
        roomID: roomID,
        tableID: tableID,
        userID: userID
    );
    
    StartCoroutine(StartGameWithDelay(joinInfo));
}

IEnumerator StartGameWithDelay(JoinRoomInfo joinInfo) {
    yield return new WaitForSeconds(1f);  // Wait for initialization
    
    // Start Fish_3D game directly
    FishNetAPI.Instance.Notifiy(SysEventType.StartGame, joinInfo);
    
    Debug.Log($"Started Fish_3D: Room {joinInfo.RoomID}, Table {joinInfo.TableID}, User {RoleInfoModel.Instance.Self.UserID}");
}
```

---

## 📊 **9. DATA REQUIREMENTS SUMMARY**

### **BẮT BUỘC (Critical):**
- ✅ `JoinRoomInfo.RoomID` (901 default)
- ✅ `JoinRoomInfo.TableID` (1 default)  
- ✅ `JoinRoomInfo.Seat` (0 default)
- ✅ `RoleInfoModel.Instance.Self.UserID` (generated)
- ✅ `RoleInfoModel.Instance.Self.NickName` (generated)
- ✅ `RoleInfoModel.Instance.Self.GoldNum` (1000000 default)

### **QUAN TRỌNG (Important):**
- ⚠️ `ServerLauncherTypeID` (calculated from room config)
- ⚠️ `RateValue` (1 default)
- ⚠️ `playerJoinArray` (at least self)
- ⚠️ `buffCacheList` (4 empty arrays)

### **TÙY CHỌN (Optional):**
- 📝 `BackgroundImage` (1 default)
- 📝 `HeroData` (empty array OK)
- 📝 `mXiuYuQiEndTime` (0 = no limit)
- 📝 Multiple players/bots

**→ Với minimal setup, chỉ cần ~10 fields để Fish_3D chạy được!**