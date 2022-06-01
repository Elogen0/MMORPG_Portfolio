# MMORPG-Portfolio 요약

### 동영상 링크
[![Video](https://img.youtube.com/vi/D5QFlw5yei8/0.jpg)](https://youtu.be/D5QFlw5yei8)

### 소개
2년간 SI개발자로 일하였고, 현재 독학으로 게임 서버 개발자로 이직 준비중입니다.

### 개발 철학
클라이언트
> 1. 하드코딩을 철저히 배제하고 객체지향적인 설계로 유지보수와 확장성을 고려하였다.
> 
> 2. 수정 시 각 시스템간 영향이 최소화 되도록 각 모듈간 의존성을 줄였다.
> - 의존성을 깨기위해 Observer 패턴을 적극 활용하였다. 특히 control 부분에서는 view단을 전혀 모르도록 설계하였다.
> 
> 3. 각 파트간 업무 분담을 할 수 있도록 각 기능을 모듈화하여 에디터 기능으로 빼놓아 생산성 증진을 도모하였다.
> - 다형성의 장점을 살릴 수 있는 Command 패턴, Statergy 패턴과 에디터상에서 편집가능한 Scriptable Object기능을 이용하여 최소한의 코딩으로 컨텐츠를 생산할 수 있도록 하였다
> 
> 5. 쾌적한 게임을 즐길 수 있도록 최적화와 비동기 프로그래밍을 진행하였다.
서버
> 1. 유저가 메모리변조로 게임의 생태계를 망칠수 없도록 영향이 큰 부분은 서버에서 최종검증하게 하였다.
> 2. 각 스레드에서 동시에 들어오는 작업들을 Priority Queue를 통해 순차적으로 실행하도록 하였다. 이 과정에서 Push하는 부분만 lock을 걸도록 하였다
> 3. 그럼에도 Data Race가 생기는 부분은 Interlocked를 통해 접근을 통제하였다.
> 4. 클라이언트 메모리 변조를 통한 게임 생태계를 망치는것을 막기위해, 영향력이 큰 부분은 서버에서 검증하도록 하였다.

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
