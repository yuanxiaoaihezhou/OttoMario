# OttoMario

一个使用 MonoGame 制作的超级马里奥风格平台游戏。

## 功能特性

- ✅ 完整的游戏系统（移动、跳跃、物理引擎）
- ✅ 丰富的游戏元素（方块、敌人、道具）
- ✅ 完整的UI系统（主菜单、HUD、暂停菜单、游戏结束画面）
- ✅ 机器生成的纹理和精灵（可替换）
- ✅ 内置关卡编辑器（支持保存/加载/测试）
- ✅ GitHub Actions自动构建Win11 AMD64版本

## 游戏玩法

### 控制方式

**游戏中：**
- `A` / `Left` - 向左移动
- `D` / `Right` - 向右移动
- `Space` / `Up` / `W` - 跳跃
- `Escape` - 暂停/返回

**关卡编辑器：**
- `1-6` - 选择不同类型的瓷砖（地面、砖块、问号方块、Goomba、Koopa、旗帜）
- `I` - 切换问号方块的道具类型（金币/蘑菇）
- 鼠标左键 - 放置瓷砖
- 鼠标右键 - 删除瓷砖
- 方向键 - 移动相机
- `Ctrl+S` - 保存关卡
- `Ctrl+O` - 加载关卡
- `T` - 测试当前关卡
- `Escape` - 返回主菜单

### 游戏元素

- **玩家（Mario）**: 可以移动、跳跃，收集金币和道具
- **地面方块**: 实心平台
- **砖块**: 可以被大马里奥从下方打破
- **问号方块**: 撞击后可获得金币或蘑菇
- **Goomba**: 棕色敌人，踩踏可消灭
- **Koopa**: 绿色敌人，踩踏可消灭
- **金币**: 收集100个获得一条生命
- **蘑菇**: 变身为大马里奥，可打破砖块
- **旗帜**: 关卡目标，碰触即通关

## 系统要求

- .NET 9.0 SDK
- Windows 11 (AMD64)
- 或任何支持 MonoGame 的平台

## 构建和运行

### 开发环境

```bash
# 克隆仓库
git clone https://github.com/yuanxiaoaihezhou/OttoMario.git
cd OttoMario

# 安装 MonoGame 模板
dotnet new install MonoGame.Templates.CSharp

# 构建项目
cd OttoMario
dotnet build

# 运行游戏
dotnet run
```

### 发布版本

```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 输出位置: bin/Release/net9.0/win-x64/publish/
```

## 自动发布

项目配置了 GitHub Actions 自动构建流程：

1. 推送带有 `v*` 标签的提交会触发自动构建
2. 或者通过 Actions 页面手动触发工作流
3. 构建完成后，会自动创建 Release 并上传 Windows x64 版本

### 创建发布版本

```bash
git tag v1.0.0
git push origin v1.0.0
```

## 项目结构

```
OttoMario/
├── Core/                    # 核心系统
│   ├── GameStateManager.cs # 游戏状态管理
│   └── TextureGenerator.cs # 程序化纹理生成
├── Entities/                # 游戏实体
│   ├── Entity.cs           # 基础实体类
│   ├── Player.cs           # 玩家
│   ├── Enemy.cs            # 敌人
│   ├── Block.cs            # 方块和道具
│   └── Flag.cs             # 旗帜
├── Levels/                  # 关卡系统
│   ├── LevelData.cs        # 关卡数据结构
│   └── Level.cs            # 关卡逻辑
├── Systems/                 # 游戏系统
│   ├── Camera.cs           # 相机系统
│   └── LevelEditor.cs      # 关卡编辑器
├── UI/                      # 用户界面
│   └── UIComponents.cs     # UI组件（菜单、HUD等）
└── Game1.cs                # 主游戏类
```

## 素材替换

游戏使用程序化生成的纹理，便于替换。要使用自定义素材：

1. 在 `TextureGenerator.cs` 中找到对应的纹理生成方法
2. 将其替换为 `Content.Load<Texture2D>("your_texture_name")`
3. 将你的纹理文件添加到 `Content` 文件夹
4. 在 `Content.mgcb` 中添加纹理引用

### 纹理列表

- `Player` - 马里奥角色 (32x32)
- `GroundBlock` - 地面方块 (32x32)
- `BrickBlock` - 砖块 (32x32)
- `QuestionBlock` - 问号方块 (32x32)
- `Goomba` - Goomba敌人 (32x32)
- `Koopa` - Koopa敌人 (32x32)
- `Coin` - 金币 (24x24)
- `Mushroom` - 蘑菇道具 (32x32)
- `Flag` - 旗帜 (16x64)

## 关卡编辑器

游戏内置关卡编辑器，支持：

- 网格化编辑
- 多种瓷砖类型
- 实时预览
- JSON格式保存/加载
- 测试模式

### 保存格式

关卡保存为 JSON 格式，包含：
- 关卡尺寸
- 玩家起始位置
- 所有实体的类型和位置
- 背景颜色
- 时间限制

## 贡献

欢迎提交 Issue 和 Pull Request！

## 许可证

MIT License

## 致谢

- MonoGame 框架
- 超级马里奥系列游戏（灵感来源）
