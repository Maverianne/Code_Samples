using _9YoS.Scripts.Managers;
using _9YoS.Scripts.UI.Alert.ScriptableObjects;
using UnityEngine;

namespace _9YoS.Scripts.UI.Skill
{
    public class ImageReferenceLocalizationManager : MonoBehaviour
    {
        public string GetLocalizedStringWithUiTags(Tutorial tutorial)
        {
            if (tutorial.ActionName.Length == 0) return MainManager.Instance.LocalizationManager.GetLocalizedString(tutorial.Tag);
            
            var actionNames = tutorial.ActionName;
            var spriteTags = new string[actionNames.Length];
            for (int i = 0; i < actionNames.Length; i++)
            {
                spriteTags[i] = actionNames[i].ToString();
            }
            for (var i = 0; i < spriteTags.Length; i++)
            {
                spriteTags[i] =   MainManager.Instance.InputUtilites.GetTextMeshProSpriteTag(spriteTags[i]); 
                if(spriteTags[i] == null) return MainManager.Instance.LocalizationManager.GetLocalizedString(tutorial.Tag);
            } 
            return MainManager.Instance.LocalizationManager.GetLocalizedString(tutorial.Tag, spriteTags);
        }

        public string GetLocalizedStringWithUiTags(string description, ConstantsManager.Input.ActionName[] actions)
        {
            if (actions.Length == 0)  return MainManager.Instance.LocalizationManager.GetLocalizedString(description);
            
            var spriteTags = new string[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                spriteTags[i] = actions[i].ToString();
            }
            for (var i = 0; i < spriteTags.Length; i++)
            {
                spriteTags[i] =   MainManager.Instance.InputUtilites.GetTextMeshProSpriteTag(spriteTags[i]); 
                if(spriteTags[i] == null) return MainManager.Instance.LocalizationManager.GetLocalizedString(description);
            } 
            return MainManager.Instance.LocalizationManager.GetLocalizedString(description, spriteTags);
        }
    public string GetLocalizedStringWithUiTags(string description, ConstantsManager.Input.UIActionName[] actions)
        {
            if (actions.Length == 0)  return MainManager.Instance.LocalizationManager.GetLocalizedString(description);
            
            var spriteTags = new string[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                spriteTags[i] = actions[i].ToString();
            }
            for (var i = 0; i < spriteTags.Length; i++)
            {
                spriteTags[i] =   MainManager.Instance.InputUtilites.GetTextMeshProSpriteTag(spriteTags[i]); 
                if(spriteTags[i] == null) return MainManager.Instance.LocalizationManager.GetLocalizedString(description);
            } 
            return MainManager.Instance.LocalizationManager.GetLocalizedString(description, spriteTags);
        }

        public ConstantsManager.Input.ControllerButtons[] GetTutorialAnimation(ConstantsManager.Input.ActionName[] actions)
        {
            if (actions.Length == 0)  return null;
            var spriteTags = new string[actions.Length];
            var buttonArray = new ConstantsManager.Input.ControllerButtons[actions.Length];
            
            for (int i = 0; i < actions.Length; i++)
            {
                spriteTags[i] = actions[i].ToString();
            }

            for (int  i= 0; i < spriteTags.Length; ++i)
            {
                buttonArray[i] = MainManager.Instance.InputUtilites.GetTutorialInput(spriteTags[i]);
            }
            
            return buttonArray;
        }

        public string GetLocalizedButtonUI(ConstantsManager.Input.UIActionName action)
        {
            var spriteTags = action.ToString();
            spriteTags =   MainManager.Instance.InputUtilites.GetTextMeshProSpriteTag(spriteTags); 
            return spriteTags;
        }     
        public string GetLocalizedButton(ConstantsManager.Input.ActionName action)
        {
            var spriteTags = action.ToString();
            spriteTags =   MainManager.Instance.InputUtilites.GetTextMeshProSpriteTag(spriteTags); 
            return spriteTags;
        }

    }
}