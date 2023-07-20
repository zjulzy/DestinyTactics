# DestinyTactics
基于unity的简易战棋游戏demo
## 项目介绍
本项目是一个基于unity的简易战棋游戏demo，主要是为了学习unity和C#编程入门开发的，目前只有简单的战斗系统，后续可能会加入更多的内容。  
目前只支持玩家和AI对战，只有一个关卡  
## 项目实现
1. 游戏核心系统
2. 寻路算法
主要通过BFS和A*算法进行寻路，其中BFS负责寻找可移动范围和可攻击范围u，A*算法负责给出起点终点后寻找最短路径。  
## 未来工作
- [ ] 丰富UI，可以显示棋子具体信息
- [ ] 优化关卡美术，为不同地形的cell添加纹理
- [ ] 实现character的动画系统
- [ ] 完成character血条显示