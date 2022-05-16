namespace Kame.Define
{
    public enum StatType
    {
        None = -1,
        HP = 10,
        MP = 20,
        STEMINA = 50,
        ATK = 60,
        ATK_RANGE = 70,
        ATK_SPEED = 80,
        DEF = 90,
        MOVE_SPEED = 100,
    }
    
    public enum EffectType
    {
        None = -1,
        NORMAL,
        MaxCount,
    }

    public enum SoundPlayType
    {
        None = -1,
        Normal,
        BGM,
        Effect,
        UI,
        MaxCount
    }
    
    public enum ItemGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }
    
    
    public enum CursorType
    {
        None = -1,
        Movement,
        CantMove,
        Combat,
        UI,
        PickUp,
        FullPickup,
        Dialogue,
    }
    
    enum CharacterWanted
    {
        None,
        Attack,
        Interact,
    }
    
    public enum UIEvent
    {
        Click,
        Drag,
    }
    
    public enum GameEntityType
    {
        None = -1,
        Player = 0,
        OtherPlayer = 1,
        Friend = 2,
        #region Objects
        Object = 10000000,
        #endregion
        #region NPC
        NPC = 20000000,
        #endregion
        #region Monster
        Monster = 30000000,
        #endregion
    }
    
    #region //Tool Defines 
    public enum EventStartType { INTERACT, AUTOSTART, TRIGGER_ENTER, TRIGGER_EXIT, NONE, KEY_PRESS, DROP };
    public enum AIConditionNeeded { ALL, ONE };
    public enum ValueCheck { EQUALS, LESS, GREATER };
    public enum SimpleOperator { ADD, SUB, SET };
    #endregion
}
