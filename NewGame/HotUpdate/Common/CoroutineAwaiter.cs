using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using HotUpdate;
public class CoroutineAwaiter
{
    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
    public CoroutineAwaiter(IEnumerator enumerator)
    {
         UICtrl.Instance.StartCoroutine(WaitCoroutine(tcs, enumerator));
    }

    public System.Threading.Tasks.Task Task
    {
        get { return tcs.Task; }
    }

    private IEnumerator WaitCoroutine(TaskCompletionSource<object> tcs, IEnumerator enumerator)
    {
        yield return enumerator;
        tcs.SetResult(null);
    }
}
