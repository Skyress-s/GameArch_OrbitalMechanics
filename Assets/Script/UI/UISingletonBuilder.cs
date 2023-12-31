﻿using System;
using System.Collections.Generic;
using Script.Interfaces;
using Script.NonECSScripts;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
// //FileName: UISingletonBuilder.cs
// //FileType: Visual C# Source file
// //Author : Mathias Mohn Mørch
// //Created On : 12/12/2023
// //Last Modified On : 12/12/2023
// //Copy Rights : Mathias Mohn Mørch
// //Description :
// //////////////////////////////////////////////////////////////////////////
// //////////////////////////////
namespace Script.UI {
    // Builder / Factory pattern
    public class UISingletonBuilder {
        public List<IUIBuilderCommand> _commands = new List<IUIBuilderCommand>();

        public void Build(Transform transform, Vector3 vector3) {
            RectTransform trans = UISingleton.Instance.RequestAt(transform, vector3);
            
            _commands.Insert(0, new AddCloseButton());
            foreach (var command in _commands) {
                command.Execute(trans);
            }
        }
        
        public UISingletonBuilder AddText(string text) {
            _commands.Add(new AddStringCommand(text));
            return this;
        }
        
        public UISingletonBuilder AddButton(string text, System.Action onClick) {
            _commands.Add(new AddButtonCommand(text, onClick));
            return this;
        }
        
        public UISingletonBuilder AddArrowVisualizer(CelestialBody celestialBody) {
            _commands.Add(new AddArrowVisualizerCommand(celestialBody));
            return this;
        }
        
        public UISingletonBuilder AddMassSlider(CelestialBody celestialBody) {
            _commands.Add(new AddMassSliderCommand(celestialBody));
            return this;
        }
    }

    internal class AddMassSliderCommand : IUIBuilderCommand {
        private CelestialBody _targetCelestialBody;
        public AddMassSliderCommand(CelestialBody celestialBody) {
            _targetCelestialBody = celestialBody;
        }
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/Slider", rectTransform).WaitForCompletion();
            
            
            TMP_Text textUI = textObject.GetComponentInChildren<TMP_Text>();
            Slider slider = textObject.GetComponent<Slider>();

            float scalar = _targetCelestialBody.unitScales["mass"];
            
            // set initial value
            slider.value = _targetCelestialBody.Mass / scalar;
            
            // Set value
            slider.onValueChanged.AddListener((float x) => {
                textUI.text = $"Solar Masses: {x:F1}";
                _targetCelestialBody.Mass = Mathf.Lerp(0.00001f, 1f, x) * scalar;
            });
        }
    }
    
    internal class AddArrowVisualizerCommand : IUIBuilderCommand {
        private CelestialBody _targetCelestialBody;
        public AddArrowVisualizerCommand(CelestialBody celestialBody) {
            _targetCelestialBody = celestialBody;
        }
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/ArrowButtons", rectTransform).WaitForCompletion();
            
            textObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                _targetCelestialBody.ArrowType = ArrowMode.Disabled;
            });
            textObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                _targetCelestialBody.ArrowType = ArrowMode.Force;
            });
            textObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => {
                _targetCelestialBody.ArrowType = ArrowMode.Velocity;
            });
        }
    }

    internal class AddCloseButton : IUIBuilderCommand {
        public AddCloseButton() {
            
        }
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/Button", rectTransform).WaitForCompletion();
            textObject.GetComponent<LayoutElement>().preferredHeight *= 0.25f;
            textObject.GetComponentInChildren<TMP_Text>().text = "X";
            textObject.GetComponentInChildren<Image>().color = Color.red;
            textObject.GetComponentInChildren<Button>().onClick.AddListener(() => {
                UISingleton.Instance.Empty();
            });
        }
    }

    internal class AddButtonCommand : IUIBuilderCommand {
        private string text;
        private Action onClick;
        public AddButtonCommand(string text, System.Action onClick) {
            this.text = text;
            this.onClick = onClick;
        }
        
        
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/Button", rectTransform).WaitForCompletion();
            textObject.GetComponentInChildren<TMP_Text>().text = text;
            textObject.GetComponentInChildren<Button>().onClick.AddListener(() => { onClick();});
        }
    }
    
    internal class AddStringCommand : IUIBuilderCommand {
        private string text;
        public AddStringCommand(string text) {
            this.text = text;
        }
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/Text", rectTransform).WaitForCompletion();
            textObject.GetComponentInChildren<TMP_Text>().text = text;
            
            return;
            Addressables.InstantiateAsync("UI/Text", rectTransform).Completed += handle => {
                handle.Result.GetComponent<UnityEngine.UI.Text>().text = text;
            };
            
            
        }
    }
}