using System;
using System.Collections.Generic;
using Script.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

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
    }
    
    internal class AddCloseButton : IUIBuilderCommand {
        public AddCloseButton() {
            
        }
        public void Execute(RectTransform rectTransform) {
            GameObject textObject =
                Addressables.InstantiateAsync("UI/Button", rectTransform).WaitForCompletion();
            textObject.GetComponent<LayoutElement>().preferredHeight *= 0.15f;
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