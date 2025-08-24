using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using R3;

namespace foriver4725.BetterButton
{
    /// <summary>
    /// A wrapper class for button.OnClickAsObservable() that prevents rapid consecutive clicks and overlapping presses.
    /// </summary>
    public sealed class BetterButton
    {
        private readonly TimeSpan throttleDuration;
        private readonly bool isOnErrorResumeFailure;
        private readonly ReactiveProperty<bool> gate;

        public ReadOnlyReactiveProperty<bool> Gate => gate;

        public BetterButton(float throttleDuration, bool isOnErrorResumeFailure = false)
        {
            this.throttleDuration = TimeSpan.FromSeconds(throttleDuration);
            this.isOnErrorResumeFailure = isOnErrorResumeFailure;
            this.gate = new(true);
        }

        public IDisposable Subscribe(Button button, Action onNext, CancellationToken ct)
        {
            if (button == null)
            {
                Debug.LogError("Button is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            if (onNext == null)
            {
                Debug.LogError("onNext action is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            return GetOnClickObservable(button)
                .ThrottleFirst(throttleDuration)
                .Select((gate, onNext), static (_, param) => (param.gate, param.onNext))
                .Where(static param => param.gate.Value)
                .Subscribe(static param =>
                {
                    param.gate.Value = false;
                    try
                    {
                        param.onNext();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return;
                    }
                    finally
                    {
                        param.gate.Value = true;
                    }
                })
                .RegisterTo(ct);
        }

        public IDisposable Subscribe<T>(Button button, T state, Action<T> onNext, CancellationToken ct)
        {
            if (button == null)
            {
                Debug.LogError("Button is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            if (onNext == null)
            {
                Debug.LogError("onNext action is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            return GetOnClickObservable(button)
                .ThrottleFirst(throttleDuration)
                .Select((gate, onNext, state), static (_, param) => (param.gate, param.onNext, param.state))
                .Where(static param => param.gate.Value)
                .Subscribe(static param =>
                {
                    param.gate.Value = false;
                    try
                    {
                        param.onNext(param.state);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return;
                    }
                    finally
                    {
                        param.gate.Value = true;
                    }
                })
                .RegisterTo(ct);
        }

        public IDisposable SubscribeAwait(Button button, Func<CancellationToken, UniTask> onNextAsync, CancellationToken ct)
        {
            if (button == null)
            {
                Debug.LogError("Button is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            if (onNextAsync == null)
            {
                Debug.LogError("onNextAsync function is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            return GetOnClickObservable(button)
                .ThrottleFirst(throttleDuration)
                .Select((gate, onNextAsync), static (_, param) => (param.gate, param.onNextAsync))
                .Where(static param => param.gate.Value)
                .SubscribeAwait(static async (param, ct) =>
                {
                    param.gate.Value = false;
                    try
                    {
                        await param.onNextAsync(ct);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.LogWarning("Operation was canceled before completion.");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return;
                    }
                    finally
                    {
                        param.gate.Value = true;
                    }
                }, AwaitOperation.Drop)
                .RegisterTo(ct);
        }

        public IDisposable SubscribeAwait<T>(Button button, T state, Func<T, CancellationToken, UniTask> onNextAsync, CancellationToken ct)
        {
            if (button == null)
            {
                Debug.LogError("Button is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            if (onNextAsync == null)
            {
                Debug.LogError("onNextAsync function is null. Cannot subscribe.");
                return Disposable.Empty;
            }

            return GetOnClickObservable(button)
                .ThrottleFirst(throttleDuration)
                .Select((gate, onNextAsync, state), static (_, param) => (param.gate, param.onNextAsync, param.state))
                .Where(static param => param.gate.Value)
                .SubscribeAwait(static async (param, ct) =>
                {
                    param.gate.Value = false;
                    try
                    {
                        await param.onNextAsync(param.state, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.LogWarning("Operation was canceled before completion.");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return;
                    }
                    finally
                    {
                        param.gate.Value = true;
                    }
                }, AwaitOperation.Drop)
                .RegisterTo(ct);
        }

        //! No null check
        private Observable<Unit> GetOnClickObservable(Button button)
            => this.isOnErrorResumeFailure ?
                button.OnClickAsObservable().OnErrorResumeAsFailure() :
                button.OnClickAsObservable();
    }
}