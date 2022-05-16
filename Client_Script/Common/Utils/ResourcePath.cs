namespace Kame.Define
{
    public static class ResourcePath
    {
        //--------------------------Depth1--------------------
        public const string ScriptableObject = "ScriptableObjects/";
        public const string Prefab = "Prefabs/";
        public const string UI = "UI/";
        public const string Data = "Data/";
        public const string Manager = "Managers/";
        
        //--------------------------Depth2-------------------
        public const string Mediator = ScriptableObject + "Mediators/";
        public const string Ability = ScriptableObject + "Abilities/";
        public const string AI = ScriptableObject + "AI/";
        public const string Dialogue = Data + "Dialogues/";
        public const string Item = Data + "Items/";
        public const string EventChannels = ScriptableObject + "EventChannels/";
        public const string RuntimeAnchors = ScriptableObject + "RuntimeAnchors/";

        public const string DialogueManager = Manager + "DialogueManager";
        public const string QuestManager = Manager + "QuestManager";
        //--------------------------Depth3-------------------
        public const string EventUI = EventChannels + "UI/";
        public const string EventLogic = EventChannels + "Logic/";
        
        //-------------------------Depth4--------------------
        public const string UIViewEventChannel = EventUI + "UIViewEventChannel";
        public const string CameraShake = EventLogic + "CameraShake";
        public const string PlayerInstantiate = EventLogic + "PlayerInstantiated";
        public const string ControlLock = EventLogic + "ControlLock";
        public const string ShopChange = EventLogic + "ShopChange";
        public const string ConversationUpdated = EventLogic + "ConversationUpdated"; 
            
        public const string CameraTransformAnchor = RuntimeAnchors + "GamePlayCameraTransform";
        public const string PlayerTransformAnchor = RuntimeAnchors + "PlayerTransform";
        public const string AudioListenerAnchor = RuntimeAnchors + "AudioListenerTransform";
         
    }
    public static class AssetPath
    {
        //Depth 1
        public const string Asset = "Assets/";
        
        //Depth 2
        public const string Resource = Asset + "Resources/";
        public const string Script = Asset + "Scripts/";
        public const string Prefab = Asset + "Prefabs/";
        public const string ScriptableObject = Asset + "ScriptableObjects/";
        public const string Scene = Asset + "Scenes/";
        
        //3
        public const string Define = Script + "Common/Defines/";
        public const string Mediator = ScriptableObject + "Mediators/";
        public const string Ability = Prefab + "Abilities/";
        public const string Placements = Prefab + "Placements/";
        public const string Props = Prefab + "Props/";
        public const string UI = Prefab + "UI/";
        public const string Dialogue = ScriptableObject + "Dialogues/";
        
        //4
        public const string UI_Popup = UI + "Popup/";
        public const string UI_Scene = UI + "Scene/";
    }
}
