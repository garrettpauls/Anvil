using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using FormattableSql.Core;
using FormattableSql.Core.Data;

namespace Anvil.Framework.Reactive
{
    public static class ReactiveFormattableSqlProviderExtensions
    {
        public static IObservable<TResult> RxQueryAsync<TResult>(
            this IFormattableSqlProvider sql,
            FormattableString query,
            Func<IAsyncDataRecord, Task<TResult>> createResult)
        {
            var subject = new ReplaySubject<TResult>();

            sql.QueryAsync(query, async row => { subject.OnNext(await createResult(row)); }).ContinueWith(task =>
            {
                if(task.Exception != null)
                {
                    subject.OnError(task.Exception.SingleOrFlattened());
                }

                subject.OnCompleted();
            });

            return subject;
        }
    }
}