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
                    betterButton.Subscribe(buttons[i], (Index: i, Time: Time.time), static param =>
                    {
                        Debug.Log(param.Index + " at " + param.Time);
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
                    betterButton.SubscribeAwait(buttons[i], (Index: i, Time: Time.time), static async (param, ct) =>
                    {
                        await UniTask.DelayFrame(64, cancellationToken: ct);
                        Debug.Log(param.Index + " at " + param.Time);
                    }, destroyCancellationToken);
                }
            }
        }
    }
}