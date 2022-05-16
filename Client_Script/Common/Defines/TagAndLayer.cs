using UnityEngine;

public class TagAndLayer
{
    public class LayerName
    {
        public const string Default = "Default";
        public const string TransparentFX = "TransparentFX";
        public const string IgnoreRayCast = "Ignore Raycast";
        public const string Water = "Water";
        public const string UI = "UI";
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Ally = "Ally";
        public const string Cover = "Cover";
        public const string IgnoreShot = "Ignore Shot";
        public const string CoverInvisible = "Cover Invisible";
        public const string Bound = "Bound";
        public const string Environment = "Environment";
        public const string Ground = "Ground";
    }
    
    public class LayerIndex
    {
        public static readonly int Default = LayerMask.NameToLayer(LayerName.Default);
        public static readonly int TransparentFX = LayerMask.NameToLayer(LayerName.TransparentFX);
        public static readonly int IgnoreRayCast = LayerMask.NameToLayer(LayerName.IgnoreRayCast);
        public static readonly int Water = LayerMask.NameToLayer(LayerName.Water);
        public static readonly int UI = LayerMask.NameToLayer(LayerName.UI);
        public static readonly int Player = LayerMask.NameToLayer(LayerName.Player);
        public static readonly int Enemy =  LayerMask.NameToLayer(LayerName.Enemy);
        public static readonly int Ally =  LayerMask.NameToLayer(LayerName.Ally);
        public static readonly int Ground =  LayerMask.NameToLayer(LayerName.Ground);
    }

    public class LayerMasking
    {
        public static readonly int Default = 1 << LayerMask.NameToLayer(LayerName.Default);
        public static readonly int TransparentFX = 1 << LayerMask.NameToLayer(LayerName.TransparentFX);
        public static readonly int Water = 1 << LayerMask.NameToLayer(LayerName.Water);
        public static readonly int UI = 1 << LayerMask.NameToLayer(LayerName.UI);
        public static readonly int Player = 1 << LayerMask.NameToLayer(LayerName.Player);
        public static readonly int Enemy = 1 << LayerMask.NameToLayer(LayerName.Enemy);
        public static readonly int Ally = 1 << LayerMask.NameToLayer(LayerName.Ally);
        public static readonly int Ground = 1 << LayerMask.NameToLayer(LayerName.Ground);
    }
    
    public class TagName
    {
        public const string Untagged = "Untagged";
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string GameController = "GameController";
        public const string Finish = "Finish";
        public const string ViewContents = "ViewContents";
    }
    
}
