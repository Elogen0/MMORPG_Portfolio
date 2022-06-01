# MMORPG-Portfolio 요약

### 동영상 링크
[![Video](https://img.youtube.com/vi/D5QFlw5yei8/0.jpg)](https://youtu.be/D5QFlw5yei8)

### 소개
2년간 SI개발자로 일하며 독학으로

### 개발 철학
> 1. 하드코딩을 철저히 배제하고 객체지향적인 설계로 유지보수와 확장성을 고려하였다.
> 
> 2. 수정 시 각 시스템간 영향이 최소화 되도록 각 모듈간 의존성을 줄였다.
> - 의존성을 깨기위해 Observer 패턴을 적극 활용하였다. 특히 control 부분에서는 view단을
> 
> 3. 각 파트간 업무 분담을 할 수 있도록 각 기능을 모듈화하여 에디터 기능으로 빼놓아 생산성 증진을 도모하였다.
> - Command 패턴, Statergy 패턴을 통해 각자의 기능을 
> 4. 유저가 메모리변조로 게임의 생태계를 망칠수 없도록 영향이 큰 부분은 서버에서 최종검증하게 하였다.
> 
> 5. 최대한 쾌적한 게임을 즐길 수 있도록 최적화를 

### 역할
전체 제작

### 사용기술
#### Unity Client
> + Addressables
> + Cinemachine
> + NPOI(Editor)

#### Game Server
> + C# Server(TCP)
> + .Net core Entity Framework (DB)
> + .Net core MVC (Web Server)
> + Protocol buffers(Packet)

### 구현 로직
> + Resource Pooling
> + Dialogue System
> + Quest System
> + Sound System
> + Inventory System
> + Stat System
> + Ability System
> + A.I.(State Machine)
> + Data Management

### Tool 제작
> + Map Tool
> + Dialogue Tool
> + Skill Tool
> + Quest Tool
> + Sound Tool
> + Object Tool (Excel)
> + Stat Tool (Excel)
> + Item Tool (Excel)

## UML
### Server Core Sequence Diagram
![ServerSequenceDiagram](https://user-images.githubusercontent.com/95978503/168649220-39a18236-662c-46fe-9e9e-973d82ebbe9a.jpg)
### Server Core Class Diagram
![Server Class Diagram](https://user-images.githubusercontent.com/95978503/168650628-7d8d1d75-687b-4b56-82aa-a935ec7dfe0e.jpg)
### Quest And Dialogue Class Diagram
![QuestAndDialogue Class Diagram](https://user-images.githubusercontent.com/95978503/168651421-27d62714-262c-4d95-86d0-c5693a6cdd05.jpg)
### Inventory System Class Diagram
![Inventory Class Diagram](https://user-images.githubusercontent.com/95978503/168651645-86eb172f-59bd-466a-b4ff-f71f05560f20.jpg)
