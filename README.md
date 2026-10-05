# A Dog_Shadow Dies a Billion Times

Unity 下的 **ARPG 动作游戏原型**：完整的玩家角色状态机、连招战斗、敌人 AI、技能与血量系统，并配合场景流程（对话 / NPC / 传送 / Boss 登场）串成可玩的流程。

角色与敌人共用同一套 **FSM 状态机框架**，玩家行为按「地面 / 空中 / 战斗 / 受击 / 技能」分模块组织，敌人则通过各自的行为机（弓箭手 / 战士）实现差异化 AI。

## 技术栈

| 分类 | 技术 |
|---|---|
| 引擎 | Unity 2022.3.62f1c1 |
| 渲染管线 | URP 14.0.12（依赖中同时包含 HDRP 14.0.12） |
| 输入 | Unity Input System 1.14.0 |
| 相机 | Cinemachine 2.10.5 |
| 导航 | AI Navigation 1.1.7 |
| 其他 | Timeline、TextMeshPro、UGUI |

## 核心系统

### 角色状态机（FSM）
- `FSM.cs` / `StateMachine.cs` / `BaseBehaviour.cs`：通用状态机骨架，`BehaviourStatus.cs` 描述状态。
- **组件化**：`Components/` 下把能力拆成可复用组件 —— `Combat`（战斗）、`Effect`（特效）、`Float`（浮空）、`GroundCheck`（地面检测）。

### 玩家行为
`PlayerBehaviourMachine` 统一调度，行为按模块拆分：

| 模块 | 覆盖内容 |
|---|---|
| `PlayerGroundBehaviours` | 地面行为：待机、起步、奔跑、停止、闪避、冲刺 |
| `PlayerAirBehaviours` | 空中行为：跳跃、下落、落地 |
| `PlayerCombatBehaviours` | 战斗行为：攻击与连招 |
| `PlayerHitBehaviours` | 受击反馈 |
| `PlayerSkillBehaviours` | 技能释放 |

配套：`PlayerAnimationCache`（动画状态缓存，避免每帧字符串查找）、`FootIK`（足部 IK，贴地）、`PlayerVision`（视野）、`PlayerInputs`（输入）。

### 连招与战斗数据
`Status/CombatData/` 下以数据描述战斗行为，与逻辑解耦：

- `NormalCombos` — 地面普攻连段
- `AirCombos` — 空中连段
- `SprintCombo` — 冲刺攻击
- `Block` — 格挡
- `Dodge` — 闪避
- `Skill` — 技能
- `Damage` — 伤害结算

角色属性由 `HealthSystem`（血量）与 `SkillSystem`（技能）承担；`DamageCollider` + `CapsuleColliderUtility` 负责攻击判定体的时序控制。

### 敌人 AI
- `ArcherBehaviourMachine` / `ArcherBehaviours`：弓箭手 —— 远程攻击行为。
- `WarriorBehaviourMachine` / `WarriorBehaviours`：战士 —— 近战行为。
- `EnemyVision`：敌人视野检测，用于索敌与进入战斗状态。
- `CharacterAI`：敌人的通用控制入口。

### 场景与流程
- `Scripts/` 提供完整的场景流程组件：`SceneManagerControl`（场景切换）、`NPC`、`DialogPanel`（对话）、`Teleporter`（传送）、`TipManager` / `AutoTip`（提示）、`SettingManager`（设置）、`Sound`（音频）、`InteractHide`（交互隐藏）、`Singleton`（全局单例基类）。
- 游戏场景：`GameScenes/Start.unity` → `BattleField_1.unity` → `BattleField_2.unity`。
- Boss 战：`SceneProject/BossDebut/`（`BossDebut.cs`、`BossRoomEntry.cs`）。
- 角色调试场景：`Character/Scenes/Lab.unity`。

## 项目结构

```
Assets/
├── Character/Script/
│   ├── Character/
│   │   ├── FSM/                    # 状态机骨架 + Combat/Effect/Float/GroundCheck 组件
│   │   ├── Player/                 # 玩家行为机、FootIK、动画缓存、视野
│   │   ├── Enemy/                  # 弓箭手 / 战士行为机与行为实现、EnemyVision
│   │   ├── CharacterSystem/        # HealthSystem、SkillSystem
│   │   ├── Collider/               # DamageCollider、CapsuleColliderUtility
│   │   ├── Status/CombatData/      # 连招、格挡、闪避、技能、伤害数据
│   │   ├── ControlIns/             # CharacterAI、PlayerInputs
│   │   └── Effect/                 # 命中 / 销毁特效
│   └── Camera/PlayerCamera.cs
├── Scripts/                        # 场景流程（NPC、对话、传送、提示、设置、音频）
├── SceneProject/BossDebut/         # Boss 登场
├── GameScenes/                     # Start / BattleField_1 / BattleField_2
├── Character/Scenes/Lab.unity      # 角色调试场景
└── Resources/ · Art/ · Effects/    # 美术与特效资源
```

## 快速开始

1. 用 **Unity 2022.3.62f1c1** 打开项目，首次打开等待依赖还原与资源导入。
2. 打开 `Assets/GameScenes/Start.unity` 从头开始流程。
3. 单独调试角色与战斗手感：打开 `Assets/Character/Scenes/Lab.unity`。
4. 调整连招与手感：查看 `Assets/Character/Script/Character/Status/CombatData/` 下的数据类。

## 备注

- 仓库体积较大，包含多套第三方美术与特效资源（ARPG Effects、KriptoFX Volumetric BloodFX、Drakkar Trail、GhostSamurai Animset 等），**版权归各自作者所有**，仅供学习与原型验证使用，请勿商用。
- 项目进度与待办见 [TODO.md](./TODO.md)。
