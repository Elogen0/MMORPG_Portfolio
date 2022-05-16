using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Kame.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue _selectDialogue = null;
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _nodeStyleOn;
        [NonSerialized] private GUIStyle _choiceNodeStyle;
        [NonSerialized] private GUIStyle _choiceNodeStyleOn;
        
        //MoveNode
        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private Vector2 _draggingNodeOffset;
        
        //Crate and Delete Node
        [NonSerialized] private DialogueNode _creatingNode = null;
        [NonSerialized] private DialogueNode _deletingNode = null;
        [NonSerialized] private DialogueNode _linkingParentNode = null;

        [NonSerialized] private DialogueNode _editingNode =null;
        //DragCanvas
        private Vector2 _scrollPosition;
        [NonSerialized] private bool _draggingCanvas = false;
        [NonSerialized] private Vector2 _draggingCanvasOffset;

        private const float DEFAULT_CANVAS_SIZE = 4000f;
        private const float BACKGROUND_TEXTURE_SIZE = 50f;
        private Texture2D _backgroundTexure;
        private Texture2D _arrowTexture;
        enum MouseEvent
        {
            NONE,
            L_DOWN,
            L_DRAG,
            L_UP,
            R_DOWN,
            R_DRAG,
            R_UP,
            C_DOWN,
            C_DRAG,
            C_UP,
        }
        private MouseEvent GetMouseEvent()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                return MouseEvent.L_DOWN;
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                return MouseEvent.L_DRAG;
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                return MouseEvent.L_UP;
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                return MouseEvent.R_DOWN;
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 1)
                return MouseEvent.R_DRAG;
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
                return MouseEvent.R_UP;
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
                return MouseEvent.C_DOWN;
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
                return MouseEvent.C_DRAG;
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 2)
                return MouseEvent.C_UP;
            
            return MouseEvent.NONE;
        }
        
        
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (!dialogue)
                return false;
            
            ShowEditorWindow();
            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
            
            _nodeStyle = BuildNodeStyle("node0");
            _nodeStyleOn = BuildNodeStyle("node0 on");
            _choiceNodeStyle = BuildNodeStyle("node1");
            _choiceNodeStyleOn = BuildNodeStyle("node1 on");

            _backgroundTexure = Resources.Load("dialogue_editor_background") as Texture2D;
            _arrowTexture = Resources.Load("arrow_icon") as Texture2D;
        }

        private GUIStyle BuildNodeStyle(string styleName)
        {
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load(styleName) as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 5, 35);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            return nodeStyle;
        }

        private void OnSelectionChange()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (!newDialogue)
                return;

            _selectDialogue = newDialogue;
            Repaint();
        }

        private void OnGUI()
        {
            if (!_selectDialogue)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                {
                    Rect canvas = GUILayoutUtility.GetRect(DEFAULT_CANVAS_SIZE, DEFAULT_CANVAS_SIZE);
                    Rect textureCoords = new Rect(0, 0,
                        DEFAULT_CANVAS_SIZE / BACKGROUND_TEXTURE_SIZE,
                        DEFAULT_CANVAS_SIZE / BACKGROUND_TEXTURE_SIZE);
                    GUI.DrawTextureWithTexCoords(canvas, _backgroundTexure, textureCoords);

                    foreach (DialogueNode node in _selectDialogue.GetAllNodes())
                    {
                        DrawConnections(node);
                    }
                    foreach (DialogueNode node in _selectDialogue.GetAllNodes())
                    {
                        DrawNode(node);
                    }
                }    EditorGUILayout.EndScrollView();

                if (_creatingNode != null)
                {
                    _selectDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }
                if (_deletingNode != null)
                {
                    _selectDialogue.DeleteNode(_deletingNode);
                    _deletingNode = null;
                }
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style;
            if (node.IsChoiceNode)
            {
                style = node == _editingNode ? _choiceNodeStyleOn : _choiceNodeStyle;
            }
            else
            {
                style = node == _editingNode ? _nodeStyleOn : _nodeStyle;
            }
            
            GUILayout.BeginArea(node.GetRect(), style);
            {
                GUILayoutOption[] labelOptions = new[]
                {
                    GUILayout.Width(node.GetRect().width - (_nodeStyle.padding.right + _nodeStyle.padding.left)),
                    GUILayout.Height(node.GetRect().height - (_nodeStyle.padding.top + _nodeStyle.padding.bottom + 20f) ),
                };
                GUIStyle textStyle = EditorStyles.label;
                textStyle.wordWrap = true;
                EditorGUILayout.LabelField(node.SpeakerName,EditorStyles.boldLabel);
                EditorGUILayout.LabelField(node.Sentence, labelOptions);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("x"))
                    {
                        _deletingNode = node;
                    }
                    DrawLinkButtons(node);
                    if (GUILayout.Button("+"))
                    {
                        _creatingNode = node;
                    }
                } GUILayout.EndHorizontal();

            } GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    _linkingParentNode = node;
                }
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode.Children.Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    _linkingParentNode.AddChild(node.name);
                    _linkingParentNode = null;
                }
            }
            
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in _selectDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
                float size = 10f;
                GUI.DrawTexture(new Rect(endPosition.x - size/2, endPosition.y - size/2, size,size), _arrowTexture, ScaleMode.StretchToFill);
            }
        }

        private void ProcessEvents()
        {
            if (GetMouseEvent() == MouseEvent.L_DOWN && _draggingNode == null) //노드 선택
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode != null)
                {
                    _draggingNodeOffset = _draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                    _editingNode = _draggingNode;
                }
                else
                {
                    Selection.activeObject = _selectDialogue;
                    _editingNode = null;
                }

                GUI.changed = true;
            }
            else if (GetMouseEvent() == MouseEvent.L_DRAG && _draggingNode) //노드 드래그
            {
                _draggingNode.SetPosition(Event.current.mousePosition + _draggingNodeOffset);
                GUI.changed = true;
            }
            else if (GetMouseEvent() == MouseEvent.L_UP && _draggingNode)
            {
                _draggingNode = null;
            }
            else if (GetMouseEvent() == MouseEvent.C_DOWN) //캔버스 선택
            {
                _draggingCanvas = true;
                _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
            }
            else if (GetMouseEvent() == MouseEvent.C_DRAG && _draggingCanvas) //캔버스 드래그
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (GetMouseEvent() == MouseEvent.C_UP && _draggingCanvas)
            {
                _draggingCanvas = false;
            }

        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in _selectDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        [ContextMenu("Test")]
        public void Test()
        {
            Debug.Log("Test");
        }
    }    
    
}

