# DestinyTactics
基于unity的简易战棋游戏demo
## 项目介绍
本项目是一个基于unity的简易战棋游戏demo，主要是为了学习unity和C#编程入门开发的，目前只有简单的战斗系统，后续可能会加入更多的内容。  
目前只支持玩家和AI对战，只有一个关卡  
![](image.png)
## 项目实现
1. 游戏核心系统
游戏中的棋盘通过
2. 寻路算法
主要通过BFS和A*算法进行寻路，其中BFS负责寻找可移动范围和可攻击范围u，A*算法负责给出起点终点后寻找最短路径。  
## 未来工作
- [x] 丰富UI，可以显示棋子具体信息
- [ ] 添加河流山脉等阻碍和其他需要不同ap的地形
- [ ] 优化关卡美术，为不同地形的cell添加纹理
- [ ] 实现character的动画系统,攻击和受击动画
- [x] 完成character血条显示
- [ ] 重构部分gridsystem的功能到cell和character中
- [x] 玩家胜利和失败ui
- [ ] 玩家棋子增加更多能力
- [x] 简易的信息记录系统
- [x] 实现网格显示效果