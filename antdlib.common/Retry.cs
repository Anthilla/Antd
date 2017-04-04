using System;
using System.Collections.Generic;
using System.Threading;

namespace antdlib.common {
    //taken from https://www.codeproject.com/Articles/1179853/Csharp-Generic-Retry-Mechanism
    public class Retry {
        public static TResult Execute<TResult>(
          Func<TResult> action,
          TimeSpan retryInterval,
          int retryCount,
          TResult expectedResult,
          bool isExpectedResultEqual = true,
          bool isSuppressException = true
           ) {
            TResult result = default(TResult);
            var succeeded = false;
            var exceptions = new List<Exception>();
            for(var retry = 0; retry < retryCount; retry++) {
                try {
                    if(retry > 0)
                        Thread.Sleep(retryInterval);
                    result = action();

                    if(isExpectedResultEqual)
                        succeeded = result.Equals(expectedResult);
                    else
                        succeeded = !result.Equals(expectedResult);
                }
                catch(Exception ex) {
                    exceptions.Add(ex);
                }
                if(succeeded)
                    return result;
            }

            if(!isSuppressException)
                throw new AggregateException(exceptions);
            else
                return result;
        }
    }
}
