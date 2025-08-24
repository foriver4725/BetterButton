# BetterButton

## Description
This library provides a wrapper class for button.OnClickAsObservable() that prevents rapid consecutive clicks and overlapping presses.

## How to Setup

### 1. Install UniTask and R3
This library depends on [UniTask](https://github.com/Cysharp/UniTask) and [R3](https://github.com/Cysharp/R3).<br/>
The lowest versions required are:
<table>
    <tr>
        <td>UniTask</td>
        <td>2.5.10</td>
    </tr>
    <tr>
        <td>R3</td>
        <td>1.3.0</td>
    </tr>
</table>

### 2. Install via Git URL
Install from:
```
https://github.com/foriver4725/BetterButton.git?path=Assets/foriver4725/BetterButton
```

### 3. (When necessary) Setup .asmdef reference
Add `foriver4725.BetterButton.asmdef` to your assembly definition references.

### 4. Add a using directive
At the top of your source code file, add:
```cs
using foriver4725.BetterButton;
```

## Examples
You can also see this program at the `Sample.cs` script.
```cs
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace foriver4725.BetterButton
{
    internal sealed class Sample : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;

        // instantiate the class before you use it
        private readonly BetterButton betterButton = new(0.3f);

        private void Start()
        {
            // non-await version
            {
                // minimum example
                // you should not capture outer variables in the lambda expression,
                // because it causes GC.Alloc.
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.Subscribe(buttons[i], static () =>
                    {
                        Debug.Log("Clicked");
                    }, destroyCancellationToken);
                }

                // if you want to use an outer variable, use this version.
                // in this method, the passed parameter is captured once on .Subscribe().
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.Subscribe(buttons[i], i, static index =>
                    {
                        Debug.Log(index);
                    }, destroyCancellationToken);
                }

                // if you want to use multiple outer variables, use this version.
                // in this method, the passed parameter is captured once on .Subscribe().
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.Subscribe(buttons[i], (Index: i, Name: buttons[i].gameObject.name), static param =>
                    {
                        Debug.Log(param.Index + " by " + param.Name);
                    }, destroyCancellationToken);
                }
            }

            // await version
            {
                // minimum example
                // you should not capture outer variables in the lambda expression,
                // because it causes GC.Alloc.
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.SubscribeAwait(buttons[i], static async ct =>
                    {
                        await UniTask.DelayFrame(64, cancellationToken: ct);
                        Debug.Log("Clicked");
                    }, destroyCancellationToken);
                }

                // if you want to use an outer variable, use this version.
                // in this method, the passed parameter is captured once on .Subscribe().
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.SubscribeAwait(buttons[i], i, static async (index, ct) =>
                    {
                        await UniTask.DelayFrame(64, cancellationToken: ct);
                        Debug.Log(index);
                    }, destroyCancellationToken);
                }

                // if you want to use multiple outer variables, use this version.
                // in this method, the passed parameter is captured once on .Subscribe().
                for (int i = 0; i < buttons.Length; i++)
                {
                    betterButton.SubscribeAwait(buttons[i], (Index: i, Name: buttons[i].gameObject.name), static async (param, ct) =>
                    {
                        await UniTask.DelayFrame(64, cancellationToken: ct);
                        Debug.Log(param.Index + " by " + param.Name);
                    }, destroyCancellationToken);
                }
            }
        }
    }
}
```