# 多自由度磁流体抛光机上位机控制系统模拟器

## 项目简介

这是一个基于 **C# WinForm** 开发的**自动化设备运动控制上位机模拟系统**，专为高精度多轴设备（如多自由度磁流体抛光机）设计。

主要用于模拟工业场景下的运动控制逻辑，为学习和演示上位机开发提供完整示例。

## 核心功能

- ✅ **6轴运动控制**（X、Y、Z、A、B、C）
- ✅ **JOG点动**（支持正反向变速）
- ✅ **定位运动**（PTP点到点）
- ✅ **轴使能 / 禁用** + 状态监控
- ✅ **实时位置反馈**（Timer刷新）
- ✅ **模拟运动控制卡**（WJ_API / 固高风格）
- ✅ **急停功能**
- ✅ 模拟模式与真实硬件DLL无缝切换

## 技术栈

- **语言**：C# 
- **框架**：.NET 8 + Windows Forms
- **架构**：Simulator 模式 + 真实DLL调用
- **核心类**：`MotionControllerSimulator.cs`、`WJ_API_Simulator.cs`

## 项目结构
用户界面层 (Windows Forms)
    ↓
业务逻辑层 (Form1.cs - 运动控制逻辑)
    ↓
硬件抽象层 (WJ_API.cs - P/Invoke 封装)
    ↓
运动控制卡 (WJ_API.dll)
    ↓
驱动器 → 电机 → 机械结构
<img width="1398" height="1004" alt="image" src="https://github.com/user-attachments/assets/41969178-e8f4-43ae-8e05-d9703f6f1f20" />
