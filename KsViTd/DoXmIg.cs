using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace KsViTd {
    class DoXmIg {

        public DoXmIg Test() {
            TestInvoke2();

            System.Xml.Linq.XNamespace ns1 = "";



            return this;
        }

        #region 委托

        /// <summary>
        /// 异步委托
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke1() {
            Func<int, int, int> fun = (a, sleepMs) => {
                System.Threading.Thread.Sleep(sleepMs);
                return ++a;
            };

            var asyncR = fun.BeginInvoke(2, 5000
                , (o) => Console.WriteLine(o.AsyncState)
                , "a");

            // asyncR.IsCompleted 线程是否执行完成。
            //while(false == asyncR.IsCompleted) {
            //    Console.Write(".");
            //    System.Threading.Thread.Sleep(200);
            //}

            var funResult = fun.EndInvoke(asyncR);      // EndInvoke 会一直等待委托执行完成。

            return this;
        }


        /// <summary>
        /// 等待句柄
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke2() {
            Func<int, int, int> fun = (a, sleepMs) => {
                var count = sleepMs / 200;
                for (var i = 0; i < count; ++i) {
                    Console.Write("+");
                    System.Threading.Thread.Sleep(200);
                }
                System.Threading.Thread.Sleep(sleepMs % 200);

                return ++a;
            };

            var asyncR = fun.BeginInvoke(2, 5000
                , (o) => Console.WriteLine(o.AsyncState)
                , "a");

            while (true) {
                Console.Write("_");
                if (asyncR.AsyncWaitHandle.WaitOne(1000, true)) {
                    Console.WriteLine("Ok");
                    break;
                }
                Console.Write(".");
                System.Threading.Thread.Sleep(200);
            }

            var funResult = fun.EndInvoke(asyncR);      // EndInvoke 会一直等待委托执行完成。

            return this;
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke3() {
            Func<int, int, int> fun = (a, sleepMs) => {
                System.Threading.Thread.Sleep(sleepMs);
                return ++a;
            };

            // 倒数第二个参数, 可以传递一个回调函数, 这个函数是委托线程中调用的, 而不是在主线程中.
            // 最后一个参数, 可以传递任何对象, 可以传递委托本身, 然后使用 EndInvoke 获取返回结果
            var asyncR = fun.BeginInvoke(2, 2000
                , (ar) => {
                    Console.WriteLine(
                        (ar.AsyncState as Func<int, int, int>).EndInvoke(ar));
                }
                , fun);


            System.Diagnostics.Trace.Assert(false);
            return this;
        }

        #endregion

        #region Thread
        // 只要有一个前台线程在运行, 应用程序的进程就在运行.
        // 如果有多个前台线程在运行, 而 Main() 方法结束了, 程序的进程仍然是激活的, 直到所有前台进程执行完成为止.
        // 默认情况下, 用 Thread 类创建的线程是前台线程, 线程池中的线程是后台线程.


        public DoXmIg Thread1() {
            new Thread(
                obj => Console.WriteLine(obj))
            .Start("start");


            Console.WriteLine("---");
            return this;
        }

        public DoXmIg Thread2() {
            var t = new Thread(obj => {
                try {
                    Console.WriteLine("..");
                    Thread.Sleep(3000);
                    Console.WriteLine(obj);
                } catch (Exception e) {
                    throw e;
                }
            });

            t.Start(2222);
            Thread.Sleep(1000);
            t.Abort();


            Console.WriteLine("---");
            return this;
        }

        public DoXmIg Thread3() {
            return this;
        }

        #endregion


        // 线程池中的所有线程都是后台线程,
        // 不能给线程池中的线程设置优先级或名称
        // 入池的线程应该是短时间工作的, 而不是那种一直运行的线程.
        public DoXmIg ThreadPool1() {
            ThreadPool.GetMaxThreads(out var workerThreads, out var completionPortThreads);
            Console.WriteLine($"最大工作线程:{ workerThreads }, 最大IO线程:{ workerThreads }");


            WaitCallback call = (obj) => {
                Thread.Sleep(1000);
                Console.WriteLine(obj + "," + Thread.CurrentThread.ManagedThreadId);
            };

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Console.WriteLine("-----------");
            for (var i = 0; i < 100; ++i) {
                ThreadPool.QueueUserWorkItem(call, "A");     //加入队列的时候就直接运行了.
            }
            ThreadPool.QueueUserWorkItem(
                (obj) => {
                    var watch = (obj as System.Diagnostics.Stopwatch);
                    watch.Stop();
                    Console.WriteLine($"当前耗时:" + watch.Elapsed);
                }
                , stopwatch);

            //AutoResetEvent
            //Interlocked.Decrement()

            return this;
        }

        public DoXmIg ThreadPool2() {
            //ExecutionContext
            // 将一些数据放到当前线程的“执行上下文”
            System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("Name", "AAA");

            ThreadPool.QueueUserWorkItem(async state => {
                await Task.Delay(1000 * 2);
                Console.WriteLine("Name=" + System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Name"));
            });

            // 阻止当前线程的“执行上下文”流动
            ExecutionContext.SuppressFlow();

            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name=" + System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Name")));

            ExecutionContext.RestoreFlow();
            Console.WriteLine("RestoreFlow");

            return this;
        }

        #region Task


        public DoXmIg Task1() {
            Action<object> call = (obj) => {
                Console.WriteLine($"TaskId:{Task.CurrentId}, {obj}");
            };

            // 
            new TaskFactory().StartNew(call, "new TaskFactory");

            //new Task()    TaskStatus.Created 
            //.Start()      开始执行
            new Task(call, "new Task").Start();

            // 
            Task.Factory.StartNew(call, "Task.Factory.StartNew");

            // TaskCreationOptions.LongRunning 通知任务调度器, 该任务需要较长时间, 这样调度器更可能会使用新线程
            Task.Factory.StartNew(
                () => {
                    for (var i = 0; i < 10; ++i) {
                        Thread.Sleep(200);
                        Console.Write(".");
                    }
                    Console.WriteLine(";");
                }
                , TaskCreationOptions.LongRunning);

            Task.Run(() => Console.WriteLine("0000000"));

            return this;
        }

        /// <summary>
        /// 连续任务
        /// </summary>
        /// <returns></returns>
        public DoXmIg Task2() {
            Func<object, string> fun1 = (a) => {
                Console.WriteLine($".....fun1-start, {DateTime.Now.ToLongTimeString()}, ");
                Thread.Sleep(1200);
                Console.WriteLine($"...........fun1, {DateTime.Now.ToLongTimeString()}, " + a);
                return $"[fun1 :{a}]";
            };

            Func<Task<string>, object, string> fun2 = (t, s) => {
                Console.WriteLine($".....fun2-start, {DateTime.Now.ToLongTimeString()}, ");
                Thread.Sleep(3000);
                Console.WriteLine($"...........fun2, {DateTime.Now.ToLongTimeString()}, " + s);
                return $"{t.Result},[fun2 :{s}]";
            };
            Func<Task<string>, object, string> fun3 = (t, s) => {
                Thread.Sleep(5100);
                Console.WriteLine($"...........fun3, {DateTime.Now.ToLongTimeString()}, " + s);
                return $"{t.Result},[fun3 :{s}]";
            };

            // 先执行 fun1, 结束后执行两个任务 fun2,fun3
            // fun3 执行完后, 再执行附加任务
            var task1 = new Task<string>(fun1, 1000);
            var task2 = task1.ContinueWith(fun2, "sssss");
            task1.ContinueWith(fun3, "aaaaaaaaaa")
                .ContinueWith((t) => Console.WriteLine(".........fun3 之后的任务"), TaskContinuationOptions.NotOnFaulted);

            task1.Start();// 0.2s 左右 task1 执行完毕, 然后异步执行 fun2, fun3, 当前线程执行下面的代码, 
            //task2.Wait();     可以手动等待线程执行完毕

            // 调用 task2.Result 会阻塞当前线程, 等待 task2 的结果
            Console.WriteLine("\n" + "Result:" + task2.Result);
            return this;
        }

        /// <summary>
        /// 连续任务
        /// </summary>
        /// <returns></returns>
        public DoXmIg Task3() {
            Action child = () => {
                Console.WriteLine("Id:" + Task.CurrentId);
                for (var i = 0; i < 20; ++i) {
                    Thread.Sleep(200);
                    Console.Write(",");
                }
            };

            Action parent = () => {
                Console.WriteLine("Id:" + Task.CurrentId);

                Task.Factory.StartNew(child);

                for (int i = 0; i < 20; ++i) {
                    Thread.Sleep(300);
                    Console.Write("-");
                }
            };

            Task.Factory.StartNew(parent);


            return this;
        }

        public DoXmIg Task4() {
            Func<CancellationToken, int, int> fnSum = (ct, n) => {
                int sum = 0;
                for (; n > 0; n--) {
                    // 抛出 OperationCanceledException 异常，表示 Task 没有一直执行结束
                    ct.ThrowIfCancellationRequested();
                    checked { sum += n; }
                }
                return sum;
            };

            var cts = new CancellationTokenSource();
            var t1 = Task.Run(() => fnSum(cts.Token, 1_000_000_000));
            //cts.Cancel();     // 如果取消之前 Task 没有执行 Start，会直接取消，不会执行
            cts.CancelAfter(100);

            try {
                // 可能会抛出 AggregateException 异常，可能是 OperationCanceledException 或 OverflowException
                Console.WriteLine($"sum = {t1.Result}");
            } catch (AggregateException aEx) {
                try {
                    // 如果还存在未处理的异常会继续抛出新的 AggregateException
                    aEx.Handle(e => e is OperationCanceledException);
                    Console.WriteLine("canceled");
                } catch (AggregateException aEx2) {
                    aEx2.Handle(e => e is OverflowException);
                    Console.WriteLine("run fnSum --OverflowException");
                }
            }

            return this;
        }

        public DoXmIg Task5() {
            var parent = new Task<int[]>(() => {
                Console.WriteLine("parent start" + DateTime.Now.ToLongTimeString());
                var ls = new int[3];

                new Task(() => {
                    Thread.Sleep(1000);
                    ls[0] = 1000;
                }, TaskCreationOptions.AttachedToParent)
                .Start();
                new Task(() => {
                    Thread.Sleep(1500);
                    ls[1] = 1500;
                    Console.WriteLine("1 --" + DateTime.Now.ToLongTimeString());
                }, TaskCreationOptions.AttachedToParent)
                .Start();
                new Task(() => {
                    Thread.Sleep(2000);
                    ls[2] = 2000;
                    Console.WriteLine("2 --" + DateTime.Now.ToLongTimeString());
                }, TaskCreationOptions.AttachedToParent)
                .Start();

                // TaskCreationOptions.AttachedToParent 附加到当前 Task，当前 Task 不会认为已经执行完毕了，
                // 还需要这三个Task 执行完毕才返回结果
                return ls;
            });

            parent.ContinueWith((t) => Console.WriteLine(string.Join(",", t.Result)));
            parent.Start();
            var tf = new TaskFactory<int>();

            Console.WriteLine("Task5 Return ---" + DateTime.Now.ToLongTimeString());
            return this;
        }

        #endregion

        #region Parallel

        /// <summary>
        /// Parallel.For    Break   Stop
        /// </summary>
        /// <returns></returns>
        public DoXmIg Parallel1() {
            Parallel.For(10, 50, (i, state) => {
                var info = $"i:{i}, TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                if (i > 30) {
                    Console.Write("!!!!Break, " + info);
                    state.Break();          //结束当前线程的下一次循环, 不会跳出当前块的代码, 后面的代码还是会运行, 不像 Break 关键字有控制代码块的效果

                    //Console.Write("!!!!Stop, " + info);
                    //state.Stop();           // 所有的线程都不会进行下一次循环
                } else {
                    Thread.Sleep(100);
                    Console.Write("----" + info);
                }

                Console.WriteLine(" [End]");
                Thread.Sleep(200);
            });


            return this;
        }

        /// <summary>
        /// Parallel.For    localInit    localFinally
        /// </summary>
        /// <returns></returns>
        public DoXmIg Parallel2() {
            Parallel.For(0, 110_000_000
                , () => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    //Console.WriteLine("线程的初始化," + info);
                    return 1000;
                }
                , (i, state, local) => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    //Console.WriteLine(i.ToString() + ", " + info);
                    local += 1;
                    //Thread.Sleep(10);

                    return local;
                }
                , (local) => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    Console.WriteLine($"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}, local;{local}");
                });

            return this;
        }

        public DoXmIg Parallel3() {

            Parallel.ForEach(new[] { "AA", "BB", "CC" }, (str, state, i) => {
                Console.WriteLine(str + i.ToString());
            });

            // 并行多个 Action()
            Parallel.Invoke(
                () => Console.WriteLine($"---------- TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($"++++++++++ TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($"========== TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($",,,,,,,,,, TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}"));

            return this;
        }

        public DoXmIg Parallel4() {
            Console.WriteLine("==========");
            var arr = new List<int>();
            Parallel.ForEach(Enumerable.Range(1, 10_000_000), () => 0,
                (i, state, sum) => {
                    return sum + (i / 2 == 0 ? 1 : 3);
                }, (sum) => {
                    arr.Add(sum);
                });
            Console.WriteLine("==========");
            Console.WriteLine($"{arr.Count}, {arr.Sum()}");
            var sum2 = 0;
            for (var i = 0;i <= 10_000_000; i++) {
                sum2 += i / 2 == 0 ? 1 : 3;
            }
            Console.WriteLine(sum2);
            System.Collections.Concurrent.ConcurrentBag<int> ls;
            return this;
        }
        #endregion

        public DoXmIg Timer1() {
            var t1 = new Timer(o => Console.WriteLine(o), "ss", 1000 * 10, 1000 * 3);
            // t1 对象如果被垃圾回收，回调将不会执行，所以要有一个变量或成员保持 Timer 的存活
            return this;
        }

        public DoXmIg Timer2() {
            Timer t2 = null;        // 必须提前声明赋值，闭包报错
            t2 = new Timer((o) => {
                t2.Change(1000 * 3, -1);        // 运行正常，但不知道会不会可能为 null，然后出错
                Console.WriteLine(DateTime.Now.ToLongTimeString());
            }, null, 0, -1);
            return this;
        }

        #region Task StateMachine
        // 抄录自《CLR via C#》 28.4

        internal sealed class Type1 { }

        internal sealed class Type2 { }

        private static async Task<Type1> Method1Async() {
            await Task.Delay(1000);
            return new Type1();
        }

        private static async Task<Type2> Method2Async() {
            await Task.Delay(300);
            return new Type2();
        }

        public static async Task<string> TaskAsync1(int argument) {
            int locat = argument;
            try {
                Type1 result1 = await Method1Async();
                for (var x = 0; x < 3; x++) {
                    Type2 result2 = await Method2Async();
                }
            } catch (Exception) {
                Console.WriteLine("Catch");
            } finally {
                Console.WriteLine("Finally");
            }
            return "Done";
        }

        // IL逆向，转换的代码
        [DebuggerStepThrough, AsyncStateMachine(typeof(StateMachine))]
        public static Task<string> TaskAsync2(int argument) {
            StateMachine stateMachine = new StateMachine() {
                m_builder = AsyncTaskMethodBuilder<string>.Create(),
                m_state = -1,
                m_argument = argument
            };

            stateMachine.m_builder.Start(ref stateMachine);
            return stateMachine.m_builder.Task;
        }

        struct StateMachine : System.Runtime.CompilerServices.IAsyncStateMachine {
            public AsyncTaskMethodBuilder<string> m_builder;
            public int m_state;
            public int m_argument, m_local, m_x;
            public Type1 m_resultType1;
            public Type2 m_resultType2;

            private TaskAwaiter<Type1> m_awaiterType1;
            private TaskAwaiter<Type2> m_awaiterType2;

            void IAsyncStateMachine.MoveNext() {
                string result = null;

                // 编译器插入 try 块来确保状态机的任务完成，很有可能程序员编写的代码抛出异常，
                try {
                    bool executeFinally = true;     
                    if (m_state == -1) { m_local = m_argument; }

                    // 开始插入程序员编写的原始代码
                    // 原始代码中的 Try 块
                    try {
                        TaskAwaiter<Type1> awaiterType1;
                        TaskAwaiter<Type2> awaiterType2;

                        switch (m_state) {
                        case -1:
                            awaiterType1 = Method1Async().GetAwaiter();
                            if (!awaiterType1.IsCompleted) {
                                m_state = 0;    // 原始代码要以异步的方式完成
                                m_awaiterType1 = awaiterType1;
                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType1, ref this);

                                executeFinally = false;
                                return;
                            }
                            // else 原始代码以同步的方式完成，因为后面会执行 GetResult()，会阻塞当前线程
                            break;
                        case 0:     // 原始代码以异步的方式完成了
                            awaiterType1 = m_awaiterType1;      // 恢复最新的 awaiter
                            // ? 这里为什么要赋值，直接使用 ref awaiterType1，不是有值了吗，或者可以直接使用类成员
                            break;
                        case 1:
                            awaiterType2 = m_awaiterType2;
                            goto ForLoopEpilog;
                        }

                        m_resultType1 = awaiterType1.GetResult();

                        ForLoopPrologue:
                        m_x = 0;
                        goto ForLoopBody;

                        ForLoopEpilog:
                        m_resultType2 = awaiterType2.GetResult();
                        m_x++;

                        ForLoopBody:
                        if (m_x < 3) {
                            awaiterType2 = Method2Async().GetAwaiter();
                            if (!awaiterType2.IsCompleted) {
                                m_state = 1;
                                m_awaiterType2 = awaiterType2;

                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType2, ref this);
                                executeFinally = false;
                                return;
                            }
                            goto ForLoopEpilog;
                        }

                    } catch (Exception) {
                        Console.WriteLine("Catch");
                    } finally {
                        // 离开了原始代码的 try 块，就执行这里的 finally
                        if (executeFinally) {
                            Console.WriteLine("Finally");
                        }
                    }
                } catch (Exception ex) {
                    // 未处理的异常：通过设置异常来完成状态机的 Task
                    m_builder.SetException(ex);
                    return;
                }
                m_builder.SetResult(result);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotImplementedException();
        }

        #endregion

        public static async Task T1() {
            
        }

        class ThreadSharingData {

        }
    }

}
