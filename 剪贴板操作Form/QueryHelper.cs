using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BestString
{
    public static class QueryHelper
    {
        private static Action<string, string[], TimeSpan> _callback;
        private static string[] _datas = null;
        private static ManualResetEvent _eventWaitHandle;
        private static string _param = string.Empty;
        private static object _syncRoot = new object();

        static QueryHelper()
        {
            _eventWaitHandle = new ManualResetEvent(false);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                string param;
                string[] datas;
                Action<string, string[], TimeSpan> callback;
                Stopwatch watch = new Stopwatch();
                string[] result;
                while (true)
                {
                    _eventWaitHandle.WaitOne();
                    _eventWaitHandle.Reset();
                    lock (_syncRoot)
                    {
                        param = _param;
                        datas = _datas;
                        callback = _callback;
                        _param = null;
                        _datas = null;
                        _callback = null;
                    }

                    watch.Restart();
                    //result = SearchHelper.Search(param, datas);

                    var q = from sentence in datas.AsParallel()
                            where sentence == param
                            select sentence;
                    result = q.ToArray();

                    watch.Stop();

                    callback?.Invoke(param, result, watch.Elapsed);

                    GC.Collect();
                }
            }).Start();
        }

        /// <summary>
        /// 查询相关字符串
        /// </summary>
        /// <param name="param">    查询关键字。 </param>
        /// <param name="datas">    查询列表。 </param>
        /// <param name="callback"> 回调函数。 </param>
        public static void Query(string param, string[] datas, Action<string, string[], TimeSpan> callback)
        {
            lock (_syncRoot)
            {
                _param = param;
                _datas = datas;
                _callback = callback;
            }
            _eventWaitHandle.Set();
        }
    }
}