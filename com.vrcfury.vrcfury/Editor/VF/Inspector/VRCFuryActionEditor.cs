using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VF.Builder;
using VF.Model.StateAction;
using VRC.SDK3.Avatars.Components;

namespace VF.Inspector {

[CustomPropertyDrawer(typeof(VF.Model.StateAction.Action))]
public class VRCFuryActionDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty prop) {
        var el = new VisualElement();
        el.AddToClassList("vfAction");
        el.Add(Render(prop));
        return el;
    }

    private static VisualElement Render(SerializedProperty prop) {

        var type = VRCFuryEditorUtils.GetManagedReferenceTypeName(prop);

        switch (type) {
            case nameof(MaterialAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Material") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("obj"));
                propField.style.flexGrow = 1;
                row.Add(propField);
            
                var propField2 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("materialIndex"));
                propField2.style.flexGrow = 0;
                propField2.style.flexBasis = 50;
                row.Add(propField2);
            
                var propField3 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("mat"));
                propField3.style.flexGrow = 1;
                row.Add(propField3);

                return row;
            }
            case nameof(FlipbookAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Flipbook Frame") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("obj"));
                propField.style.flexGrow = 1;
                row.Add(propField);
            
                var propField3 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("frame"));
                propField3.style.flexGrow = 0;
                propField3.style.flexBasis = 30;
                row.Add(propField3);

                return row;
            }
            case nameof(ShaderInventoryAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Shader Inventory") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("renderer"));
                propField.style.flexGrow = 1;
                row.Add(propField);
            
                var propField3 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("slot"));
                propField3.style.flexGrow = 0;
                propField3.style.flexBasis = 30;
                row.Add(propField3);

                return row;
            }
            case nameof(MaterialPropertyAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Material Property") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };

                var col = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Column,
                        flexGrow = 1
                    }
                };
                
                row.Add(label);

                var rendererRow = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center,
                        marginBottom = 1
                    }
                };

                var affectAllMeshesProp = prop.FindPropertyRelative("affectAllMeshes");
                
                var rendererProp = prop.FindPropertyRelative("renderer");
                var propField = VRCFuryEditorUtils.Prop(rendererProp);
                propField.style.flexGrow = 0;
                propField.style.flexShrink = 1;
                propField.SetEnabled(!affectAllMeshesProp.boolValue);
                rendererRow.Add(propField);
                
                rendererRow.Add(new Label("All Meshes") {
                    style = {
                        marginLeft = 2,
                        marginRight = 2
                    }
                });
                
                var propField4 = VRCFuryEditorUtils.Prop(affectAllMeshesProp);
                propField4.style.flexGrow = 0;
                propField4.style.flexShrink = 0;
                propField4.style.flexBasis = 20;
                propField4.schedule.Execute(() => {
                    propField.SetEnabled(!affectAllMeshesProp.boolValue);
                }).Every(100);
                rendererRow.Add(propField4);
                
                col.Add(rendererRow);
                
                var materialRow = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };
            
                var propertyNameProp = prop.FindPropertyRelative("propertyName");
                var propField2 = VRCFuryEditorUtils.Prop(propertyNameProp);
                propField2.style.flexGrow = 1;
                propField2.tooltip = "Property Name";
                materialRow.Add(propField2);
                
                var propField3 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("value"));
                propField3.style.flexGrow = 0;
                propField3.style.flexBasis = 60;
                propField3.tooltip = "Property Value";
                materialRow.Add(propField3);

                var searchButton = new Button(SearchClick)
                {
                    text = "Search",
                    style =
                    {
                        marginTop = 0,
                        marginLeft = 0,
                        marginRight = 0,
                        marginBottom = 0
                    }
                };
                materialRow.Add(searchButton);
                col.Add(materialRow);
                
                row.Add(col);

                return row;

                void SearchClick() {
                    var targetWidth = row.GetFirstAncestorOfType<UnityEditor.UIElements.InspectorElement>().worldBound
                        .width;
                    var searchContext = new UnityEditor.Experimental.GraphView.SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), targetWidth, 300);
                    var provider = ScriptableObject.CreateInstance<VRCFuryMaterialPropertySearchWindow>();
                    var editorObject = prop.serializedObject.targetObject;
                    if (affectAllMeshesProp.boolValue) {
                        if (editorObject is UnityEngine.Component c) {
                            VFGameObject avatarObject = c.owner().GetComponentInSelfOrParent<VRCAvatarDescriptor>()?.owner();
                            if (avatarObject != null) {
                                provider.InitProperties(avatarObject.GetComponentsInSelfAndChildren<Renderer>(), propertyNameProp);
                            }
                        }
                    } else {
                        provider.InitProperties(rendererProp.objectReferenceValue as Renderer, propertyNameProp);
                    }
                    UnityEditor.Experimental.GraphView.SearchWindow.Open(searchContext, provider);
                }
            }
            case nameof(ScaleAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Scale") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("obj"));
                propField.style.flexGrow = 1;
                row.Add(propField);
            
                var propField3 = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("scale"));
                propField3.style.flexGrow = 0;
                propField3.style.flexBasis = 50;
                row.Add(propField3);

                return row;
            }
            case nameof(ObjectToggleAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Object Toggle") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("obj"));
                propField.style.flexGrow = 1;
                row.Add(propField);

                return row;
            }
            case nameof(SpsOnAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label("Enable SPS") {
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("target"));
                propField.style.flexGrow = 1;
                row.Add(propField);

                return row;
            }
            case nameof(BlendShapeAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label {
                    text = "BlendShape",
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var blendshapeProp = prop.FindPropertyRelative("blendShape");
                var propField = VRCFuryEditorUtils.Prop(blendshapeProp);
                propField.style.flexGrow = 1;
                row.Add(propField);
            
                var valueField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("blendShapeValue"));
                valueField.style.flexGrow = 0;
                valueField.style.flexBasis = 50;
                row.Add(valueField);

                System.Action selectButtonPress = () => {
                    var editorObject = prop.serializedObject.targetObject;
                    var shapes = new Dictionary<string,string>();
                    if (editorObject is UnityEngine.Component c) {
                        VFGameObject avatarObject = c.owner().GetComponentInSelfOrParent<VRCAvatarDescriptor>()?.owner();
                        if (avatarObject) {
                            foreach (var skin in avatarObject.GetComponentsInSelfAndChildren<SkinnedMeshRenderer>()) {
                                if (!skin.sharedMesh) continue;
                                for (var i = 0; i < skin.sharedMesh.blendShapeCount; i++) {
                                    var bs = skin.sharedMesh.GetBlendShapeName(i);
                                    if (shapes.ContainsKey(bs)) {
                                        shapes[bs] += ", " + skin.owner().name;
                                    } else {
                                        shapes[bs] = skin.owner().name;
                                    }
                                }
                            }
                        }
                    }

                    var menu = new GenericMenu();
                    foreach (var entry in shapes.OrderBy(entry => entry.Key)) {
                        menu.AddItem(
                            new GUIContent(entry.Key + " (" + entry.Value + ")"),
                            false,
                            () => {
                                blendshapeProp.stringValue = entry.Key;
                                blendshapeProp.serializedObject.ApplyModifiedProperties();
                            });
                    }
                    menu.ShowAsContext();
                };
                var selectButton = new Button(selectButtonPress) { text = "Select" };
                row.Add(selectButton);

                return row;
            }
            case nameof(AnimationClipAction): {
                var row = new VisualElement {
                    style = {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.FlexStart
                    }
                };

                var label = new Label {
                    text = "Animation Clip",
                    style = {
                        flexGrow = 0,
                        flexBasis = 100
                    }
                };
                row.Add(label);

                var propField = VRCFuryEditorUtils.Prop(prop.FindPropertyRelative("clip"));
                propField.style.flexGrow = 1;
                row.Add(propField);

                return row;
            }
        }

        return VRCFuryEditorUtils.WrappedLabel($"Unknown action type: {type}");
    }
}

}
