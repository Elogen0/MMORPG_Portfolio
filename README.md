# MMORPG-Portfolio 요약

### 동영상 링크
[![Video](https://img.youtube.com/vi/D5QFlw5yei8/0.jpg)](https://youtu.be/D5QFlw5yei8)
------------------

### 소개
> 2년간 SI개발자로 일하였고, 현재 독학으로 게임 서버 개발자로 이직 준비중입니다.  
> 코드리뷰를 통한 올바른 개발 방향을 잡아 주면서 성장할 수 있는 회사를 찾고 있습니다.  
> 다른 업계로 이직하는 것인 만큼 신입이라 생각하고 겸손한 자세로 배우겠습니다.  
> 반복업무, 신규 기능 개발 가릴것 없이 '프로젝트의 완수가 곧 나의 성공'이라는 마인드로 프로젝트의 완수를위해 열심히 뛰겠습니다.

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

### 개발 방향
#### 클라이언트
> - 하드코딩을 철저히 배제하고 객체지향적인 설계로 유지보수와 확장성을 고려하였습니다.
>   - Stat System 살펴보기 : 
>   - Stat은 체력, 마나, 스피드 등 각각의 필드가 존재하는 것이 아닌, Map안의 Modifier클래스로 정의되어 새로운 스탯을 넣고 싶다면 코드 전체를 수정하는 것이 아닌 Enum값 추가로 간단히 해결 가능합니다.
> - 수정 시 각 시스템간 영향이 최소화 되도록 각 모듈간 의존성을 줄였습니다.
>   - Quest System 살펴보기 :
>   - Observer 패턴을 적극 활용하여 클래스간의 의존성을 줄였습니다. 특히 control단에서는 view단을 전혀 모르도록 설계하였습니다.
>   - 다른 시스템끼리는 Id를 통해 서로의 기능을 모르고도 특정 기능을 수행할수 있도록 하였습니다.
> - 각 파트간 업무 분담을 할 수 있도록 각 기능을 모듈화하여 에디터 기능으로 빼놓아 생산성 증진을 도모하였습니다.
>   - Ability System 살펴보기
>   - 다형성의 장점을 살릴 수 있는 Command 패턴, Statergy 패턴과 에디터상에서 편집가능한 Scriptable Object기능을 이용하여 최소한의 코딩으로 컨텐츠를 생산할 수 있도록 하였습니다
>   
> - 쾌적한 게임을 즐길 수 있도록 최적화와 비동기 프로그래밍을 진행하였습니다.
>   - 예) AddressablePooling : 

#### 서버
> - DB와 Logic Thread를 따로 분리, I/O로 인한 Device Time이 전체적인 서버에 영향이 가지 않도록 설계하였습니다.
>   - 예) Main : 
> - lock의 범위 : 각 소켓을 통해 들어오는 패킷을 Priority Queue를 통해 Serialize하여 순차적으로 실행하도록 하였다. 이 과정에서 Queue에 Push하는 부분만 lock을 걸었습니다.
>   - 예) Room :
> - Data Race가 생기는 부분은 Interlocked를 통해 접근을 통제하여 비정상적인 작동을 막았습니다.
>   - 예) Inventory System :
> - 클라이언트 메모리 변조를 통한 게임 생태계를 망치는것을 막기위해, 영향력이 큰 부분은 서버에서 검증하도록 하였습니다.
>   - 예) Battle System :
> - 대량의 클라이언트를 수용하기 위해 영역에 따라 패킷을 Broadcast하는 범위를 한정하고, 범위 외의 오브젝트들은 클라이언트에서 Desapwn 처리되게 하여 서버가 처리해야할 복잡도를 줄였습니다.
>   - 예) View :

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
