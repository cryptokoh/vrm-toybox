using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using System.Threading.Tasks;
using UniGLTF;
using VRMShaders;
using SFB; // StandaloneFileBrowser
using UnityEngine.Events;

public class VRMLoader : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    private GameObject oldAvatar;
    
    // List to store all previously loaded avatars
    private List<GameObject> loadedAvatars = new List<GameObject>();

    public static UnityEvent<GameObject> OnAvatarChanged = new UnityEvent<GameObject>();

    public sealed class ImmediateCaller : VRMShaders.IAwaitCaller
    {
        public Task NextFrame()
        {
            return Task.FromResult<object>(null);
        }

        public Task Run(Action action)
        {
            action();
            return Task.FromResult<object>(null);
        }

        public Task<T> Run<T>(Func<T> action)
        {
            return Task.FromResult(action());
        }

        public Task NextFrameIfTimedOut() => NextFrame();
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClick);
        oldAvatar = GameObject.FindGameObjectWithTag("Avatar");
        if (oldAvatar == null)
        {
            Debug.LogError("No game object with tag 'Avatar' found!");
        }
        else
        {
            loadedAvatars.Add(oldAvatar);
        }
    }

    private async void OnClick()
    {
        var extensions = new [] {
            new ExtensionFilter("VRM Files", "vrm"),
            new ExtensionFilter("All Files", "*"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length > 0) 
        {
            var model = await VrmUtility.LoadAsync(paths[0], awaitCaller: new ImmediateCaller());
            var newAvatarModel = model.Root;

            // Set the position, parent and tag
            newAvatarModel.transform.position = oldAvatar.transform.position;
            newAvatarModel.transform.parent = oldAvatar.transform.parent;
            newAvatarModel.tag = "Avatar";

            // Copy the rotation of the Rigidbody if it exists
            // Copy the rotation from the transform of the old avatar
            newAvatarModel.transform.rotation = oldAvatar.transform.rotation;

            // Activate all SkinnedMeshRenderers in the new model
            foreach (var renderer in newAvatarModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.enabled = true;
            }

            // Add the new avatar to the loadedAvatars list
            loadedAvatars.Add(newAvatarModel);

            // Disable all old avatars
            foreach (var avatar in loadedAvatars)
            {
                if (avatar != newAvatarModel)
                {
                    avatar.SetActive(false);
                }
            }

            // Raise the event for avatar change
            OnAvatarChanged.Invoke(newAvatarModel);
        }
    }
}
