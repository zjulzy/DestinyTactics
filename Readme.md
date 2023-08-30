# DestinyTactics
基于unity的简易战棋游戏demo
## 项目介绍
本项目是一个基于unity的简易战棋游戏demo，主要是为了学习unity和C#编程入门开发的，目前只有简单的战斗系统，后续可能会加入更多的内容。  
目前只支持玩家和AI对战，只有一个关卡  

[//]: # (![]&#40;image.jpg&#41;)
[//]: # (<iframe height=498 width=510 src="./video.mkv">  )
https://github.com/zjulzy/DestinyTactics/assets/34120274/28f2b454-8ca6-45aa-a02b-1e6b5944537f

## 项目实现
1. 游戏核心系统
游戏中的棋盘通过邻接矩阵存储图的数据结构,并通过各个各自的几何关系初始化.
2. 寻路算法
主要通过BFS和A*算法进行寻路，其中BFS负责寻找可移动范围和可攻击范围u，A*算法负责给出起点终点后寻找最短路径。 
3. 角色能力和属性管理
实现了一个简易的能力系统用来实现各种技能和伤害计算,以及和动画系统的联动.  

## 未来工作

-[x] UI系统
  -[x] 玩家胜利和失败ui
  -[x] 简易的信息记录系统
  -[ ] 优化ui，通过右键点击实现退后
  -[x] 完成character血条显示
  -[x] 丰富UI，可以显示棋子具体信息
  
-[x] 地形格子系统
  -[x] 添加河流山脉等阻碍和其他需要不同ap的地形
  -[x] 优化关卡美术，为不同地形的cell添加纹理
  -[x] 实现网格显示效果
  -[x] 重构部分gridsystem的功能到cell和character中
  
-[x] 角色与能力系统
  -[x] 实现character的动画系统,攻击和受击动画
  -[x] 玩家棋子增加更多能力

-[] AI系统
  -[] 优化AI索敌系统



